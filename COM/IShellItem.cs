/* Date: 5.9.2017, Time: 2:40 */
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace IllidanS4.SharpUtils.COM
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
    [CLSCompliant(false)]
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
	
	[CLSCompliant(false)]
	[Flags]
	public enum SICHINTF : uint
	{
		SICHINT_DISPLAY = 0x00000000,
		SICHINT_ALLFIELDS  = 0x80000000,
		SICHINT_CANONICAL = 0x10000000,
		SICHINT_TEST_FILESYSPATH_IF_NOT_EQUAL = 0x20000000,
	}
	
	[CLSCompliant(false)]
	public enum SIGDN : uint
	{
		SIGDN_NORMALDISPLAY = 0x00000000,
		SIGDN_PARENTRELATIVEPARSING = 0x80018001,
		SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,
		SIGDN_PARENTRELATIVEEDITING = 0x80031001,
		SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,
		SIGDN_FILESYSPATH = 0x80058000,
		SIGDN_URL = 0x80068000,
		SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001,
		SIGDN_PARENTRELATIVE  = 0x80080001,
		SIGDN_PARENTRELATIVEFORUI  = 0x80094001,
	}
}
