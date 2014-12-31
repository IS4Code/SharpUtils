/* Date: 15.12.2014, Time: 22:07 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Accessing;

namespace IllidanS4.SharpUtils.Reflection
{
	public static class VarArgsInvoke<TDelegate>
	{
		public static readonly IIndexableGetter<int,TDelegate> Invoke;
		
		private static readonly Type delType = typeof(TDelegate);
		private static readonly Type dynType;
		private static readonly Type[] argTypes;
		private static readonly Type retType;
		private static readonly bool passmethod;
		
		static VarArgsInvoke()
		{
			if(delType.BaseType != typeof(MulticastDelegate))
			{
				throw new TypeLoadException("TDelegate must be a delegate type.");
			}
			
			Invoke = new InvokeCache();
			MethodInfo invoke = delType.GetMethod("Invoke");
			argTypes = invoke.GetParameters().Select(p => p.ParameterType).ToArray();
			Type last = argTypes[argTypes.Length-1];
			retType = invoke.ReturnType;
			if(last == typeof(MethodInfo))
			{
				passmethod = true;
				argTypes[argTypes.Length-1] = typeof(IntPtr);
				dynType = ReflectionTools.GetDelegateType(retType, argTypes);
			}else if(last != typeof(IntPtr))
			{
				throw new TypeLoadException("Last parameter of the delegate type must be System.IntPtr or System.Reflection.MethodInfo.");
			}else{
				dynType = delType;
			}
		}
		
		public static TDelegate GetInvoker(int fixedArgsCount)
		{
			Type[] fix = new Type[fixedArgsCount];
			Type[] vrg = new Type[argTypes.Length-fixedArgsCount-1];
			Array.Copy(argTypes, 0, fix, 0, fix.Length);
			Array.Copy(argTypes, fix.Length, vrg, 0, vrg.Length);
			DynamicMethod dyn = new DynamicMethod("VarArgsInvoker", retType, argTypes, typeof(VarArgsInvoke<TDelegate>).Module, true);
			var il = dyn.GetILGenerator();
			for(int i = 0; i < argTypes.Length; i++)
			{
				il.EmitLdarg(i);
			}
			il.EmitCalli(OpCodes.Calli, CallingConventions.VarArgs, retType, fix, vrg);
			il.Emit(OpCodes.Ret);
			if(passmethod)
			{
				ParameterExpression[] pargs = argTypes.Select(a => Expression.Parameter(a)).ToArray();
				pargs[pargs.Length-1] = Expression.Parameter(typeof(MethodInfo));
				Expression exp = Expression.Call(
					dyn,
					pargs.Take(pargs.Length-1).Cast<Expression>().Concat(
						Extensions.Once(
							Expression.Call(
								Expression.Property(pargs[pargs.Length-1], "MethodHandle"),
								"GetFunctionPointer", null
							)
						)
					)
				);
				return Expression.Lambda<TDelegate>(exp, pargs).Compile();
			}else{
				TDelegate del = (TDelegate)(object)dyn.CreateDelegate(delType);
				((InvokeCache)Invoke).dict[fixedArgsCount] = del;
				return del;
			}
		}
		
		private class InvokeCache : IIndexableGetter<int,TDelegate>
		{
			public readonly Dictionary<int,TDelegate> dict;
			
			public InvokeCache()
			{
				dict = new Dictionary<int, TDelegate>();
			}
			
			public TDelegate this[int fixedArgsCount]{
				get{
					TDelegate invoke;
					if(!dict.TryGetValue(fixedArgsCount, out invoke))
					{
						return GetInvoker(fixedArgsCount);
					}
					return invoke;
				}
			}
		}
	}
}
