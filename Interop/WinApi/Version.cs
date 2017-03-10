/* Date: 10.3.2017, Time: 12:16 */
using System;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Interop.WinApi
{
	partial class Win32Control
	{
		private static class Version
		{
			public const string Lib = "version.dll";
			const CharSet DefaultCharSet = CharSet.Auto;
			
			[DllImport(Lib, SetLastError=true, CharSet=DefaultCharSet)]
			public static extern bool VerQueryValue(byte[] pBlock, string lpSubBlock, out IntPtr lplpBuffer, out int puLen);
		}
	}
}
