/* Date: 10.9.2017, Time: 12:47 */
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using IllidanS4.SharpUtils.COM;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	partial class Win32FileSystem
	{
		static class Kernel32
		{
			[DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
			public static extern int GetFileAttributes(string lpFileName);
			
			[DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
			static extern bool GetFileAttributesEx(string lpFileName, int fInfoLevelId, out WIN32_FILE_ATTRIBUTE_DATA fileData);
			
			[DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
			static extern int GetFullPathName(string lpFileName, int nBufferLength, StringBuilder lpBuffer, IntPtr lpFilePart=default(IntPtr));
			
			[DebuggerStepThrough]
			public static string GetFullPathName(string lpFileName)
			{
				var buffer = new StringBuilder(0);
				int len = GetFullPathName(lpFileName, 0, buffer);
				if(len == 0) throw new Win32Exception();
				buffer.EnsureCapacity(len);
				len = GetFullPathName(lpFileName, len, buffer);
				if(len == 0) throw new Win32Exception();
				return buffer.ToString();
			}
			
			[StructLayout(LayoutKind.Sequential)]
			public struct WIN32_FILE_ATTRIBUTE_DATA
			{
				public FileAttributes dwFileAttributes;
				public FILETIME ftCreationTime;
				public FILETIME ftLastAccessTime;
				public FILETIME ftLastWriteTime;
				public int nFileSizeHigh;
				public int nFileSizeLow;
			}
			
			[DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true, EntryPoint="CreateFile")]
			static extern IntPtr _CreateFile(string lpFileName, FileAccess dwDesiredAccess, FileShare dwShareMode, IntPtr lpSecurityAttributes, FileMode dwCreationDisposition, FileAttributes dwFlagsAndAttributes, IntPtr templateFile);
			
			
		    private const int FILE_READ_EA = 0x0008;
		    private const int FILE_FLAG_BACKUP_SEMANTICS = 0x2000000;
			
		    [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		    static extern int GetFinalPathNameByHandle(IntPtr hFile, StringBuilder lpszFilePath, int cchFilePath, int dwFlags);
			
		    [DllImport("kernel32.dll", SetLastError=true, EntryPoint="CloseHandle")]
		    static extern bool _CloseHandle(IntPtr hObject);
		    
			[DebuggerStepThrough]
		    public static string GetFinalPathNameByHandle(IntPtr hFile, int dwFlags)
		    {
				var buffer = new StringBuilder(0);
				int len = GetFinalPathNameByHandle(hFile, buffer, 0, dwFlags);
				if(len == 0) throw new Win32Exception();
				buffer.EnsureCapacity(len);
				len = GetFinalPathNameByHandle(hFile, buffer, len, dwFlags);
				if(len == 0) throw new Win32Exception();
				return buffer.ToString();
		    }
			
			[DebuggerStepThrough]
		    public static WIN32_FILE_ATTRIBUTE_DATA GetFileAttributesEx(string lpFileName, int fInfoLevelId)
		    {
				WIN32_FILE_ATTRIBUTE_DATA data;
				if(!GetFileAttributesEx(lpFileName, fInfoLevelId, out data)) throw new Win32Exception();
				return data;
		    }
		    
			[DebuggerStepThrough]
		    public static IntPtr CreateFile(string lpFileName, FileAccess dwDesiredAccess, FileShare dwShareMode, IntPtr lpSecurityAttributes, FileMode dwCreationDisposition, FileAttributes dwFlagsAndAttributes, IntPtr templateFile)
		    {
		    	IntPtr handle = _CreateFile(lpFileName, dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, templateFile);
		    	if(handle == new IntPtr(-1)) throw new Win32Exception();
		    	return handle;
		    }
		    
			[DebuggerStepThrough]
		    public static void CloseHandle(IntPtr hObject)
		    {
		    	if(!_CloseHandle(hObject)) throw new Win32Exception();
		    }
		}
		
		static class Shlwapi
		{
		    [DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
			static extern int UrlCreateFromPath(string pszPath, StringBuilder pszUrl, ref int pcchUrl, int dwFlags);
			
		    [DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
			static extern int PathCreateFromUrl(string pszUrl, StringBuilder pszPath, ref int pcchPath, int dwFlags);
			
			[DebuggerStepThrough]
			public static string UrlCreateFromPath(string pszPath)
			{
				int len = 1;
				var buffer = new StringBuilder(len);
				int result = UrlCreateFromPath(pszPath, buffer, ref len, 0);
				if(len == 1) Marshal.ThrowExceptionForHR(result);
				
				buffer.EnsureCapacity(len);
				result = UrlCreateFromPath(pszPath, buffer, ref len, 0);
				if(result == 1) return null;
				Marshal.ThrowExceptionForHR(result);
				return buffer.ToString();
			}
			
			[DebuggerStepThrough]
			public static string PathCreateFromUrl(string pszUrl)
			{
				int len = 1;
				var buffer = new StringBuilder(len);
				int result = PathCreateFromUrl(pszUrl, buffer, ref len, 0);
				if(len == 1) Marshal.ThrowExceptionForHR(result);
				else if(len == 0) return String.Empty;
				
				buffer.EnsureCapacity(len);
				result = PathCreateFromUrl(pszUrl, buffer, ref len, 0);
				Marshal.ThrowExceptionForHR(result);
				return buffer.ToString();
			}
		}
		
		static class Urlmon
		{
		    [DllImport("urlmon.dll", CharSet=CharSet.Unicode, ExactSpelling=true, PreserveSig=false)]
		    static extern void FindMimeFromData(IBindCtx pBC, string pwzUrl, IntPtr pBuffer, int cbSize, string pwzMimeProposed, int dwMimeFlags, out string ppwzMimeOut, int dwReserved);
		    
			[DebuggerStepThrough]
		    public static string FindMimeFromData(IBindCtx pBC, string pwzUrl, IntPtr pBuffer, int cbSize, string pwzMimeProposed, int dwMimeFlags)
		    {
		    	string mime;
		    	FindMimeFromData(pBC, pwzUrl, pBuffer, cbSize, pwzMimeProposed, dwMimeFlags, out mime, 0);
		    	return mime;
		    }
		}
	}
	
	partial class ShellFileSystem
	{
		static class Shlwapi
		{
			[DllImport("shlwapi.dll", CharSet=CharSet.Auto, PreserveSig=false)]
			public static extern string StrRetToStr(ref STRRET pstr, [Optional]IntPtr pidl);
			
		}
		
		static class Shell32
		{
			public static readonly PROPERTYKEY PKEY_Size = new PROPERTYKEY("B725F130-47EF-101A-A5F1-02608C9EEBAC", 12);
			public static readonly PROPERTYKEY PKEY_Title = new PROPERTYKEY("F29F85E0-4FF9-1068-AB91-08002B27B3D9", 2);
			public static readonly PROPERTYKEY PKEY_DateModified = new PROPERTYKEY("B725F130-47EF-101A-A5F1-02608C9EEBAC", 14);
			public static readonly PROPERTYKEY PKEY_DateCreated = new PROPERTYKEY("B725F130-47EF-101A-A5F1-02608C9EEBAC", 15);
			public static readonly PROPERTYKEY PKEY_DateAccessed = new PROPERTYKEY("B725F130-47EF-101A-A5F1-02608C9EEBAC", 16);
			public static readonly PROPERTYKEY PKEY_Link_TargetParsingPath = new PROPERTYKEY("B9B4B3FC-2B51-4A42-B5D8-324146AFCF25", 2);
			public static readonly PROPERTYKEY PKEY_Link_TargetUrl = new PROPERTYKEY("5CBF2787-48CF-4208-B90E-EE5E5D420294", 2);
			public static readonly PROPERTYKEY PKEY_ParsingName = new PROPERTYKEY("28636AA6-953D-11D2-B5D6-00C04FD918D0", 24);
			public static readonly PROPERTYKEY PKEY_ParsingPath = new PROPERTYKEY("28636AA6-953D-11D2-B5D6-00C04FD918D0", 30);
			
			public static readonly Guid BHID_Stream = new Guid("1CEBB3AB-7C10-499a-A417-92CA16C4CB83");
			public static readonly Guid BHID_LinkTargetItem = new Guid("3981E228-F559-11D3-8E3A-00C04F6837D5");
			public static readonly Guid BHID_SFUIObject = new Guid("3981E225-F559-11D3-8E3A-00C04F6837D5");
			
			[DllImport("shell32.dll", CharSet=CharSet.Unicode, PreserveSig=false)]
			[return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex=2)]
			static extern object SHCreateItemFromParsingName(string pszPath, IBindCtx pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
			
			[DebuggerStepThrough]
			public static T SHCreateItemFromParsingName<T>(string pszPath, IBindCtx pbc) where T : class
			{
				return (T)SHCreateItemFromParsingName(pszPath, pbc, typeof(T).GUID);
			}
			
			[DllImport("shell32.dll", PreserveSig=false)]
			[return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex=3)]
			static extern object SHBindToObject(IShellFolder psf, IntPtr pidl, IBindCtx pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
			
			[DebuggerStepThrough]
			public static T SHBindToObject<T>(IShellFolder psf, IntPtr pidl, IBindCtx pbc) where T : class
			{
				return (T)SHBindToObject(psf, pidl, pbc, typeof(T).GUID);
			}
			
			[DllImport("shell32.dll", CharSet=CharSet.Unicode, PreserveSig=false)]
			static extern void SHParseDisplayName(string pszName, IBindCtx pbc, out IntPtr ppidl, SFGAOF sfgaoIn, out SFGAOF psfgaoOut);
			
			[DebuggerStepThrough]
			public static IntPtr SHParseDisplayName(string pszName, IBindCtx pbc, SFGAOF sfgaoIn, out SFGAOF psfgaoOut)
			{
				IntPtr pidl;
				SHParseDisplayName(pszName, pbc, out pidl, sfgaoIn, out psfgaoOut);
				return pidl;
			}
			
			[DllImport("shell32.dll", PreserveSig=false)]
			[return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex=3)]
			static extern object SHCreateItemWithParent(IntPtr pidlParent, IShellFolder psfParent, IntPtr pidl, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
			
			[DebuggerStepThrough]
			public static T SHCreateItemWithParent<T>(IntPtr pidlParent, IShellFolder psfParent, IntPtr pidl) where T : class
			{
				return (T)SHCreateItemWithParent(pidlParent, psfParent, pidl, typeof(T).GUID);
			}
			
			[DllImport("shell32.dll", PreserveSig=false)]
			[return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex = 2)]
			static extern object SHCreateItemFromIDList(IntPtr pidl, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
			
			[DebuggerStepThrough]
			public static T SHCreateItemFromIDList<T>(IntPtr pidl) where T : class
			{
				return (T)SHCreateItemFromIDList(pidl, typeof(T).GUID);
			}
			
			[DllImport("shell32.dll", PreserveSig=false)]
			static extern IntPtr SHGetIDListFromObject([MarshalAs(UnmanagedType.IUnknown)] object punk);
			
			public static IShellLink CreateShellLink()
			{
				return (IShellLink)new ShellLink();
			}
			
		    [ComImport]
			[Guid("00021401-0000-0000-C000-000000000046")]
			private class ShellLink
			{
				
			}
		}
	}
}
