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
		
		private readonly Uri baseUri;
		
		public ShellFileSystem(Uri baseUri)
		{
			this.baseUri = baseUri;
		}
		
		#region Public API
		public string GetShellPath(Uri uri)
		{
			var rel = baseUri.MakeRelativeUri(uri);
			if(rel.IsAbsoluteUri) throw new ArgumentException("URI is not within this subsystem.", "uri");
			
			string path = rel.OriginalString.Replace('/', Path.DirectorySeparatorChar);
			
			if(path.StartsWith(@".\")) path = path.Substring(2);
			
			return "shell:"+HttpUtility.UrlDecode(path);
		}
		
		public Uri GetShellUri(string path, bool isFileSystem)
		{
			Uri relUri;
			if(isFileSystem || File.Exists(path))
			{
				relUri = new Uri(@"MyComputerFolder\"+path, UriKind.Relative);
			}else if(path.StartsWith("shell:", StringComparison.OrdinalIgnoreCase))
			{
				relUri = new Uri(path.Substring(6), UriKind.Relative);
			}else{
				relUri = new Uri(path, UriKind.Relative);
			}
			return new Uri(baseUri, relUri);
		}
		
		[CLSCompliant(false)]
		public IShellLink LoadLink(byte[] linkData)
		{
			var data = (IPersistStream)new ShellLink();
			using(var buffer = new MemoryStream(linkData))
			{
				var stream = new StreamWrapper(buffer);
				data.Load(stream);
			}
			var link = (IShellLink)data;
			link.Resolve(IntPtr.Zero, SLR_FLAGS.SLR_UPDATE);
			return link;
		}
		
		[CLSCompliant(false)]
		public IShellLink LoadLink(string linkFile)
		{
			var data = (IPersistFile)new ShellLink();
			data.Load(linkFile, 0);
			var link = (IShellLink)data;
			link.Resolve(IntPtr.Zero, SLR_FLAGS.SLR_UPDATE);
			return link;
		}
		
		[CLSCompliant(false)]
		public Uri GetLinkTargetUri(IShellLink link)
		{
			var target = GetLinkTarget(link);
			try{
				string path = target.GetDisplayName(SIGDN.SIGDN_DESKTOPABSOLUTEPARSING);
				var attr = target.GetAttributes(SFGAOF.SFGAO_FILESYSTEM);
				
				return GetShellUri(path, (attr & SFGAOF.SFGAO_FILESYSTEM) != 0);
			}finally{
				Marshal.FinalReleaseComObject(target);
			}
		}
		
		public Uri LoadLinkTargetUri(byte[] linkData)
		{
			var link = LoadLink(linkData);
			try{
				return GetLinkTargetUri(link);
			}finally{
				Marshal.FinalReleaseComObject(link);
			}
		}
		
		public Uri LoadLinkTargetUri(string linkFile)
		{
			var link = LoadLink(linkFile);
			try{
				return GetLinkTargetUri(link);
			}finally{
				Marshal.FinalReleaseComObject(link);
			}
		}
		
		[CLSCompliant(false)]
		public IShellItem GetLinkTarget(IShellLink link)
		{
			link.Resolve(IntPtr.Zero, SLR_FLAGS.SLR_UPDATE);
			IntPtr pidl = link.GetIDList();
			try{
				return SHCreateItemFromIDList<IShellItem>(pidl);
			}finally{
				Marshal.FreeCoTaskMem(pidl);
			}
		}
		#endregion
		
		static readonly Regex pathNameRegex = new Regex(@"^(shell:.*?\\?)([^\\]*)$", RegexOptions.Compiled);
		private IShellItem GetItem(Uri uri)
		{
			string path = GetShellPath(uri);
			return GetItem(path);
		}
		
		private IShellItem GetItem(string path)
		{
			try{
				return SHCreateItemFromParsingName<IShellItem>(path, null);
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
					}catch(IOException)
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
		
		#region Implementation
		public FileAttributes GetAttributes(Uri uri)
		{
			throw new NotImplementedException();
		}
		
		public DateTime GetCreationTime(Uri uri)
		{
			var item = (IShellItem2)GetItem(uri);
			try{
				FILETIME ft = item.GetFileTime(PKEY_DateCreated);
				return Win32FileSystem.GetDateTime(ft);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public DateTime GetLastAccessTime(Uri uri)
		{
			var item = (IShellItem2)GetItem(uri);
			try{
				FILETIME ft = item.GetFileTime(PKEY_DateAccessed);
				return Win32FileSystem.GetDateTime(ft);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public DateTime GetLastWriteTime(Uri uri)
		{
			var item = (IShellItem2)GetItem(uri);
			try{
				FILETIME ft = item.GetFileTime(PKEY_DateModified);
				return Win32FileSystem.GetDateTime(ft);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public long GetLength(Uri uri)
		{
			var item = (IShellItem2)GetItem(uri);
			try{
				return (long)item.GetUInt64(PKEY_Size);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public Stream GetStream(Uri uri, FileMode mode, FileAccess access)
		{
			var item = GetItem(uri);
			try{
				var stream = item.BindToHandler<IStream>(null, BHID_Stream);
				return new IStreamWrapper(stream);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public Uri GetTarget(Uri uri)
		{
			var item = (IShellItem2)GetItem(uri);
			try{
				try{
					string linkuri = item.GetString(PKEY_Link_TargetUrl);
					return new Uri(linkuri);
				}catch(COMException e)
				{
					if(unchecked((uint)e.HResult) != 0x80070490) throw;
				}
				
				var attr = item.GetAttributes(SFGAOF.SFGAO_LINK | SFGAOF.SFGAO_FILESYSTEM);
				
				IShellItem target;
				if((attr & SFGAOF.SFGAO_LINK) != 0)
				{
					target = item.BindToHandler<IShellItem>(null, BHID_LinkTargetItem);
				}else{
					try{
						var link = item.BindToHandler<IShellLink>(null, BHID_SFUIObject);
						target = GetLinkTarget(link);
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
					
					return GetShellUri(tpath, (tattr & SFGAOF.SFGAO_FILESYSTEM) != 0);
				}finally{
					Marshal.FinalReleaseComObject(target);
				}
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public string GetContentType(Uri uri)
		{
			throw new NotImplementedException();
		}
		#endregion
		
		#region COM interop
		static readonly PROPERTYKEY PKEY_Size = new PROPERTYKEY("B725F130-47EF-101A-A5F1-02608C9EEBAC", 12);
		static readonly PROPERTYKEY PKEY_Title = new PROPERTYKEY("F29F85E0-4FF9-1068-AB91-08002B27B3D9", 2);
		static readonly PROPERTYKEY PKEY_DateModified = new PROPERTYKEY("B725F130-47EF-101A-A5F1-02608C9EEBAC", 14);
		static readonly PROPERTYKEY PKEY_DateCreated = new PROPERTYKEY("B725F130-47EF-101A-A5F1-02608C9EEBAC", 15);
		static readonly PROPERTYKEY PKEY_DateAccessed = new PROPERTYKEY("B725F130-47EF-101A-A5F1-02608C9EEBAC", 16);
		static readonly PROPERTYKEY PKEY_Link_TargetParsingPath = new PROPERTYKEY("B9B4B3FC-2B51-4A42-B5D8-324146AFCF25", 2);
		static readonly PROPERTYKEY PKEY_Link_TargetUrl = new PROPERTYKEY("5CBF2787-48CF-4208-B90E-EE5E5D420294", 2);
		static readonly PROPERTYKEY PKEY_ParsingName = new PROPERTYKEY("28636AA6-953D-11D2-B5D6-00C04FD918D0", 24);
		static readonly PROPERTYKEY PKEY_ParsingPath = new PROPERTYKEY("28636AA6-953D-11D2-B5D6-00C04FD918D0", 30);
		
		static Guid BHID_Stream = new Guid("1CEBB3AB-7C10-499a-A417-92CA16C4CB83");
		static Guid BHID_LinkTargetItem = new Guid("3981E228-F559-11D3-8E3A-00C04F6837D5");
		static Guid BHID_SFUIObject = new Guid("3981E225-F559-11D3-8E3A-00C04F6837D5");
		
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
		
		
	    [ComImport]
		[Guid("00021401-0000-0000-C000-000000000046")]
		private class ShellLink
		{
			
		}
		#endregion
	}
}
