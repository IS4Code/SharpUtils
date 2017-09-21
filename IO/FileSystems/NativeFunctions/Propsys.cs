/* Date: 21.9.2017, Time: 11:15 */
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.COM;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	partial class ShellFileSystem
	{
		static class Propsys
		{
			[DllImport("propsys.dll", PreserveSig=false)]
			[return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex=3)]
			static extern object PSCreateSimplePropertyChange(PKA_FLAGS flags, ref PROPERTYKEY key, ref PROPVARIANT propvar, [MarshalAs(UnmanagedType.LPStruct)]Guid riid);
			
			[DebuggerStepThrough]
			public static T PSCreateSimplePropertyChange<T>(PKA_FLAGS flags, PROPERTYKEY key, PROPVARIANT propvar) where T : class
			{
				return (T)PSCreateSimplePropertyChange(flags, ref key, ref propvar, typeof(T).GUID);
			}
			
			[DebuggerStepThrough]
			public static T PSCreateSimplePropertyChange<T>(PKA_FLAGS flags, PROPERTYKEY key, ref PROPVARIANT propvar) where T : class
			{
				return (T)PSCreateSimplePropertyChange(flags, ref key, ref propvar, typeof(T).GUID);
			}
			
			[DllImport("propsys.dll", PreserveSig=false)]
			[return: MarshalAs(UnmanagedType.IUnknown, IidParameterIndex=4)]
			static extern object PSCreatePropertyChangeArray([MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)]PROPERTYKEY[] rgpropkey, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)]PKA_FLAGS[] rgflags, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)]PROPVARIANT[] rgpropvar, int cChanges, [MarshalAs(UnmanagedType.LPStruct)]Guid riid);
			
			[DebuggerStepThrough]
			public static T PSCreatePropertyChangeArray<T>(PROPERTYKEY[] rgpropkey, PKA_FLAGS[] rgflags, PROPVARIANT[] rgpropvar) where T : class
			{
				return (T)PSCreatePropertyChangeArray(rgpropkey, rgflags, rgpropvar, rgpropkey.Length, typeof(T).GUID);
			}
			
			[DllImport("propsys.dll", PreserveSig=false)]
			[return: MarshalAs(UnmanagedType.Struct)]
			public static extern object PropVariantToVariant(ref PROPVARIANT pPropVar);
			
			[DebuggerStepThrough]
			public static object PropVariantToVariant(PROPVARIANT pPropVar)
			{
				return PropVariantToVariant(ref pPropVar);
			}
			
			[DllImport("propsys.dll")]
			static extern int VariantToPropVariant([MarshalAs(UnmanagedType.Struct), In]ref object pVar, out PROPVARIANT pPropVar);
			
			[DebuggerStepThrough]
			public static PROPVARIANT VariantToPropVariant(object pVar)
			{
				PROPVARIANT propVar;
				int hRes = VariantToPropVariant(ref pVar, out propVar);
				Marshal.ThrowExceptionForHR(hRes);
				return propVar;
			}
			
			public enum PKA_FLAGS
			{
				PKA_SET = 0,
				PKA_APPEND = 1,
				PKA_DELETE = 2,
			}
		}
	}
}
