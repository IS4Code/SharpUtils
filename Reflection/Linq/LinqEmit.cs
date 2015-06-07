/* Date: 31.12.2014, Time: 0:08 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace IllidanS4.SharpUtils.Reflection.Linq
{
	public sealed class LinqEmit : LinqEmitBase<MulticastDelegate>
	{
		
	}
	
	public abstract class LinqEmitBase<TDelegateBase> where TDelegateBase : class, ICloneable, ISerializable
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
			DynamicMethod dyn = new DynamicMethod(name, delSig.ReturnType, delSig.ParameterTypes, TypeOf<LinqEmit>.TypeID.Module, true);
			var il = dyn.GetILGenerator();
			foreach(var instruction in instructions)
			{
				instruction.ToString();
				instruction.Emit(il);
			}
			return (TDelegate)(object)dyn.CreateDelegate(typeof(TDelegate));
		}
		
		
		public static TDelegate CreateDynamicMethod<TDelegate>(params object[] instructions) where TDelegate : TDelegateBase
		{
			return CreateDynamicMethod<TDelegate>("", (IEnumerable<object>)instructions);
		}
		
		public static TDelegate CreateDynamicMethod<TDelegate>(string name, params object[] instructions) where TDelegate : TDelegateBase
		{
			return CreateDynamicMethod<TDelegate>(name, (IEnumerable<object>)instructions);
		}
		
		public static TDelegate CreateDynamicMethod<TDelegate>(IEnumerable<object> instructions) where TDelegate : TDelegateBase
		{
			return CreateDynamicMethod<TDelegate>("", instructions);
		}
		
		public static TDelegate CreateDynamicMethod<TDelegate>(string name, IEnumerable<object> instructions) where TDelegate : TDelegateBase
		{
			return CreateDynamicMethod<TDelegate>(name, GetInstructions(instructions));
		}
		
		private static IEnumerable<Instruction> GetInstructions(IEnumerable<object> tokens)
		{
			List<object> ctorArgs = new List<object>();
			foreach(object token in tokens)
			{
				if(token is Instruction)
				{
					yield return ((Instruction)token);
					continue;
				}
				if(token is OpCode)
				{
					if(ctorArgs.Count > 0)
					{
						yield return (Instruction)Activator.CreateInstance(TypeOf<Instruction>.TypeID, ctorArgs.ToArray());
						ctorArgs.Clear();
					}
				}
				ctorArgs.Add(token);
			}
			if(ctorArgs.Count > 0)
			{
				yield return (Instruction)Activator.CreateInstance(TypeOf<Instruction>.TypeID, ctorArgs.ToArray());
			}
		}
	}
}
