/* Date: 30.11.2014, Time: 11:31 */
using System;
using System.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection.Emit
{
	public enum MdSigCallingConvention : byte
	{
		C = 1,
		CallConvMask = 15,
		Default = 0,
		ExplicitThis = 0x40,
		FastCall = 4,
		Field = 6,
		Generic = 0x10,
		GenericInst = 10,
		HasThis = 0x20,
		LocalSig = 7,
		Property = 8,
		StdCall = 2,
		ThisCall = 3,
		Unmgd = 9,
		Vararg = 5
	}
	
	public static class MdSigCallingConvention_Extensions
	{
		private static readonly Type type = typeof(AssemblyBuilder).Assembly.GetType("System.Reflection.MdSigCallingConvention");
		
		public static object ToInternal(this MdSigCallingConvention elemType)
		{
			return Enum.ToObject(type, (byte)elemType);
		}
		
	}
}
