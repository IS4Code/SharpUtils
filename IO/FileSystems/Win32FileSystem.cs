/* Date: 3.9.2017, Time: 4:04 */
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	public class Win32FileSystem : MountFileSystem, IFileSystem
	{
		public static readonly Win32FileSystem Instance = new Win32FileSystem();
		
		public Win32FileSystem()
		{
			var shellUri = new Uri("file:///shell/");
			Mount(shellUri, new ShellFileSystem(shellUri));
		}
		
		public static Uri UrlOrPath(string path)
		{
			Uri uri;
			if(Uri.TryCreate(path, UriKind.Absolute, out uri) && (uri.Scheme != Uri.UriSchemeFile || path.StartsWith(Uri.UriSchemeFile+Uri.SchemeDelimiter, StringComparison.Ordinal)))
			{
				return uri;
			}else{
				return FileUrlFromPath(path);
			}
		}
		
		public static Uri FileUrlFromPath(string path)
		{
			if(!extendedRegex.IsMatch(path))
			{
				path = GetFullPath(path);
			}
			RemoveBackslash(ref path);
			return FileUrlFromPathInternal(path);
		}
		
		protected override FileAttributes GetAttributesInternal(Uri url)
		{
			int attr = GetFileAttributes(GetPath(url));
			if(attr == -1) throw new Win32Exception();
			return (FileAttributes)attr;
		}
		
		protected override DateTime GetCreationTimeInternal(Uri url)
		{
			return GetDateTime(GetAttributeData(GetPath(url)).ftCreationTime);
		}
		
		protected override DateTime GetLastAccessTimeInternal(Uri url)
		{
			return GetDateTime(GetAttributeData(GetPath(url)).ftLastAccessTime);
		}
		
		protected override DateTime GetLastWriteTimeInternal(Uri url)
		{
			return GetDateTime(GetAttributeData(GetPath(url)).ftLastWriteTime);
		}
		
		protected override long GetLengthInternal(Uri url)
		{
			var data = GetAttributeData(GetPath(url));
			unchecked{
				return ((long)((ulong)(uint)data.nFileSizeHigh << 32) | (long)(uint)data.nFileSizeLow);
			}
		}
		
		protected override Stream GetStreamInternal(Uri url, FileMode mode, FileAccess access)
		{
			return new DeviceStream(GetPath(url), mode, access);
		}
		
		protected override Uri GetTargetInternal(Uri url)
		{
			IntPtr handle = CreateFile(GetPath(url), (FileAccess)8, FileShare.ReadWrite | FileShare.Delete, IntPtr.Zero, FileMode.Open, (FileAttributes)0x2000000, IntPtr.Zero);
			if(handle == new IntPtr(-1)) throw new Win32Exception();
			try{
				var buffer = new StringBuilder(0);
				int len = GetFinalPathNameByHandle(handle, buffer, 0, 0);
				if(len == 0) throw new Win32Exception();
				buffer.EnsureCapacity(len);
				len = GetFinalPathNameByHandle(handle, buffer, len, 0);
				if(len == 0) throw new Win32Exception();
				return FileUrlFromPath(buffer.ToString());
			}finally{
				CloseHandle(handle);
			}
		}
		
		protected override string GetContentTypeInternal(Uri url)
		{
			string mime;
			FindMimeFromData(IntPtr.Zero, GetPath(url), IntPtr.Zero, 0, null, 0x23, out mime, 0);
			if(mime == "application/x-msdownload") return "application/octet-stream";
			return mime;
		}
		
		private static string GetPath(Uri url)
		{
			string path = PathFromFileUrl(url);
			if(String.IsNullOrEmpty(url.Host))
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
		
		private static WIN32_FILE_ATTRIBUTE_DATA GetAttributeData(string path)
		{
			WIN32_FILE_ATTRIBUTE_DATA data;
			if(!GetFileAttributesEx(path, 0, out data)) throw new Win32Exception();
			return data;
		}
	
		public static DateTime GetDateTime(FILETIME ft)
		{
			unchecked{
				var fileTime = ((long)((ulong)(uint)ft.dwHighDateTime << 32) | (long)(uint)ft.dwLowDateTime);
				return DateTime.FromFileTimeUtc(fileTime);
			}
		}
		
		public static string GetFullPath(string path)
		{
			var buffer = new StringBuilder(0);
			int len = GetFullPathName(path, 0, buffer, IntPtr.Zero);
			if(len == 0) throw new Win32Exception();
			buffer.EnsureCapacity(len);
			len = GetFullPathName(path, len, buffer, IntPtr.Zero);
			if(len == 0) throw new Win32Exception();
			return buffer.ToString();
		}
		
		private static readonly Regex extendedRegex = new Regex(@"^\\[\?\\](\?\\UNC|\?)(?:\\|$)");
		private static readonly Regex virtualRegex = new Regex(@"^::");
		private static readonly Regex prefixRegex = new Regex(@"^\\\\(\.|localhost)(\\.*|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static readonly Regex filePrefixRegex = new Regex(@"^file:///");
		
		private static Uri FileUrlFromPathInternal(string path)
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
			
			int len = 1;
			var buffer = new StringBuilder(len);
			int result = UrlCreateFromPath(path, buffer, ref len, 0);
			if(len == 1) Marshal.ThrowExceptionForHR(result);
			
			buffer.EnsureCapacity(len);
			result = UrlCreateFromPath(path, buffer, ref len, 0);
			if(result == 1) throw new ArgumentException("Argument is not a valid path.", "path");
			Marshal.ThrowExceptionForHR(result);
			
			string url = buffer.ToString();
			if(share != null && share.Equals("localhost", StringComparison.OrdinalIgnoreCase))
			{
				url = filePrefixRegex.Replace(url, "file://"+share+"/");
			}
			return new Uri(url);
		}
		
		public static string PathFromFileUrl(Uri uri)
		{
			string url = uri.AbsoluteUri;
			
			int len = 1;
			var buffer = new StringBuilder(len);
			int result = PathCreateFromUrl(url, buffer, ref len, 0);
			if(len == 1) Marshal.ThrowExceptionForHR(result);
			else if(len == 0) return String.Empty;
			
			buffer.EnsureCapacity(len);
			result = PathCreateFromUrl(url, buffer, ref len, 0);
			Marshal.ThrowExceptionForHR(result);
			
			string path = buffer.ToString();
			if(uri.Host == "localhost")
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
		
		[DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		static extern int GetFileAttributes(string lpFileName);
		
		[DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		static extern bool GetFileAttributesEx(string lpFileName, int fInfoLevelId, out WIN32_FILE_ATTRIBUTE_DATA fileData);
		
		[DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		static extern int GetFullPathName(string lpFileName, int nBufferLength, StringBuilder lpBuffer, IntPtr lpFilePart);
		
	    [DllImport("shlwapi.dll", CharSet=CharSet.Auto, SetLastError=true)]
		static extern int UrlCreateFromPath(string pszPath, StringBuilder pszUrl, ref int pcchUrl, int dwFlags);
		
	    [DllImport("shlwapi.dll", CharSet=CharSet.Auto, SetLastError=true)]
		static extern int PathCreateFromUrl(string pszUrl, StringBuilder pszPath, ref int pcchPath, int dwFlags);
		
	    [DllImport("urlmon.dll", CharSet=CharSet.Unicode, ExactSpelling=true, PreserveSig=false)]
	    static extern void FindMimeFromData(IntPtr pBC, string pwzUrl, IntPtr pBuffer, int cbSize, string pwzMimeProposed, int dwMimeFlags, out string ppwzMimeOut, int dwReserved);
		
		[StructLayout(LayoutKind.Sequential)]
		struct WIN32_FILE_ATTRIBUTE_DATA
		{
			public FileAttributes dwFileAttributes;
			public FILETIME ftCreationTime;
			public FILETIME ftLastAccessTime;
			public FILETIME ftLastWriteTime;
			public int nFileSizeHigh;
			public int nFileSizeLow;
		}
		
		[DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		static extern IntPtr CreateFile(
			string filename,
			FileAccess access,
			FileShare share,
			IntPtr securityAttributes,
			FileMode creationDisposition,
			FileAttributes flagsAndAttributes,
			IntPtr templateFile
		);
		

	    private const int FILE_READ_EA = 0x0008;
	    private const int FILE_FLAG_BACKUP_SEMANTICS = 0x2000000;
	
	    [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
	    static extern int GetFinalPathNameByHandle(IntPtr hFile, StringBuilder lpszFilePath, int cchFilePath, int dwFlags);
		
	    [DllImport("kernel32.dll", SetLastError=true)]
	    static extern bool CloseHandle(IntPtr hObject);
	}
}
