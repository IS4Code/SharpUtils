/* Date: 13.9.2017, Time: 18:53 */
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	partial class Win32FileSystem
	{
		static class Ntdll
		{
			[DllImport("ntdll.dll")]
			public static extern int RtlNtStatusToDosError(int Status);
			
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
			
			[DebuggerStepThrough]
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
