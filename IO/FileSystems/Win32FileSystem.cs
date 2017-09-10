/* Date: 3.9.2017, Time: 4:04 */
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	/// <summary>
	/// This file system contains files in the Win32 path scheme, also
	/// supporting NTFS file system extensions and UNC paths.
	/// </summary>
	public partial class Win32FileSystem : MountFileSystem, IFileSystem
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
			string path = PathFromFileUri(uri);
			if(String.IsNullOrEmpty(uri.Host))
			{
				if(path.Length > 256)
				{
					return @"\\?\"+path;
				}else{
					return @"\\.\"+path;
				}
			}else{
				if(path.Length > 260)
				{
					return @"\\?\UNC\"+path.Substring(2);
				}else{
					return path;
				}
			}
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
		protected override FileAttributes GetAttributesInternal(Uri uri)
		{
			int attr = Kernel32.GetFileAttributes(GetPath(uri));
			if(attr == -1) throw new Win32Exception();
			return (FileAttributes)attr;
		}
		
		protected override DateTime GetCreationTimeInternal(Uri uri)
		{
			return GetDateTime(GetAttributeData(GetPath(uri)).ftCreationTime);
		}
		
		protected override DateTime GetLastAccessTimeInternal(Uri uri)
		{
			return GetDateTime(GetAttributeData(GetPath(uri)).ftLastAccessTime);
		}
		
		protected override DateTime GetLastWriteTimeInternal(Uri uri)
		{
			return GetDateTime(GetAttributeData(GetPath(uri)).ftLastWriteTime);
		}
		
		protected override long GetLengthInternal(Uri uri)
		{
			var data = GetAttributeData(GetPath(uri));
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
		
		protected override Uri GetTargetInternal(Uri uri)
		{
			IntPtr handle = Kernel32.CreateFile(GetPath(uri), (FileAccess)8, FileShare.ReadWrite | FileShare.Delete, IntPtr.Zero, FileMode.Open, (FileAttributes)0x2000000, IntPtr.Zero);
			try{
				string path = Kernel32.GetFinalPathNameByHandle(handle, 0);
				return FileUriFromPath(path);
			}finally{
				Kernel32.CloseHandle(handle);
			}
		}
		
		protected override string GetContentTypeInternal(Uri uri)
		{
			string mime = Urlmon.FindMimeFromData(null, GetPath(uri), IntPtr.Zero, 0, null, 0x23);
			if(mime == "application/x-msdownload") return "application/octet-stream";
			return mime;
		}
		#endregion
	}
}
