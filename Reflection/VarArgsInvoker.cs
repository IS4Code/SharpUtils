/* Date: 5.12.2014, Time: 23:00 */
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Interop;

namespace IllidanS4.SharpUtils.Reflection
{
	public class VarArgsInvoker : IDynamicMetaObjectProvider
	{
		public MethodInfo Method{get; private set;}
		public object Target{get; private set;}
		
		public VarArgsInvoker(MethodInfo method, object target)
		{
			Method = method;
			Target = target;
		}
		
		public static dynamic Create(MethodInfo method, object target)
		{
			return new VarArgsInvoker(method, target);
		}
		
		public LambdaExpression CreateInvoker(params Type[] vartypes)
		{
			var margs = Method.GetParameters();
			Type[] dyntypes;
			var tenum = margs.Select(p => p.ParameterType).Concat(vartypes);
			if(Method.IsStatic)
			{
				dyntypes = tenum.ToArray();
			}else{
				dyntypes = new[]{Method.DeclaringType}.Concat(tenum).ToArray();
			}
			DynamicMethod dyn = new DynamicMethod(Method.Name+"DynamicVarArgsInvoker", Method.ReturnType, dyntypes, Method.Module);
			var il = dyn.GetILGenerator();
			for(int i = 0; i < dyntypes.Length; i++)
			{
				il.EmitLdarg(i);
			}
			il.EmitCall(Method.IsStatic?OpCodes.Call:OpCodes.Callvirt, Method, vartypes);
			il.Emit(OpCodes.Ret);
			if(Target == null)
			{
				ParameterExpression[] args = dyntypes.Select(t => Expression.Parameter(t)).ToArray();
				var expr = Expression.Call(dyn, args);
				return Expression.Lambda(expr, args);
			}else{
				ParameterExpression[] args = dyntypes.Skip(1).Select(t => Expression.Parameter(t)).ToArray();
				var expr = Expression.Call(dyn, new Expression[]{Expression.Constant(Target)}.Concat(args).ToArray());
				return Expression.Lambda(expr, args);
			}
		}
		
		private static readonly Expression<Func<MethodInfo,IntPtr>> GetFnptr = (m) => m.MethodHandle.GetFunctionPointer();
		private static readonly Expression<Func<MethodInfo,bool>> IsVoid = (m) => m.ReturnType == typeof(void);
		
		public static LambdaExpression CreateDynamicInvoker(Type returnType, Type[] fixtypes, Type[] vartypes)
		{
			Type[] mtypes;
			int skip = 0;
			/*if(target != null)
			{
				mtypes = new Type[1+fixtypes.Length+vartypes.Length+1];
				mtypes[0] = target.GetType();
				skip = 1;
			}else{*/
			mtypes = new Type[fixtypes.Length+vartypes.Length+1];
			//}
			fixtypes.CopyTo(mtypes, skip);
			vartypes.CopyTo(mtypes, skip+fixtypes.Length);
			mtypes[mtypes.Length-1] = typeof(IntPtr);
			DynamicMethod dyn = new DynamicMethod("DynamicVarArgsInvoker", returnType, mtypes, typeof(VarArgsInvoker));
			var il = dyn.GetILGenerator();
			for(int i = 0; i < mtypes.Length; i++)
			{
				il.EmitLdarg(i);
			}
			il.EmitCalli(OpCodes.Calli, CallingConventions.VarArgs, returnType, fixtypes, vartypes);
			il.Emit(OpCodes.Ret);
			
			var mpar = Expression.Parameter(typeof(MethodInfo));
			List<ParameterExpression> pars = new List<ParameterExpression>();
			pars.Add(mpar);
			pars.AddRange(mtypes.Take(mtypes.Length-1).Select(t => Expression.Parameter(t)));
			
			List<Expression> args = new List<Expression>();
			args.AddRange(pars.Skip(1));
			args.Add(Expression.Invoke(GetFnptr, mpar));
			var invexpr = Expression.Call(dyn, args);
			if(returnType == typeof(void))
			{
				return Expression.Lambda(Expression.Block(invexpr, Expression.Constant(null)), pars);
			}else{
				return Expression.Lambda(invexpr, pars);
			}
		}
		
		public static readonly dynamic Invoke = new DynamicMethodInvoker();
		
		private class DynamicMethodInvoker : DynamicObject
		{
			private class DynamicVarArgsMethodInvoker : IDynamicMetaObjectProvider
			{
				readonly MethodInfo method;
				readonly Type[] fixtypes;
				
				public DynamicVarArgsMethodInvoker(MethodInfo method, object[] args)
				{
					this.method = method;
					fixtypes = args.Select(a => a==null?typeof(object):a.GetType()).ToArray();
				}
				
				private class DynamicVarArgsMethodInvokerMetaObject : DynamicMetaObject
				{
					readonly DynamicVarArgsMethodInvoker value;
					
					public DynamicVarArgsMethodInvokerMetaObject(Expression expression, DynamicVarArgsMethodInvoker value) : base(expression, BindingRestrictions.Empty, value)
					{
						this.value = value;
					}
					
					public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
					{
						var method = value.method;
						BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);
			    		Type[] vartypes = args.Select(a => a.LimitType).ToArray();
						LambdaExpression invoker = CreateDynamicInvoker(method.ReturnType, value.fixtypes, vartypes);
						return new DynamicMetaObject(Expression.Invoke(invoker, new[]{Expression.Constant(method)}.Concat(args.Select(a => a.Expression)).ToArray()), restrictions);
					}
				}
			
				public DynamicMetaObject GetMetaObject(Expression parameter)
				{
					return new DynamicVarArgsMethodInvokerMetaObject(parameter, this);
				}
			}
			
			public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
			{
				MethodInfo mi = (MethodInfo)args[0];
				result = new DynamicVarArgsMethodInvoker(mi, args.Skip(1).ToArray());
				return true;
			}
		}
		
		DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
		{
			return new VarArgsInvokerMetaObject(parameter, this);
		}
		
		private class VarArgsInvokerMetaObject : DynamicMetaObject
		{
			public new VarArgsInvoker Value{get; private set;}
			
			public VarArgsInvokerMetaObject(Expression expression, VarArgsInvoker value) : base(expression, BindingRestrictions.Empty, value)
			{
				Value = value;
			}
			
			public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
			{
			    BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);
			    int skip = Value.Method.GetParameters().Length;
			    if(Value.Target == null && !Value.Method.IsStatic) skip++;
			    Expression expr = Value.CreateInvoker(args.Skip(skip).Select(a => a.LimitType).ToArray());
			    expr = Expression.Block(Expression.Invoke(expr, args.Select(a => a.Expression)), Expression.Default(binder.ReturnType));
			    return new DynamicMetaObject(expr, restrictions);
			}
		}
	}
}
