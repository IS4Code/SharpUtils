/* Date: 30.11.2014, Time: 11:28 */
using System;
using System.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection.Emit
{
	public enum CorElementType : byte
	{
		Array = 20,
		Boolean = 2,
		ByRef = 0x10,
		Class = 0x12,
		CModOpt = 0x20,
		CModReqd = 0x1f,
		End = 0,
		FnPtr = 0x1b,
		GenericInst = 0x15,
		Char = 3,
		I = 0x18,
		I1 = 4,
		I2 = 6,
		I4 = 8,
		I8 = 10,
		Internal = 0x21,
		Max = 0x22,
		Modifier = 0x40,
		MVar = 30,
		Object = 0x1c,
		Pinned = 0x45,
		Ptr = 15,
		R4 = 12,
		R8 = 13,
		Sentinel = 0x41,
		String = 14,
		SzArray = 0x1d,
		TypedByRef = 0x16,
		U = 0x19,
		U1 = 5,
		U2 = 7,
		U4 = 9,
		U8 = 11,
		ValueType = 0x11,
		Var = 0x13,
		Void = 1
	}
	
	public static class CorElementType_Extensions
	{
		private static readonly Type type = typeof(AssemblyBuilder).Assembly.GetType("System.Reflection.CorElementType");
		
		public static object ToInternal(this CorElementType elemType)
		{
			return Enum.ToObject(type, (byte)elemType);
		}
	}
}
