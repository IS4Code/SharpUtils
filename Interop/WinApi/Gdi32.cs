/* Date: 10.3.2017, Time: 12:33 */
using System;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Interop.WinApi
{
	partial class Win32Control
	{
		private static class Gdi32
		{
			public const string Lib = "gdi32.dll";
			const CharSet DefaultCharSet = CharSet.Auto;
			
			[DllImport(Lib, SetLastError=true, CharSet=DefaultCharSet)]
			public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
		}
	}
}
