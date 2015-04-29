/* Date: 16.4.2015, Time: 13:31 */
using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Reflection;

namespace IllidanS4.SharpUtils.Interop
{
	public sealed class UnmanagedFunctionCaller<TReturn> : IDynamicMetaObjectProvider
	{
		public static readonly dynamic Invoke = new UnmanagedFunctionCaller<TReturn>();
		
		private UnmanagedFunctionCaller()
		{
			
		}
		
		public DynamicMetaObject GetMetaObject(Expression parameter)
		{
			return new InvokerMetaObject(parameter, this);
		}
		
		private class InvokerMetaObject : DynamicMetaObject
		{
			public InvokerMetaObject(Expression expression, UnmanagedFunctionCaller<TReturn> value) : base(expression, BindingRestrictions.Empty, value)
			{
				
			}
			
			public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
			{
				Type tRet = TypeOf<TReturn>.TypeID;
			    BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);
			    Type[] pTypes = args.Select(a => a.LimitType).ToArray();
			    DynamicMethod dyn = new DynamicMethod("Invoker", tRet, pTypes, typeof(UnmanagedFunctionCaller<TReturn>), true);
				var il = dyn.GetILGenerator();
				for(int i = 1; i < pTypes.Length; i++)
				{
					il.EmitLdarg(i);
				}
				il.Emit(OpCodes.Ldarg_0);
				il.EmitCalli(OpCodes.Calli, CallingConvention.Cdecl, tRet, pTypes.Skip(1).ToArray());
				il.Emit(OpCodes.Ret);
				Expression expr = Expression.Call(dyn, args.Select(a => a.Expression));
			    expr = Expression.Convert(expr, binder.ReturnType);
			    return new DynamicMetaObject(expr, restrictions);
			}
		}
	}
}
