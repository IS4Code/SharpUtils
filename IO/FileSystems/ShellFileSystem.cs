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
	public partial class ShellFileSystem : IFileSystem
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
			var data = (IPersistStream)Shell32.CreateShellLink();
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
			var data = (IPersistFile)Shell32.CreateShellLink();
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
				return Shell32.SHCreateItemFromIDList<IShellItem>(pidl);
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
				return Shell32.SHCreateItemFromParsingName<IShellItem>(path, null);
			}catch(IOException)
			{
				var match = pathNameRegex.Match(path);
				if(!match.Success) throw new ArgumentException("Argument is not a valid path.", "path");
				
				string dir = match.Groups[1].Value;
				if(dir.Equals("shell:", StringComparison.OrdinalIgnoreCase)) dir = "";
				
				SFGAOF sfgao;
				IntPtr pidl = Shell32.SHParseDisplayName(dir, null, 0, out sfgao);
				try{
					path = match.Groups[2].Value;
					var psf = Shell32.SHBindToObject<IShellFolder>(null, pidl, null);
					
					try{
						uint tmp;
						IntPtr pidl2;
						psf.ParseDisplayName(IntPtr.Zero, null, path, out tmp, out pidl2, 0);
						try{
							return Shell32.SHCreateItemWithParent<IShellItem>(pidl, psf, pidl2);
						}finally{
							Marshal.FreeCoTaskMem(pidl2);
						}
					}catch(ArgumentException)
					{
						if(!String.IsNullOrWhiteSpace(path)) throw;
						return Shell32.SHCreateItemFromIDList<IShellItem>(pidl);
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
									
									string name = Shlwapi.StrRetToStr(ref sr, pidl2);
									if(pathNameRegex.Replace(name, "$2") == path)
									{
										return Shell32.SHCreateItemWithParent<IShellItem>(pidl, psf, pidl2);
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
				FILETIME ft = item.GetFileTime(Shell32.PKEY_DateCreated);
				return Win32FileSystem.GetDateTime(ft);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public DateTime GetLastAccessTime(Uri uri)
		{
			var item = (IShellItem2)GetItem(uri);
			try{
				FILETIME ft = item.GetFileTime(Shell32.PKEY_DateAccessed);
				return Win32FileSystem.GetDateTime(ft);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public DateTime GetLastWriteTime(Uri uri)
		{
			var item = (IShellItem2)GetItem(uri);
			try{
				FILETIME ft = item.GetFileTime(Shell32.PKEY_DateModified);
				return Win32FileSystem.GetDateTime(ft);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public long GetLength(Uri uri)
		{
			var item = (IShellItem2)GetItem(uri);
			try{
				return (long)item.GetUInt64(Shell32.PKEY_Size);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public Stream GetStream(Uri uri, FileMode mode, FileAccess access)
		{
			var item = GetItem(uri);
			try{
				var stream = item.BindToHandler<IStream>(null, Shell32.BHID_Stream);
				return new IStreamWrapper(stream);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public Uri GetTarget(Uri uri)
		{
			var item = GetItem(uri);
			try{
				bool isUri, isFs;
				string path = GetTargetPath(item, out isUri, out isFs);
				if(isUri)
				{
					return new Uri(path);
				}else{
					return GetShellUri(path, isFs);
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
		
		private string GetTargetPath(IShellItem shellItem, out bool isUri, out bool isFs)
		{
			var item = (IShellItem2)shellItem;
			try{
				string linkuri = item.GetString(Shell32.PKEY_Link_TargetUrl);
				isUri = true;
				isFs = false;
				return linkuri;
			}catch(COMException e)
			{
				if(unchecked((uint)e.HResult) != 0x80070490) throw;
			}
			
			var attr = item.GetAttributes(SFGAOF.SFGAO_LINK | SFGAOF.SFGAO_FILESYSTEM);
			
			IShellItem target;
			if((attr & SFGAOF.SFGAO_LINK) != 0)
			{
				target = item.BindToHandler<IShellItem>(null, Shell32.BHID_LinkTargetItem);
			}else{
				try{
					var link = item.BindToHandler<IShellLink>(null, Shell32.BHID_SFUIObject);
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
				
				isUri = false;
				isFs = (tattr & SFGAOF.SFGAO_FILESYSTEM) != 0;
				return tpath;
			}finally{
				Marshal.FinalReleaseComObject(target);
			}
		}
	}
}
