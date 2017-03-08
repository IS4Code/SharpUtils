/* Date: 8.3.2017, Time: 14:21 */
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace IllidanS4.SharpUtils.Interop.WinApi
{
	partial class Win32Control
	{
		private static class PsApi
		{
			public const string Lib = "psapi.dll";
			const CharSet DefaultCharSet = CharSet.Auto;
			
			[DllImport(Lib, SetLastError=true, CharSet=DefaultCharSet)]
			static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, StringBuilder lpFilename, int nSize);
			
			public static string GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule)
			{
				int capacity = 260;
				StringBuilder buffer;
				do{
					buffer = new StringBuilder(capacity);
					GetModuleFileNameEx(hProcess, hModule, buffer, buffer.Capacity);
					capacity *= 2;
				}while(Marshal.GetLastWin32Error() == 0x7A);
				return buffer.ToString();
			}
		}
	}
}
