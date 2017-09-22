/* Date: 3.9.2017, Time: 4:04 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	/// <summary>
	/// This file system contains files in the Win32 path scheme, also
	/// supporting NTFS file system extensions and UNC paths.
	/// </summary>
	public partial class Win32FileSystem : MountFileSystem, IHandleProvider
	{
		public static readonly Win32FileSystem Instance = new Win32FileSystem();
		
		public Win32FileSystem()
		{
			var shellUri = new Uri("file:///shell/");
			Mount(shellUri, new ShellFileSystem(shellUri));
		}
		
		#region Public API
		public Uri UriOrPath(string path)
		{
			Uri uri;
			if(Uri.TryCreate(path, UriKind.Absolute, out uri) && (uri.Scheme != Uri.UriSchemeFile || path.StartsWith(Uri.UriSchemeFile+Uri.SchemeDelimiter, StringComparison.Ordinal)))
			{
				return uri;
			}else{
				return FileUriFromPath(path);
			}
		}
		
		public Uri FileUriFromPath(string path)
		{
			if(!extendedRegex.IsMatch(path))
			{
				path = Kernel32.GetFullPathName(path);
			}
			RemoveBackslash(ref path);
			return FileUriFromPathInternal(path);
		}
	
		public static DateTime GetDateTime(FILETIME ft)
		{
			unchecked{
				var fileTime = ((long)((ulong)(uint)ft.dwHighDateTime << 32) | (long)(uint)ft.dwLowDateTime);
				return DateTime.FromFileTimeUtc(fileTime);
			}
		}
		
		public string PathFromFileUri(Uri uri)
		{
			var bareUri = new UriBuilder(uri){Fragment = null}.Uri;
			
			string absUri = bareUri.AbsoluteUri;
			
			string path = Shlwapi.PathCreateFromUrl(absUri);
			if(bareUri.Host == "localhost")
			{
				return @"\\localhost"+path;
			}else{
				return path;
			}
		}
		
		private static readonly Regex backslashRegex = new Regex(@"([^:\\])\\$", RegexOptions.Compiled);
		public static void RemoveBackslash(ref string path)
		{
			path = backslashRegex.Replace(path, "$1");
		}
		#endregion
		
		private string GetPath(Uri uri)
		{
			var segments = uri.AbsolutePath.Split('/').SkipWhile(String.IsNullOrEmpty).Select(s => HttpUtility.UrlDecode(s)).ToList();
			
			string prefix = String.IsNullOrEmpty(uri.Host) ? @"\\?\" : @"\\?\UNC\"+uri.Host+@"\";
			
			if(segments.Any(s => s.Contains(@"\"))) throw new ArgumentException("The URI contains invalid characters.", "uri");
			
			return prefix+String.Join(@"\", segments);
		}
		
		private static readonly Regex extendedRegex = new Regex(@"^\\[\?\\](\?\\UNC|\?)(?:\\|$)");
		private static readonly Regex virtualRegex = new Regex(@"^::");
		private static readonly Regex prefixRegex = new Regex(@"^\\\\(\.|localhost)(\\.*|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static readonly Regex filePrefixRegex = new Regex(@"^file:///");
		
		private Uri FileUriFromPathInternal(string path)
		{
			const string device = @"\\";
			
			path = extendedRegex.Replace(path, device);
			
			var match = prefixRegex.Match(path);
			string share;
			if(match.Success)
			{
				share = match.Groups[1].Value;
				if(!share.Equals("localhost", StringComparison.OrdinalIgnoreCase))
				{
					string local = match.Groups[2].Value;
					path = device+local;
				}
			}else{
				share = null;
			}
			
			string uri = Shlwapi.UrlCreateFromPath(path);
			if(uri == null) throw new ArgumentException("Argument is not a valid path.", "path");
			if(share != null && share.Equals("localhost", StringComparison.OrdinalIgnoreCase))
			{
				uri = filePrefixRegex.Replace(uri, "file://"+share+"/");
			}
			return new Uri(uri);
		}
		
		private Kernel32.WIN32_FILE_ATTRIBUTE_DATA GetAttributeData(string path)
		{
			return Kernel32.GetFileAttributesEx(path, 0);
		}
		
		#region Implementation
		public ResourceHandle ObtainHandle(Uri uri)
		{
			IntPtr handle = Kernel32.CreateFile(GetPath(uri), (FileAccess)8, FileShare.ReadWrite | FileShare.Delete, IntPtr.Zero, FileMode.Open, (FileAttributes)0x2000000, IntPtr.Zero);
			try{
				return new Win32FileHandle(uri, handle, this);
			}finally{
				Kernel32.CloseHandle(handle);
			}
		}
		
		protected override T GetPropertyInternal<T>(Uri uri, ResourceProperty property)
		{
			string path = GetPath(uri);
			switch(property)
			{
				case ResourceProperty.FileAttributes:
					return To<T>.Cast((FileAttributes)Kernel32.GetFileAttributes(path));
				case ResourceProperty.CreationTimeUtc:
					return To<T>.Cast(GetDateTime(GetAttributeData(path).ftCreationTime));
				case ResourceProperty.LastAccessTimeUtc:
					return To<T>.Cast(GetDateTime(GetAttributeData(path).ftLastAccessTime));
				case ResourceProperty.LastWriteTimeUtc:
					return To<T>.Cast(GetDateTime(GetAttributeData(path).ftLastWriteTime));
				case ResourceProperty.LongLength:
					return To<T>.Cast(GetLengthInternal(path));
				case ResourceProperty.TargetUri:
					return To<T>.Cast(GetTargetInternal(path));
				case ResourceProperty.ContentType:
					return To<T>.Cast(GetContentTypeInternal(path));
				case ResourceProperty.LocalPath:
					return To<T>.Cast(path);
				case ResourceProperty.DisplayPath:
					return To<T>.Cast(PathFromFileUri(uri));
				default:
					throw new NotImplementedException();
			}
		}
		
		protected override void SetPropertyInternal<T>(Uri uri, ResourceProperty property, T value)
		{
			string path = GetPath(uri);
			switch(property)
			{
				case ResourceProperty.FileAttributes:
					Kernel32.SetFileAttributes(path, (int)To<FileAttributes>.Cast(value));
					break;
				default:
					throw new NotImplementedException();
			}
		}
		
		private long GetLengthInternal(string path)
		{
			var data = GetAttributeData(path);
			unchecked{
				return ((long)((ulong)(uint)data.nFileSizeHigh << 32) | (long)(uint)data.nFileSizeLow);
			}
		}
		
		protected override Stream GetStreamInternal(Uri uri, FileMode mode, FileAccess access)
		{
			string path = GetPath(uri);
			
			long offset;
			if(!String.IsNullOrEmpty(uri.Fragment) && Int64.TryParse(HttpUtility.UrlDecode(uri.Fragment.Substring(1)), out offset))
			{
				DeviceStream stream;
				switch(mode)
				{
					case FileMode.Create:
						stream = new DeviceStream(path, FileMode.OpenOrCreate, access);
						stream.SetLength(offset);
						break;
					case FileMode.Truncate:
						stream = new DeviceStream(path, FileMode.Open, access);
						stream.SetLength(offset);
						break;
					default:
						stream = new DeviceStream(path, mode, access);
						break;
				}
				stream.Seek(offset, SeekOrigin.Begin);
				return stream;
			}else{
				return new DeviceStream(path, mode, access);
			}
		}
		
		private Uri GetTargetInternal(string fpath)
		{
			IntPtr handle = Kernel32.CreateFile(fpath, (FileAccess)8, FileShare.ReadWrite | FileShare.Delete, IntPtr.Zero, FileMode.Open, (FileAttributes)0x2000000, IntPtr.Zero);
			try{
				string path = Kernel32.GetFinalPathNameByHandle(handle, 0);
				if(fpath == path) return null;
				return FileUriFromPath(path);
			}finally{
				Kernel32.CloseHandle(handle);
			}
		}
		
		private string GetContentTypeInternal(string path)
		{
			string mime = Urlmon.FindMimeFromData(null, path, IntPtr.Zero, 0, null, 0x23);
			if(mime == "application/x-msdownload") return "application/octet-stream";
			return mime;
		}
		
		private static readonly Regex backslashRegex2 = new Regex(@"[^\\]$", RegexOptions.Compiled);
		protected override List<Uri> GetResourcesInternal(Uri uri)
		{
			string path = GetPath(uri);
			
			string catPath = backslashRegex2.Replace(path, @"$0\");
			string searchPath = catPath+"*";
			
			var list = new List<Uri>();
			foreach(var find in Kernel32.FindFiles(searchPath))
			{
				string name = find.cFileName;
				if(!String.IsNullOrEmpty(name) && name != "." && name != "..")
				{
					list.Add(FileUriFromPath(catPath+name));
				}
			}
			
			IntPtr handle = Kernel32.CreateFile(path, (FileAccess)(1 | 8 | 32), FileShare.ReadWrite | FileShare.Delete, IntPtr.Zero, FileMode.Open, (FileAttributes)0x2000000, IntPtr.Zero);
			try{
				IntPtr buffer = Marshal.AllocHGlobal(32768);
				try{
					Kernel32.FILE_STREAM_INFO streamInfo;
					bool result = Kernel32.GetFileInformationByHandleEx(handle, Kernel32.FILE_INFO_BY_HANDLE_CLASS.FileStreamInfo, buffer, 32768);
					if(!result)
					{
						int error = Marshal.GetLastWin32Error();
						if(error != 38 && error != 87) throw new Win32Exception(error);
					}else{
						int offset = 0;
						do{
							streamInfo = Marshal.PtrToStructure<Kernel32.FILE_STREAM_INFO>(buffer+offset);
							list.Add(FileUriFromPath(catPath+streamInfo.StreamName));
							offset = streamInfo.NextEntryOffset;
						}while(offset != 0);
					}
				}finally{
					Marshal.FreeHGlobal(buffer);
				}
			}finally{
				Kernel32.CloseHandle(handle);
			}
			
			return list;
		}
		
		protected override ResourceHandle PerformOperationInternal(Uri uri, ResourceOperation operation, object arg)
		{
			Uri target;
			string tpath, spath;
			switch(operation)
			{
				case ResourceOperation.Create:
					spath = GetPath(uri);
					IntPtr handle = Kernel32.CreateFile(spath, FileAccess.ReadWrite, FileShare.None, IntPtr.Zero, FileMode.CreateNew, (FileAttributes)arg, IntPtr.Zero);
					try{
						return new Win32FileHandle(uri, handle, this);
					}finally{
						Kernel32.CloseHandle(handle);
					}
				case ResourceOperation.Delete:
					spath = GetPath(uri);
					Kernel32.DeleteFile(spath);
					break;
				case ResourceOperation.Move:
					string name = arg as string;
					if(name != null)
					{
						target = new Uri(uri, name);
					}else{
						target = (Uri)arg;
					}
					spath = GetPath(uri);
					tpath = GetPath(target);
					Kernel32.MoveFileEx(spath, tpath, 0x2 | 0x8);
					break;
				case ResourceOperation.Copy:
					target = (Uri)arg;
					spath = GetPath(uri);
					tpath = GetPath(target);
					Kernel32.CopyFileEx(spath, tpath, null, 0x00000800 | 0x00000001, true);
					break;
				default:
					throw new NotImplementedException();
			}
			return null;
		}
		
		protected override async Task<ResourceHandle> PerformOperationAsyncInternal(Uri uri, ResourceOperation operation, object arg, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			
			Uri target;
			string tpath, spath;
			switch(operation)
			{
				case ResourceOperation.Create:
					spath = GetPath(uri);
					cancellationToken.ThrowIfCancellationRequested();
					IntPtr handle = Kernel32.CreateFile(spath, FileAccess.ReadWrite, FileShare.None, IntPtr.Zero, FileMode.CreateNew, (FileAttributes)arg, IntPtr.Zero);
					try{
						return new Win32FileHandle(uri, handle, this);
					}finally{
						Kernel32.CloseHandle(handle);
					}
				case ResourceOperation.Delete:
					spath = GetPath(uri);
					cancellationToken.ThrowIfCancellationRequested();
					Kernel32.DeleteFile(spath);
					break;
				case ResourceOperation.Move:
					string name = arg as string;
					if(name != null)
					{
						target = new Uri(uri, name);
					}else{
						target = (Uri)arg;
					}
					spath = GetPath(uri);
					tpath = GetPath(target);
					await MoveFile(spath, tpath, cancellationToken);
					break;
				case ResourceOperation.Copy:
					target = (Uri)arg;
					spath = GetPath(uri);
					tpath = GetPath(target);
					await CopyFile(spath, tpath, cancellationToken);
					break;
				default:
					throw new NotImplementedException();
			}
			return null;
		}
		#endregion
		
		private async Task MoveFile(string source, string target, CancellationToken token)
		{
			Kernel32.CopyProgressRoutine progress = delegate{
				return token.IsCancellationRequested ? 1 : 0;
			};
			token.ThrowIfCancellationRequested();
			await Task.Run(
				()=>{
					bool ok = Kernel32.MoveFileWithProgress(source, target, progress, 0x2 | 0x8, false);
					if(!ok) token.ThrowIfCancellationRequested();
				}, token
			);
		}
		
		private async Task CopyFile(string source, string target, CancellationToken token)
		{
			Kernel32.CopyProgressRoutine progress = delegate{
				return token.IsCancellationRequested ? 1 : 0;
			};
			token.ThrowIfCancellationRequested();
			await Task.Run(
				()=>{
					bool ok = Kernel32.CopyFileEx(source, target, progress, 0x00000800 | 0x00000001, false);
					if(!ok) token.ThrowIfCancellationRequested();
				}, token
			);
		}
	}
}
