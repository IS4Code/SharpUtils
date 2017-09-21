/* Date: 20.9.2017, Time: 23:32 */
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Com
{
	[ComImport]
	[Guid("F917BC8A-1BBA-4478-A245-1BDE03EB9431")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IPropertyChange
	{
		//IObjectWithPropertyKey
		void SetPropertyKey(ref PROPERTYKEY key);
		PROPERTYKEY GetPropertyKey();
		
		//IPropertyChange
		PROPVARIANT ApplyToPropVariant(ref PROPVARIANT propvarIn);
	}
	
	[ComImport]
	[Guid("380F5CAD-1B5E-42F2-805D-637FD392D31E")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IPropertyChangeArray
	{
		int GetCount();
		[return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex=1)]
		object GetAt(int iIndex, [MarshalAs(UnmanagedType.LPStruct)]Guid riid);
		void InsertAt(int iIndex, IPropertyChange ppropChange);
		void Append(IPropertyChange ppropChange);
		void AppendOrReplace(IPropertyChange ppropChange);
		void RemoveAt(int iIndex);
		[PreserveSig]
		HRESULT IsKeyInArray(ref PROPERTYKEY key);
	}
			
	[StructLayout(LayoutKind.Sequential, Size=16)]
	public struct PROPVARIANT
	{
		uint vt;
	}
}
