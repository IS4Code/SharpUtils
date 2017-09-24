/* Date: 13.9.2017, Time: 18:53 */
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

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
			
			/*[StructLayout(LayoutKind.Sequential)]
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
			}*/
			
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
			
			[DllImport("ntdll.dll")]
			public static extern int RtlNtStatusToDosError(int Status);
			
			[DllImport("ntdll.dll", CharSet=CharSet.Unicode)]
			public static extern int NtQueryDirectoryFile(
				IntPtr FileHandle, [Optional]IntPtr Event, [Optional]IntPtr ApcRoutine,
				[Optional]IntPtr ApcContext, out IO_STATUS_BLOCK IoStatusBlock,
				IntPtr FileInformation, int Length, FILE_INFORMATION_CLASS FileInformationClass,
				bool ReturnSingleEntry, [Optional]string FileName, bool RestartScan
			);
			
			[StructLayout(LayoutKind.Sequential)]
			public struct IO_STATUS_BLOCK
			{
				IntPtr Status;
				IntPtr Information;
			}
			
			[DllImport("ntdll.dll")]
			public static extern int NtQueryInformationFile(
				IntPtr FileHandle, out IO_STATUS_BLOCK IoStatusBlock, IntPtr FileInformation,
				int Length, FILE_INFORMATION_CLASS FileInformationClass
			);
			
			
			[DllImport("ntdll.dll")]
			static extern int NtQueryObject(
				IntPtr Handle, int ObjectInformationClass,
				IntPtr ObjectInformation, int ObjectInformationLength,
				out int ReturnLength
			);
			
			public static int NtQueryObject(IntPtr Handle, int ObjectInformationClass, IntPtr ObjectInformation, int ObjectInformationLength)
			{
				int length;
				int status = NtQueryObject(Handle, ObjectInformationClass, ObjectInformation, ObjectInformationLength, out length);
				if(status != 0) throw new Win32Exception(RtlNtStatusToDosError(status));
				return length;
			}
			
			[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
			public struct OBJECT_NAME_INFORMATION
			{
				public ushort Length;
				public ushort MaximumLength;
				public string Buffer;
			}
			
			public static void NtQueryObject(IntPtr Handle, out OBJECT_NAME_INFORMATION ObjectNameInformation)
			{
				IntPtr buffer = Marshal.AllocHGlobal(1024);
				try{
					Ntdll.NtQueryObject(Handle, 1, buffer, 1024);
					ObjectNameInformation = Marshal.PtrToStructure<Ntdll.OBJECT_NAME_INFORMATION>(buffer);
				}finally{
					Marshal.FreeHGlobal(buffer);
				}
			}
		}
	}
}
