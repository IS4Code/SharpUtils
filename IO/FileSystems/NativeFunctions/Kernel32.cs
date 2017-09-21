/* Date: 13.9.2017, Time: 18:54 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	partial class Win32FileSystem
	{
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
			
			[DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true, EntryPoint="GetFileAttributes")]
			static extern int _GetFileAttributes(string lpFileName);
			
			[DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true, EntryPoint="SetFileAttributes")]
			static extern bool _SetFileAttributes(string lpFileName, int dwFileAttributes);
			
			[DebuggerStepThrough]
			public static int GetFileAttributes(string lpFileName)
			{
				int attr = _GetFileAttributes(lpFileName);
				if(attr == -1) throw new Win32Exception();
				return attr;
			}
			
			[DebuggerStepThrough]
			public static void SetFileAttributes(string lpFileName, int dwFileAttributes)
			{
				if(!_SetFileAttributes(lpFileName, dwFileAttributes)) throw new Win32Exception();
			}
			
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
			
		    [DllImport("kernel32.dll")]
		    public static extern bool CompareObjectHandles(IntPtr hFirstObjectHandle, IntPtr hSecondObjectHandle);
		    
		    [DllImport("kernel32.dll", SetLastError=true, EntryPoint="CloseHandle")]
		    static extern bool _CloseHandle(IntPtr hObject);
		    
		    [DllImport("kernel32.dll", SetLastError=true)]
		    static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, IntPtr hSourceHandle, IntPtr hTargetProcessHandle, out IntPtr lpTargetHandle, int dwDesiredAccess, bool bInheritHandle, int dwOptions);
		    
		    [DllImport("kernel32.dll", SetLastError=true, CharSet=CharSet.Auto)]
		    static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName, CopyProgressRoutine lpProgressRoutine, IntPtr lpData, IntPtr pbCancel, int dwCopyFlags);
		    
			[DebuggerStepThrough]
		    public static bool CopyFileEx(string lpExistingFileName, string lpNewFileName, CopyProgressRoutine lpProgressRoutine, int dwCopyFlags, bool throwOnCancel)
		    {
		    	bool ok = CopyFileEx(lpExistingFileName, lpNewFileName, lpProgressRoutine, IntPtr.Zero, IntPtr.Zero, dwCopyFlags);
		    	if(!ok)
		    	{
		    		var exception = new Win32Exception();
		    		if(throwOnCancel || unchecked((uint)exception.NativeErrorCode != 1235)) throw exception;
		    		return false;
		    	}
		    	return true;
		    }
		    
			[DebuggerStepThrough]
		    public static bool CopyFileEx(string lpExistingFileName, string lpNewFileName, CopyProgressRoutine lpProgressRoutine, int dwCopyFlags, CancellationToken cancellationToken)
		    {
		    	IntPtr pbCancel = Marshal.AllocHGlobal(4);
		    	try{
			    	cancellationToken.Register(()=>Marshal.WriteInt32(pbCancel, 1));
			    	bool ok = CopyFileEx(lpExistingFileName, lpNewFileName, lpProgressRoutine, IntPtr.Zero, pbCancel, dwCopyFlags);
			    	if(!ok)
			    	{
			    		var exception = new Win32Exception();
			    		if(unchecked((uint)exception.NativeErrorCode != 1235)) throw exception;
			    		return false;
			    	}
		    	}finally{
		    		Marshal.FreeHGlobal(pbCancel);
		    	}
		    	return true;
		    }
		    
		    public delegate int CopyProgressRoutine(long TotalFileSize, long TotalBytesTransferred, long StreamSize, long StreamBytesTransferred, int dwStreamNumber, int dwCallbackReason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData);
		    
		    [DllImport("kernel32.dll", SetLastError=true, CharSet=CharSet.Auto, EntryPoint="MoveFileEx")]
		    static extern bool _MoveFileEx(string lpExistingFileName, string lpNewFileName, int dwFlags);
		    
			[DebuggerStepThrough]
		    public static void MoveFileEx(string lpExistingFileName, string lpNewFileName, int dwFlags)
		    {
		    	bool ok = _MoveFileEx(lpExistingFileName, lpNewFileName, dwFlags);
		    	if(!ok) throw new Win32Exception();
		    }
		    
		    [DllImport("kernel32.dll", SetLastError=true, CharSet=CharSet.Auto, EntryPoint="DeleteFile")]
		    static extern bool _DeleteFile(string lpFileName);
		    
			[DebuggerStepThrough]
			public static void DeleteFile(string lpFileName)
			{
				if(!_DeleteFile(lpFileName)) throw new Win32Exception();
			}
			
			[DllImport("kernel32.dll", SetLastError=true, CharSet=CharSet.Auto)]
			static extern bool MoveFileWithProgress(string lpExistingFileName, string lpNewFileName, CopyProgressRoutine lpProgressRoutine, IntPtr lpData, int dwFlags);
			
			[DebuggerStepThrough]
		    public static bool MoveFileWithProgress(string lpExistingFileName, string lpNewFileName, CopyProgressRoutine lpProgressRoutine, int dwFlags, bool throwOnCancel)
		    {
		    	bool ok = MoveFileWithProgress(lpExistingFileName, lpNewFileName, lpProgressRoutine, IntPtr.Zero, dwFlags);
		    	if(!ok)
		    	{
		    		var exception = new Win32Exception();
		    		if(throwOnCancel || unchecked((uint)exception.NativeErrorCode != 1235)) throw exception;
		    		return false;
		    	}
		    	return true;
		    }
			
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
	}
}
