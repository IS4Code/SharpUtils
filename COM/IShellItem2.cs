/* Date: 5.9.2017, Time: 3:16 */
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace IllidanS4.SharpUtils.COM
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("7E9FB0D3-919F-4307-AB2E-9B1860310C93")]
    [CLSCompliant(false)]
	public interface IShellItem2
	{
		[return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex=2)]
		object BindToHandler(IBindCtx pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid bhid, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
        IShellItem GetParent();
        string GetDisplayName(SIGDN sigdnName);
        SFGAOF GetAttributes(SFGAOF sfgaoMask);
        int Compare(IShellItem psi, SICHINTF hint);
		
        //IShellItem2
	    [return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex=1)]
        object GetPropertyStore(GETPROPERTYSTOREFLAGS flags, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
        [return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex=2)]
        object GetPropertyStoreWithCreateObject(GETPROPERTYSTOREFLAGS flags, [MarshalAs(UnmanagedType.IUnknown)]object punkCreateObject, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
        [return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex=3)]
        object GetPropertyStoreForKeys([MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)]PROPERTYKEY[] rgKeys, uint cKeys, GETPROPERTYSTOREFLAGS flags, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
        [return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex=1)]
        object GetPropertyDescriptionList([In]ref PROPERTYKEY keyType, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
        void Update(IBindCtx pbc);
        IntPtr GetProperty([In]ref PROPERTYKEY key);
        Guid GetCLSID([In]ref PROPERTYKEY key);
        FILETIME GetFileTime([In]ref PROPERTYKEY key);
        int GetInt32([In]ref PROPERTYKEY key);
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetString([In]ref PROPERTYKEY key);
        uint GetUInt32([In]ref PROPERTYKEY key);
        ulong GetUInt64([In]ref PROPERTYKEY key);
        bool GetBool([In]ref PROPERTYKEY key);
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct PROPERTYKEY
	{
		public Guid fmtid;
		public int pid;
		
		public PROPERTYKEY(string fmtid, int pid)
		{
			this.fmtid = new Guid(fmtid);
			this.pid = pid;
		}
	}
	
	[Flags]
	public enum GETPROPERTYSTOREFLAGS
	{
		GPS_DEFAULT                  = 0,
		GPS_HANDLERPROPERTIESONLY    = 0x1,
		GPS_READWRITE                = 0x2,
		GPS_TEMPORARY                = 0x4,
		GPS_FASTPROPERTIESONLY       = 0x8,
		GPS_OPENSLOWITEM             = 0x10,
		GPS_DELAYCREATION            = 0x20,
		GPS_BESTEFFORT               = 0x40,
		GPS_NO_OPLOCK                = 0x80,
		GPS_PREFERQUERYPROPERTIES    = 0x100,
		GPS_MASK_VALID               = 0x1ff,
		GPS_EXTRINSICPROPERTIES      = 0x00000200,
		GPS_EXTRINSICPROPERTIESONLY  = 0x00000400,
	}
}
