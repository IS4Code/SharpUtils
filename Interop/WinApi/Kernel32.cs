/* Date: 8.3.2017, Time: 14:03 */
using System;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Interop.WinApi
{
	partial class Win32Control
	{
		private static class Kernel32
		{
			public const string Lib = "kernel32.dll";
			const CharSet DefaultCharSet = CharSet.Auto;
			
			public static readonly IntPtr RT_VERSION = (IntPtr)16;
			
			[DllImport(Lib, SetLastError=true, CharSet=DefaultCharSet)]
			public static extern IntPtr FindResource(IntPtr hModule, IntPtr lpName, IntPtr lpType);
			
			[DllImport(Lib, SetLastError=true, CharSet=DefaultCharSet)]
			public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);
			
			[DllImport(Lib, SetLastError=true, CharSet=DefaultCharSet)]
			public static extern IntPtr LockResource(IntPtr hResData);
			
			[DllImport(Lib, SetLastError=true, CharSet=DefaultCharSet)]
			public static extern int SizeofResource(IntPtr hModule, IntPtr hResInfo);
		}
	}
}
