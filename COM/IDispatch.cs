/* Date: 21.12.2012, Time: 20:48 */
using System;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Com
{
	/// <summary>
	/// IDispatch interface for COM reflection.
	/// </summary>
	[Guid("00020400-0000-0000-C000-000000000046"), ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[CLSCompliant(false)]
	public interface IDispatch
	{ 
	    [PreserveSig] 
	    int GetTypeInfoCount(out int pctinfo); 
	
	    [PreserveSig] 
	    int GetTypeInfo( 
	        [MarshalAs(UnmanagedType.U4)] int iTInfo, 
	        [MarshalAs(UnmanagedType.U4)] int lcid, 
	        out System.Runtime.InteropServices.ComTypes.ITypeInfo ppTInfo); 
	
	    [PreserveSig] 
	    int GetIDsOfNames( 
	        ref Guid riid, 
	        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] 
	        string[] rgszNames, 
	        int cNames, 
	        int lcid, 
	        [MarshalAs(UnmanagedType.LPArray)] int[] rgDispId); 
	
	    [PreserveSig]
		int Invoke( 
	        int dispIdMember, 
	        ref Guid riid, 
	        uint lcid, 
	        ushort wFlags, 
	        ref System.Runtime.InteropServices.ComTypes.DISPPARAMS pDispParams, 
	        out object pVarResult, 
	        ref System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo, 
	        IntPtr[] puArgErr); 
	}
}
