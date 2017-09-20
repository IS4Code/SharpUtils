/* Date: 4.9.2017, Time: 19:09 */
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace IllidanS4.SharpUtils.COM
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214E6-0000-0000-C000-000000000046")]
    [CLSCompliant(false)]
	public interface IShellFolder
    {
	    void ParseDisplayName(IntPtr hwnd, IBindCtx pbc, [MarshalAs(UnmanagedType.LPWStr)]string pszDisplayName, out uint pchEaten, out IntPtr ppidl, ref uint pdwAttributes);
		IEnumIDList EnumObjects(IntPtr hwnd, SHCONTF grfFlags);
		[return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex=2)]
	    object BindToObject(IntPtr pidl, IBindCtx pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
		[return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex=2)]
	    object BindToStorage(IntPtr pidl, IBindCtx pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
		[PreserveSig]Int32 CompareIDs(Int32 lParam, IntPtr pidl1, IntPtr pidl2);
		[return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex=1)]
	    object CreateViewObject(IntPtr hwndOwner, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
		void GetAttributesOf(uint cidl, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]IntPtr[] apidl, ref SFGAOF rgfInOut);
		[return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex=3)]
	    object GetUIObjectOf(IntPtr hwndOwner, uint cidl, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]IntPtr[] apidl, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, ref uint rgfReserved);
		STRRET GetDisplayNameOf(IntPtr pidl, SHGDNF uFlags);
		IntPtr SetNameOf(IntPtr hwnd, IntPtr pidl, [MarshalAs(UnmanagedType.LPWStr)]string pszName, SHGDNF uFlags);
	}
    
    [Flags]
	public enum SFGAOF
    {
		SFGAO_CANCOPY = 0x00000001,
		SFGAO_CANMOVE = 0x00000002,
		SFGAO_CANLINK = 0x00000004,
		SFGAO_STORAGE = 0x00000008,
		SFGAO_CANRENAME = 0x00000010,
		SFGAO_CANDELETE = 0x00000020,
		SFGAO_HASPROPSHEET = 0x00000040,
		SFGAO_DROPTARGET = 0x00000100,
		SFGAO_CAPABILITYMASK = 0x00000177,
		SFGAO_SYSTEM = 0x00001000,
		SFGAO_ENCRYPTED = 0x00002000,
		SFGAO_ISSLOW = 0x00004000,
		SFGAO_GHOSTED = 0x00008000,
		SFGAO_LINK = 0x00010000,
		SFGAO_SHARE = 0x00020000,
		SFGAO_READONLY = 0x00040000,
		SFGAO_HIDDEN = 0x00080000,
		SFGAO_DISPLAYATTRMASK = 0x000FC000,
		SFGAO_FILESYSANCESTOR = 0x10000000,
		SFGAO_FOLDER = 0x20000000,
		SFGAO_FILESYSTEM = 0x40000000,
		SFGAO_HASSUBFOLDER = unchecked((int)0x80000000),
		SFGAO_CONTENTSMASK = unchecked((int)0x80000000),
		SFGAO_VALIDATE = 0x01000000,
		SFGAO_REMOVABLE = 0x02000000,
		SFGAO_COMPRESSED = 0x04000000,
		SFGAO_BROWSABLE = 0x08000000,
		SFGAO_NONENUMERATED = 0x00100000,
		SFGAO_NEWCONTENT = 0x00200000,
		SFGAO_CANMONIKER = 0x00400000,
		SFGAO_HASSTORAGE = 0x00400000,
		SFGAO_STREAM = 0x00400000,
		SFGAO_STORAGEANCESTOR = 0x00800000,
		SFGAO_STORAGECAPMASK = 0x70C50008,
    }

	[Flags]
    public enum SHCONTF
    {
 		SHCONTF_CHECKING_FOR_CHILDREN = 0x00010,
	    SHCONTF_FOLDERS = 0x0020,
	    SHCONTF_NONFOLDERS = 0x0040,
	    SHCONTF_INCLUDEHIDDEN = 0x0080,
	    SHCONTF_INIT_ON_FIRST_NEXT = 0x0100,
	    SHCONTF_NETPRINTERSRCH = 0x0200,
	    SHCONTF_SHAREABLE = 0x0400,
	    SHCONTF_STORAGE = 0x0800,
		SHCONTF_NAVIGATION_ENUM = 0x01000,
		SHCONTF_FASTITEMS = 0x02000,
		SHCONTF_FLATLIST = 0x04000,
		SHCONTF_ENABLE_ASYNC = 0x08000,
		SHCONTF_INCLUDESUPERHIDDEN = 0x10000,
    }

	[Flags]
    public enum SHGDNF
    {
	    SHGDN_NORMAL = 0x0000,
	    SHGDN_INFOLDER = 0x0001,
	    SHGDN_FOREDITING = 0x1000,
	    SHGDN_FORADDRESSBAR = 0x4000,
	    SHGDN_FORPARSING = 0x8000,
    }

    [StructLayout(LayoutKind.Sequential, Size=264)]
    [CLSCompliant(false)]
    public struct STRRET
    {
	    public uint uType;
    }
}
