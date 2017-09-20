/* Date: 5.9.2017, Time: 2:40 */
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace IllidanS4.SharpUtils.COM
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
	public interface IShellItem
	{
		[return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex=2)]
		object BindToHandler(IBindCtx pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid bhid, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
        IShellItem GetParent();
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetDisplayName(SIGDN sigdnName);
        SFGAOF GetAttributes(SFGAOF sfgaoMask);
        int Compare(IShellItem psi, SICHINTF hint);
	}
	
	[Flags]
	public enum SICHINTF
	{
		SICHINT_DISPLAY = 0x00000000,
		SICHINT_ALLFIELDS  = unchecked((int)0x80000000),
		SICHINT_CANONICAL = 0x10000000,
		SICHINT_TEST_FILESYSPATH_IF_NOT_EQUAL = 0x20000000,
	}
	
	public enum SIGDN
	{
		SIGDN_NORMALDISPLAY = 0x00000000,
		SIGDN_PARENTRELATIVEPARSING = unchecked((int)0x80018001),
		SIGDN_DESKTOPABSOLUTEPARSING = unchecked((int)0x80028000),
		SIGDN_PARENTRELATIVEEDITING = unchecked((int)0x80031001),
		SIGDN_DESKTOPABSOLUTEEDITING = unchecked((int)0x8004c000),
		SIGDN_FILESYSPATH = unchecked((int)0x80058000),
		SIGDN_URL = unchecked((int)0x80068000),
		SIGDN_PARENTRELATIVEFORADDRESSBAR = unchecked((int)0x8007c001),
		SIGDN_PARENTRELATIVE = unchecked((int)0x80080001),
		SIGDN_PARENTRELATIVEFORUI = unchecked((int)0x80094001),
	}
}
