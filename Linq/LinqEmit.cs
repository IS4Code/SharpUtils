/* Date: 31.12.2014, Time: 0:08 */
using System;
using System.Reflection.Emit;

namespace IllidanS4.SharpUtils.Linq
{
	public static class LinqEmit
	{
		public static TDelegate CreateDynamicMethod<TDelegate>(string name, Type returnType, Type[] parameterTypes, params OpCode[] opcodes) where TDelegate : class
		{
			DynamicMethod dyn = new DynamicMethod(name, returnType, parameterTypes);
			var il = dyn.GetILGenerator();
			foreach(var opcode in opcodes)
			{
				il.Emit(opcode);
			}
			return (TDelegate)(object)dyn.CreateDelegate(typeof(TDelegate));
		}
	}
}
