/* Date: 31.12.2014, Time: 0:08 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection.Linq
{
	public sealed class LinqEmit : LinqEmitBase<MulticastDelegate>
	{
		
	}
	
	public abstract class LinqEmitBase<TDelegateBase>
	{
		internal LinqEmitBase(){}
		
		public static TDelegate CreateDynamicMethod<TDelegate>(params Instruction[] instructions) where TDelegate : TDelegateBase
		{
			return CreateDynamicMethod<TDelegate>("", (IEnumerable<Instruction>)instructions);
		}
		
		public static TDelegate CreateDynamicMethod<TDelegate>(string name, params Instruction[] instructions) where TDelegate : TDelegateBase
		{
			return CreateDynamicMethod<TDelegate>(name, (IEnumerable<Instruction>)instructions);
		}
		
		public static TDelegate CreateDynamicMethod<TDelegate>(IEnumerable<Instruction> instructions) where TDelegate : TDelegateBase
		{
			return CreateDynamicMethod<TDelegate>("", instructions);
		}
		
		public static TDelegate CreateDynamicMethod<TDelegate>(string name, IEnumerable<Instruction> instructions) where TDelegate : TDelegateBase
		{
			var delSig = ReflectionTools.GetDelegateSignature(typeof(TDelegate));
			DynamicMethod dyn = new DynamicMethod(name, delSig.ReturnType, delSig.ParameterTypes);
			var il = dyn.GetILGenerator();
			foreach(var instruction in instructions)
			{
				instruction.Emit(il);
			}
			return (TDelegate)(object)dyn.CreateDelegate(typeof(TDelegate));
		}
	}
}
