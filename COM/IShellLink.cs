/* Date: 5.9.2017, Time: 18:27 */
using System;
using System.Runtime.InteropServices;
using System.Text;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace IllidanS4.SharpUtils.Com
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("000214F9-0000-0000-C000-000000000046")]
    [CLSCompliant(false)]
	public interface IShellLink
	{
		void GetPath([MarshalAs(UnmanagedType.LPWStr)]StringBuilder pszFile, int cchMaxPath, out WIN32_FIND_DATA pfd, SLGP_FLAGS fFlags);
		IntPtr GetIDList();
		void SetIDList(IntPtr pidl);
		void GetDescription([MarshalAs(UnmanagedType.LPWStr)]StringBuilder pszName, int cchMaxName);
		void SetDescription([MarshalAs(UnmanagedType.LPWStr)]string pszName);
		void GetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)]StringBuilder pszDir, int cchMaxPath);
		void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)]string pszDir);
		void GetArguments([MarshalAs(UnmanagedType.LPWStr)]StringBuilder pszArgs, int cchMaxPath);
		void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
		short GetHotkey();
		void SetHotkey(short wHotkey);
		int GetShowCmd();
		void SetShowCmd(int iShowCmd);
		int GetIconLocation([MarshalAs(UnmanagedType.LPWStr)]StringBuilder pszIconPath, int cchIconPath);
		void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)]string pszIconPath, int iIcon);
		void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)]string pszPathRel, int dwReserved);
		void Resolve(IntPtr hwnd, SLR_FLAGS fFlags);
		void SetPath([MarshalAs(UnmanagedType.LPWStr)]string pszFile);
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
	public struct WIN32_FIND_DATA
	{
		public int dwFileAttributes;
		FILETIME ftCreationTime;
		FILETIME ftLastAccessTime;
		FILETIME ftLastWriteTime;
		public int nFileSizeHigh;
		public int nFileSizeLow;
		public int dwReserved0;
		public int dwReserved1;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=260)]
		public string cFileName;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=14)]
		public string cAlternateFileName;
	}
	
	[Flags]
	public enum SLGP_FLAGS
	{
		SLGP_SHORTPATH = 0x1,
		SLGP_UNCPRIORITY = 0x2,
		SLGP_RAWPATH = 0x4,
	}
	
	[Flags]
	public enum SLR_FLAGS
	{
		SLR_NO_UI = 0x1,
		SLR_ANY_MATCH = 0x2,
		SLR_UPDATE = 0x4,
		SLR_NOUPDATE = 0x8,
		SLR_NOSEARCH = 0x10,
		SLR_NOTRACK = 0x20,
		SLR_NOLINKINFO = 0x40,
		SLR_INVOKE_MSI = 0x80,
	}
}
