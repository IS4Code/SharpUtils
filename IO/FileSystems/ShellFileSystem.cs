/* Date: 5.9.2017, Time: 1:58 */
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Web;
using IllidanS4.SharpUtils.COM;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	/// <summary>
	/// This file system contains locations accessible using the Win32 shell model.
	/// The resources in this system may be real files, but also virtual shell items.
	/// </summary>
	public class ShellFileSystem : IFileSystem
	{
		public static readonly ShellFileSystem Instance = new ShellFileSystem(new Uri("shell:"));
		
		private readonly Uri baseUrl;
		
		public ShellFileSystem(Uri baseUrl)
		{
			this.baseUrl = baseUrl;
		}
		
		public string GetShellPath(Uri url)
		{
			var rel = baseUrl.MakeRelativeUri(url);
			if(rel.IsAbsoluteUri) throw new ArgumentException("URI is not within this subsystem.", "url");
			
			string path = rel.OriginalString.Replace('/', Path.DirectorySeparatorChar);
			
			if(path.StartsWith(@".\")) path = path.Substring(2);
			
			return "shell:"+HttpUtility.UrlDecode(path);
		}
		
		public Uri GetShellUrl(string path, bool isFileSystem)
		{
			Uri relUrl;
			if(isFileSystem || File.Exists(path))
			{
				relUrl = new Uri(@"MyComputerFolder\"+path, UriKind.Relative);
				//relUrl = new Uri(path, UriKind.Relative);
			}else if(path.StartsWith("shell:", StringComparison.OrdinalIgnoreCase))
			{
				relUrl = new Uri(path.Substring(6), UriKind.Relative);
			}else{
				relUrl = new Uri(path, UriKind.Relative);
			}
			return new Uri(baseUrl, relUrl);
		}
		
		static readonly Regex pathNameRegex = new Regex(@"^(shell:.*?\\?)([^\\]*)$", RegexOptions.Compiled);
		private IShellItem GetItem(Uri url)
		{
			string path = GetShellPath(url);
			return GetItem(path);
		}
		
		private IShellItem GetItem(string path)
		{
			try{
				//try{
					return SHCreateItemFromParsingName<IShellItem>(path, null);
				/*}catch(IOException)
				{
					if(!path.StartsWith("shell:", StringComparison.OrdinalIgnoreCase)) throw;
					return SHCreateItemFromParsingName<IShellItem>(path.Substring(6), null);
				}*/
			}catch(IOException)
			{
				var match = pathNameRegex.Match(path);
				if(!match.Success) throw new ArgumentException("Argument is not a valid path.", "path");
				
				string dir = match.Groups[1].Value;
				if(dir.Equals("shell:", StringComparison.OrdinalIgnoreCase)) dir = "";
				
				IntPtr pidl;
				SFGAOF sfgao;
				SHParseDisplayName(dir, null, out pidl, 0, out sfgao);
				try{
					path = match.Groups[2].Value;
					var psf = SHBindToObject<IShellFolder>(null, pidl, null);
					
					try{
						uint tmp;
						IntPtr pidl2;
						psf.ParseDisplayName(IntPtr.Zero, null, path, out tmp, out pidl2, 0);
						try{
							return SHCreateItemWithParent<IShellItem>(pidl, psf, pidl2);
						}finally{
							Marshal.FreeCoTaskMem(pidl2);
						}
					}catch(ArgumentException)
					{
						if(!String.IsNullOrWhiteSpace(path)) throw;
						return SHCreateItemFromIDList<IShellItem>(pidl);
					}catch(FileNotFoundException)
					{
						//Probably not needed
						IEnumIDList peidl = psf.EnumObjects(IntPtr.Zero, SHCONTF.SHCONTF_FOLDERS | SHCONTF.SHCONTF_NONFOLDERS);
						
						try{
							while(true)
							{
								IntPtr pidl2;
								uint num;
								peidl.Next(1, out pidl2, out num);
								if(num == 0) break;
								try{
									STRRET sr = psf.GetDisplayNameOf(pidl2, SHGDNF.SHGDN_FORPARSING);
									
									string name = StrRetToStr(ref sr, pidl2);
									if(pathNameRegex.Replace(name, "$2") == path)
									{
										return SHCreateItemWithParent<IShellItem>(pidl, psf, pidl2);
									}
								}finally{
									Marshal.FreeCoTaskMem(pidl2);
								}
							}
						}finally{
							Marshal.FinalReleaseComObject(peidl);
						}
					}finally{
						Marshal.FinalReleaseComObject(psf);
					}
				}finally{
					Marshal.FreeCoTaskMem(pidl);
				}
				throw;
			}
		}
		
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto, PreserveSig=false)]
		static extern string StrRetToStr(ref STRRET pstr, [Optional]IntPtr pidl);
		
		[DllImport("shell32.dll", CharSet=CharSet.Unicode, PreserveSig=false)]
		[return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex=2)]
		static extern object SHCreateItemFromParsingName(string pszPath, IBindCtx pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
		
		[DebuggerStepThrough]
		static T SHCreateItemFromParsingName<T>(string pszPath, IBindCtx pbc) where T : class
		{
			return (T)SHCreateItemFromParsingName(pszPath, pbc, typeof(T).GUID);
		}
		
		[DllImport("shell32.dll", PreserveSig=false)]
		[return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex=3)]
		static extern object SHBindToObject(IShellFolder psf, IntPtr pidl, IBindCtx pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
		
		[DebuggerStepThrough]
		static T SHBindToObject<T>(IShellFolder psf, IntPtr pidl, IBindCtx pbc) where T : class
		{
			return (T)SHBindToObject(psf, pidl, pbc, typeof(T).GUID);
		}
		
		[DllImport("shell32.dll", CharSet=CharSet.Unicode, PreserveSig=false)]
		static extern void SHParseDisplayName(string pszName, IBindCtx pbc, out IntPtr ppidl, SFGAOF sfgaoIn, out SFGAOF psfgaoOut);
		
		[DllImport("shell32.dll", PreserveSig=false)]
		[return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex=3)]
		static extern object SHCreateItemWithParent(IntPtr pidlParent, IShellFolder psfParent, IntPtr pidl, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
		
		[DebuggerStepThrough]
		static T SHCreateItemWithParent<T>(IntPtr pidlParent, IShellFolder psfParent, IntPtr pidl) where T : class
		{
			return (T)SHCreateItemWithParent(pidlParent, psfParent, pidl, typeof(T).GUID);
		}
		
		[DllImport("shell32.dll", PreserveSig=false)]
		[return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex = 2)]
		static extern object SHCreateItemFromIDList(IntPtr pidl, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
		
		[DebuggerStepThrough]
		static T SHCreateItemFromIDList<T>(IntPtr pidl) where T : class
		{
			return (T)SHCreateItemFromIDList(pidl, typeof(T).GUID);
		}
		
		[DllImport("shell32.dll", PreserveSig=false)]
		static extern IntPtr SHGetIDListFromObject([MarshalAs(UnmanagedType.IUnknown)] object punk);
		
		
		public FileAttributes GetAttributes(Uri url)
		{
			throw new NotImplementedException();
		}
		
		public DateTime GetCreationTime(Uri url)
		{
			var item = (IShellItem2)GetItem(url);
			try{
				FILETIME ft = item.GetFileTime(PKEY_DateCreated);
				return Win32FileSystem.GetDateTime(ft);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public DateTime GetLastAccessTime(Uri url)
		{
			var item = (IShellItem2)GetItem(url);
			try{
				FILETIME ft = item.GetFileTime(PKEY_DateAccessed);
				return Win32FileSystem.GetDateTime(ft);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public DateTime GetLastWriteTime(Uri url)
		{
			var item = (IShellItem2)GetItem(url);
			try{
				FILETIME ft = item.GetFileTime(PKEY_DateModified);
				return Win32FileSystem.GetDateTime(ft);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		static readonly PROPERTYKEY PKEY_Size = new PROPERTYKEY("B725F130-47EF-101A-A5F1-02608C9EEBAC", 12);
		static readonly PROPERTYKEY PKEY_Title = new PROPERTYKEY("F29F85E0-4FF9-1068-AB91-08002B27B3D9", 2);
		static readonly PROPERTYKEY PKEY_DateModified = new PROPERTYKEY("B725F130-47EF-101A-A5F1-02608C9EEBAC", 14);
		static readonly PROPERTYKEY PKEY_DateCreated = new PROPERTYKEY("B725F130-47EF-101A-A5F1-02608C9EEBAC", 15);
		static readonly PROPERTYKEY PKEY_DateAccessed = new PROPERTYKEY("B725F130-47EF-101A-A5F1-02608C9EEBAC", 16);
		static readonly PROPERTYKEY PKEY_Link_TargetParsingPath = new PROPERTYKEY("B9B4B3FC-2B51-4A42-B5D8-324146AFCF25", 2);
		static readonly PROPERTYKEY PKEY_Link_TargetUrl = new PROPERTYKEY("5CBF2787-48CF-4208-B90E-EE5E5D420294", 2);
		static readonly PROPERTYKEY PKEY_ParsingName = new PROPERTYKEY("28636AA6-953D-11D2-B5D6-00C04FD918D0", 24);
		static readonly PROPERTYKEY PKEY_ParsingPath = new PROPERTYKEY("28636AA6-953D-11D2-B5D6-00C04FD918D0", 30);
		
		public long GetLength(Uri url)
		{
			var item = (IShellItem2)GetItem(url);
			try{
				return (long)item.GetUInt64(PKEY_Size);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		static Guid BHID_Stream = new Guid("1CEBB3AB-7C10-499a-A417-92CA16C4CB83");
		public Stream GetStream(Uri url, FileMode mode, FileAccess access)
		{
			var item = GetItem(url);
			try{
				var stream = item.BindToHandler<IStream>(null, BHID_Stream);
				return new IStreamWrapper(stream);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		static Guid BHID_LinkTargetItem = new Guid("3981E228-F559-11D3-8E3A-00C04F6837D5");
		static Guid BHID_SFUIObject = new Guid("3981E225-F559-11D3-8E3A-00C04F6837D5");
		public Uri GetTarget(Uri url)
		{
			var item = (IShellItem2)GetItem(url);
			try{
				try{
					string linkurl = item.GetString(PKEY_Link_TargetUrl);
					return new Uri(linkurl);
				}catch(COMException e)
				{
					if(unchecked((uint)e.HResult) != 0x80070490) throw;
				}
				
				var attr = item.GetAttributes(SFGAOF.SFGAO_LINK | SFGAOF.SFGAO_FILESYSTEM);
				
				IShellItem target;
				if((attr & SFGAOF.SFGAO_LINK) != 0)
				{
					target = item.BindToHandler<IShellItem>(null, BHID_LinkTargetItem);
				}/*else if((attr & SFGAOF.SFGAO_FILESYSTEM) != 0)
				{
					string path = item.GetString(PKEY_ParsingPath);
					var data = (IPersistFile)new ShellLink();
					try{
						data.Load(path, 0);
						var link = (IShellLinkW)data;
						link.Resolve(IntPtr.Zero, SLR_FLAGS.SLR_UPDATE);
						IntPtr pidl = link.GetIDList();
						try{
							target = SHCreateItemFromIDList<IShellItem>(pidl);
						}finally{
							Marshal.FreeCoTaskMem(pidl);
						}
					}finally{
						Marshal.FinalReleaseComObject(data);
					}
				}*/else{
					/*var parent = item.GetParent();
					var pidl = SHGetIDListFromObject(parent);
					try{
						var folder = SHBindToObject<IShellFolder>(null, pidl, null);
						var rpidl = SHGetIDListFromObject(item);
						try{
							var link = folder.GetUIObjectOf<IShellLink>(IntPtr.Zero, new[]{rpidl});
							IntPtr lpidl = link.GetIDList();
							try{
								target = SHCreateItemFromIDList<IShellItem>(lpidl);
							}finally{
								Marshal.FreeCoTaskMem(lpidl);
							}
						}finally{
							Marshal.FreeCoTaskMem(rpidl);
						}
					}finally{
						Marshal.FreeCoTaskMem(pidl);
					}*/
					try{
						var link = item.BindToHandler<IShellLink>(null, BHID_SFUIObject);
						IntPtr lpidl = link.GetIDList();
						try{
							target = SHCreateItemFromIDList<IShellItem>(lpidl);
						}finally{
							Marshal.FreeCoTaskMem(lpidl);
						}
					}catch(NotImplementedException)
					{
						target = (IShellItem)item;
					}catch(InvalidCastException)
					{
						target = (IShellItem)item;
					}
				}
				try{
					string tpath = target.GetDisplayName(SIGDN.SIGDN_DESKTOPABSOLUTEPARSING);
					var tattr = target.GetAttributes(SFGAOF.SFGAO_FILESYSTEM);
					
					return GetShellUrl(tpath, (tattr & SFGAOF.SFGAO_FILESYSTEM) != 0);
				}finally{
					Marshal.FinalReleaseComObject(target);
				}
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public string GetContentType(Uri url)
		{
			throw new NotImplementedException();
		}
		
	    [ComImport]
		[Guid("00021401-0000-0000-C000-000000000046")]
		private class ShellLink
		{
			
		}
	}
}
