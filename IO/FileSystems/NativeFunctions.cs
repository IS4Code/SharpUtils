/* Date: 10.9.2017, Time: 12:47 */
using System;
using System.Collections.Generic;
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
		static class Ntdll
		{
			public enum FILE_INFORMATION_CLASS
			{
				FileDirectoryInformation = 1,
				FileFullDirectoryInformation,
				FileBothDirectoryInformation,
				FileBasicInformation,
				FileStandardInformation,
				FileInternalInformation,
				FileEaInformation,
				FileAccessInformation,
				FileNameInformation,
				FileRenameInformation,
				FileLinkInformation,
				FileNamesInformation,
				FileDispositionInformation,
				FilePositionInformation,
				FileFullEaInformation,
				FileModeInformation,
				FileAlignmentInformation,
				FileAllInformation,
				FileAllocationInformation,
				FileEndOfFileInformation,
				FileAlternateNameInformation,
				FileStreamInformation,
				FilePipeInformation,
				FilePipeLocalInformation,
				FilePipeRemoteInformation,
				FileMailslotQueryInformation,
				FileMailslotSetInformation,
				FileCompressionInformation,
				FileObjectIdInformation,
				FileCompletionInformation,
				FileMoveClusterInformation,
				FileQuotaInformation,
				FileReparsePointInformation,
				FileNetworkOpenInformation,
				FileAttributeTagInformation,
				FileTrackingInformation,
				FileIdBothDirectoryInformation,
				FileIdFullDirectoryInformation,
				FileValidDataLengthInformation,
				FileShortNameInformation,
				FileIoCompletionNotificationInformation,
				FileIoStatusBlockRangeInformation,
				FileIoPriorityHintInformation,
				FileSfioReserveInformation,
				FileSfioVolumeInformation,
				FileHardLinkInformation,
				FileProcessIdsUsingFileInformation,
				FileNormalizedNameInformation,
				FileNetworkPhysicalNameInformation,
				FileIdGlobalTxDirectoryInformation,
				FileIsRemoteDeviceInformation,
				FileUnusedInformation,
				FileNumaNodeInformation,
				FileStandardLinkInformation,
				FileRemoteProtocolInformation,
				FileRenameInformationBypassAccessCheck,
				FileLinkInformationBypassAccessCheck,
				FileVolumeNameInformation,
				FileIdInformation,
				FileIdExtdDirectoryInformation,
				FileReplaceCompletionInformation,
				FileHardLinkFullIdInformation,
				FileIdExtdBothDirectoryInformation,
				FileMaximumInformation,
			}
			
			[StructLayout(LayoutKind.Sequential)]
			public struct IO_STATUS_BLOCK
			{
				[StructLayout(LayoutKind.Explicit)]
				public struct StatusBlockUnion
				{
					[FieldOffset(0)]
					public int Status;
					[FieldOffset(0)]
					public IntPtr Pointer;
				}
				
				public StatusBlockUnion StatusBlock;
				public IntPtr Information;
			}
			
			[StructLayout(LayoutKind.Sequential)]
			public struct FILE_FULL_DIR_INFORMATION
			{
				public int NextEntryOffset;
				public int FileIndex;
				public long CreationTime;
				public long LastAccessTime;
				public long LastWriteTime;
				public long ChangeTime;
				public long EndOfFile;
				public long AllocationSize;
				public int FileAttributes;
				public int FileNameLength;
				public int EaSize;
				
				ushort FileNameString;
				
				public unsafe string FileName{
					get{
						fixed(ushort* ptr = &FileNameString)
						{
							return Marshal.PtrToStringUni((IntPtr)ptr, FileNameLength/2);
						}
					}
				}
			}
			
			[DllImport("ntdll.dll", SetLastError=true)]
			public static extern int RtlNtStatusToDosError(int Status);
			
			[DllImport("ntdll.dll", SetLastError=true, CharSet= CharSet.Unicode)]
			public static extern int NtQueryDirectoryFile(
				IntPtr FileHandle, [Optional]IntPtr Event, [Optional]IntPtr ApcRoutine,
				[Optional]IntPtr ApcContext, out IO_STATUS_BLOCK IoStatusBlock,
				IntPtr FileInformation, int Length, FILE_INFORMATION_CLASS FileInformationClass,
				bool ReturnSingleEntry, [Optional]string FileName, bool RestartScan
			);
		}
		
		static class Kernel32
		{
			public enum FILE_INFO_BY_HANDLE_CLASS
			{
				FileBasicInfo = 0,
				FileStandardInfo = 1,
				FileNameInfo = 2,
				FileRenameInfo = 3,
				FileDispositionInfo = 4,
				FileAllocationInfo = 5,
				FileEndOfFileInfo = 6,
				FileStreamInfo = 7,
				FileCompressionInfo = 8,
				FileAttributeTagInfo = 9,
				FileIdBothDirectoryInfo = 10, // 0xA
				FileIdBothDirectoryRestartInfo = 11, // 0xB
				FileIoPriorityHintInfo = 12, // 0xC
				FileRemoteProtocolInfo = 13, // 0xD
				FileFullDirectoryInfo = 14, // 0xE
				FileFullDirectoryRestartInfo = 15, // 0xF
				FileStorageInfo = 16, // 0x10
				FileAlignmentInfo = 17, // 0x11
				FileIdInfo = 18, // 0x12
				FileIdExtdDirectoryInfo = 19, // 0x13
				FileIdExtdDirectoryRestartInfo = 20, // 0x14
				MaximumFileInfoByHandlesClass
			}
			
			[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
			public struct FILE_FULL_DIR_INFO
			{
				public int NextEntryOffset;
				public int FileIndex;
				public long CreationTime;
				public long LastAccessTime;
				public long LastWriteTime;
				public long ChangeTime;
				public long EndOfFile;
				public long AllocationSize;
				public int FileAttributes;
				public int FileNameLength;
				public int EaSize;
				[MarshalAs(UnmanagedType.ByValTStr, SizeConst=256)]
				public string FileName;
			}
			
			[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
			public struct FILE_ID_BOTH_DIR_INFO
			{
				public static readonly int Size = Marshal.SizeOf(typeof(FILE_ID_BOTH_DIR_INFO));
				
				public int NextEntryOffset;
				public int FileIndex;
				public long CreationTime;
				public long LastAccessTime;
				public long LastWriteTime;
				public long ChangeTime;
				public long EndOfFile;
				public long AllocationSize;
				public int FileAttributes;
				public int FileNameLength;
				public int EaSize;
				public int ShortNameLength;
				[MarshalAs(UnmanagedType.ByValTStr, SizeConst=12)]
				public string ShortName;
				public long FileId;
				[MarshalAs(UnmanagedType.ByValTStr, SizeConst=256)]
				public string FileName;
			}
			
			[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
			public struct FILE_STREAM_INFO
			{
				public static readonly int Size = Marshal.SizeOf(typeof(FILE_STREAM_INFO));
				
				public int NextEntryOffset;
				public int StreamNameLength;
				public long StreamSize;
				public long StreamAllocationSize;
				[MarshalAs(UnmanagedType.ByValTStr, SizeConst=256)]
				private string StreamNameInternal;
				
				public string StreamName{
					get{
						return StreamNameInternal.Substring(0, StreamNameLength/2);
					}
				}
			}
			
			[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
			public struct FILE_NAME_INFO
			{
				public static readonly int Size = Marshal.SizeOf(typeof(FILE_NAME_INFO));
				
				public int FileNameLength;
				
				[MarshalAs(UnmanagedType.ByValTStr, SizeConst=256)]
				private string FileNameInternal;
				
				public string FileName{
					get{
						return FileNameInternal.Substring(0, FileNameLength/2);
					}
				}
			}
			
			[StructLayout(LayoutKind.Sequential)]
			public struct BY_HANDLE_FILE_INFORMATION
			{
				public int dwFileAttributes;
				public FILETIME ftCreationTime;
				public FILETIME ftLastAccessTime;
				public FILETIME ftLastWriteTime;
				public int dwVolumeSerialNumber;
				public int nFileSizeHigh;
				public int nFileSizeLow;
				public int nNumberOfLinks;
				public int nFileIndexHigh;
				public int nFileIndexLow;
			}
			
			[DllImport("kernel32.dll", SetLastError=true)]
			static extern bool GetFileInformationByHandle(IntPtr hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);
			
			[DebuggerStepThrough]
			public static BY_HANDLE_FILE_INFORMATION GetFileInformationByHandle(IntPtr hFile)
			{
				BY_HANDLE_FILE_INFORMATION fileInformation;
				if(!GetFileInformationByHandle(hFile, out fileInformation)) throw new Win32Exception();
				return fileInformation;
			}
			
			[DllImport("kernel32.dll", SetLastError=true)]
			static extern bool GetFileInformationByHandleEx(IntPtr hFile, FILE_INFO_BY_HANDLE_CLASS FileInformationClass, out FILE_ID_BOTH_DIR_INFO lpFileInformation, int dwBufferSize);
			
			[DllImport("kernel32.dll", SetLastError=true)]
			static extern bool GetFileInformationByHandleEx(IntPtr hFile, FILE_INFO_BY_HANDLE_CLASS FileInformationClass, out FILE_NAME_INFO lpFileInformation, int dwBufferSize);
			
			[DllImport("kernel32.dll", SetLastError=true)]
			public static extern bool GetFileInformationByHandleEx(IntPtr hFile, FILE_INFO_BY_HANDLE_CLASS FileInformationClass, IntPtr lpFileInformation, int dwBufferSize);
			
			[DebuggerStepThrough]
			public static bool GetFileInformationByHandleEx(IntPtr hFile, out FILE_ID_BOTH_DIR_INFO lpFileInformation)
			{
				bool result = GetFileInformationByHandleEx(hFile, FILE_INFO_BY_HANDLE_CLASS.FileIdBothDirectoryInfo, out lpFileInformation, FILE_ID_BOTH_DIR_INFO.Size);
				if(!result)
				{
					int error = Marshal.GetLastWin32Error();
					if(error != 18) throw new Win32Exception(error);
					return false;
				}
				return true;
			}
			
			[DebuggerStepThrough]
			public static void GetFileInformationByHandleEx(IntPtr hFile, out FILE_NAME_INFO lpFileInformation)
			{
				bool result = GetFileInformationByHandleEx(hFile, FILE_INFO_BY_HANDLE_CLASS.FileNameInfo, out lpFileInformation, FILE_NAME_INFO.Size);
				if(!result)
				{
					throw new Win32Exception();
				}
			}
			
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
			
			[DllImport("kernel32.dll")]
			public static extern IntPtr GetCurrentProcess();
			
		    private const int FILE_READ_EA = 0x0008;
		    private const int FILE_FLAG_BACKUP_SEMANTICS = 0x2000000;
			
		    [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		    static extern int GetFinalPathNameByHandle(IntPtr hFile, StringBuilder lpszFilePath, int cchFilePath, int dwFlags);
			
		    [DllImport("kernel32.dll", SetLastError=true, EntryPoint="CloseHandle")]
		    static extern bool _CloseHandle(IntPtr hObject);
		    
		    [DllImport("kernel32.dll", SetLastError=true)]
		    static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, IntPtr hSourceHandle, IntPtr hTargetProcessHandle, out IntPtr lpTargetHandle, int dwDesiredAccess, bool bInheritHandle, int dwOptions);
			
			[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
			public struct WIN32_FIND_DATA
			{
				public FileAttributes dwFileAttributes;
				public FILETIME ftCreationTime;
				public FILETIME ftLastAccessTime;
				public FILETIME ftLastWriteTime;
				public int nFileSizeHigh;
				public int nFileSizeLow;
				public int dwReserved0;
				public int dwReserved1;
				[MarshalAs(UnmanagedType.ByValTStr, SizeConst=260)]
  				public string cFileName;
				[MarshalAs(UnmanagedType.ByValTStr, SizeConst=14)]
				public string cAlternateFileName;
			}
			
		    [DllImport("kernel32.dll", SetLastError=true, CharSet=CharSet.Auto)]
		    static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);
		    
		    [DllImport("kernel32.dll", SetLastError=true, CharSet=CharSet.Auto)]
		    static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);
		    
		    [DllImport("kernel32.dll", SetLastError=true)]
		    static extern bool FindClose(IntPtr hFindFile);
		    
			[DebuggerStepThrough]
		    public static IEnumerable<WIN32_FIND_DATA> FindFiles(string lpFileName)
		    {
		    	WIN32_FIND_DATA findFileData;
		    	
		    	IntPtr handle = FindFirstFile(lpFileName, out findFileData);
		    	if(handle == (IntPtr)(-1))
		    	{
		    		int error = Marshal.GetLastWin32Error();
		    		if(error != 2) throw new Win32Exception(error);
		    		yield break;
		    	}
		    	try{
		    		yield return findFileData;
		    		
		    		while(true)
		    		{
		    			if(!FindNextFile(handle, out findFileData))
		    			{
				    		int error = Marshal.GetLastWin32Error();
				    		if(error != 18) throw new Win32Exception(error);
				    		yield break;
		    			}
		    			yield return findFileData;
		    		}
		    	}finally{
		    		if(!FindClose(handle)) throw new Win32Exception();
		    	}
		    }
		    
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
		    
			[DebuggerStepThrough]
		    public static IntPtr DuplicateHandle(IntPtr hSourceProcessHandle, IntPtr hSourceHandle, IntPtr hTargetProcessHandle, int dwDesiredAccess, bool bInheritHandle, int dwOptions)
		    {
		    	IntPtr targetHandle;
		    	if(!DuplicateHandle(hSourceProcessHandle, hSourceHandle, hTargetProcessHandle, out targetHandle, dwDesiredAccess, bInheritHandle, dwOptions)) throw new Win32Exception();
		    	return targetHandle;
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
			public static readonly Guid BHID_StorageEnum = new Guid("4621A4E3-F0D6-4773-8A9C-46E77B174840");
			
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
			public static extern IntPtr SHGetIDListFromObject([MarshalAs(UnmanagedType.IUnknown)] object punk);
			
			[DllImport("shell32.dll", SetLastError=true)]
			public static extern IntPtr ILClone(IntPtr pidl);
			
			
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
