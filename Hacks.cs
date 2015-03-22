/* Date: 6.1.2015, Time: 20:42 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection;
using IllidanS4.SharpUtils.Reflection.Emit;

namespace IllidanS4.SharpUtils
{
	public sealed class Hacks : HacksBase<MulticastDelegate>
	{
		private Hacks(){}
	}
	
	public abstract class HacksBase<TDelegateBase> where TDelegateBase : class
	{
		internal HacksBase(){}
		
		const BindingFlags privflags = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
		
		public static TDelegate GenerateInvoker<TDelegate>(Type type, string method, bool? instance = null) where TDelegate : TDelegateBase
		{
			BindingFlags flags = privflags;
			if(instance.HasValue)
			{
				if(instance.Value)
				{
					flags &=~ BindingFlags.Static;
				}else{
					flags &=~ BindingFlags.Instance;
				}
			}
			MethodInfo mi = type.GetMethod(method, flags);
			Type delType = typeof(TDelegate);
			MethodSignature delSig = ReflectionTools.GetDelegateSignature(delType);
			Type[] tParams = delSig.ParameterTypes;
			Type retType = delSig.ReturnType;
			
			ParameterExpression[] parExp = tParams.Select(t => Expression.Parameter(t.UnderlyingSystemType)).ToArray();
			IEnumerable<Type> mParams = mi.GetParameters().Select(p => p.ParameterType);
			if(!mi.IsStatic)
			{
				mParams = Enumerable.Repeat(type, 1).Concat(mParams);
			}
			
			IEnumerable<Expression> parPass = parExp.Zip(mParams, ParamOrConvert);
			Expression inst;
			if(!mi.IsStatic)
			{
				inst = parExp.Take(1).First();
				parPass = parPass.Skip(1);
			}else{
				inst = null;
			}
			var pxas = parPass.ToArray();
			Expression exp = Expression.Call(inst, mi, parPass);
			CheckRetType(ref exp, retType);
			return Expression.Lambda<TDelegate>(exp, parExp).Compile();
		}
		
		public static TDelegate GenerateConstructor<TDelegate>(Type type, int ord) where TDelegate : TDelegateBase
		{
			ConstructorInfo ctor = type.GetConstructors(privflags)[ord];
			Type delType = typeof(TDelegate);
			MethodSignature delSig = ReflectionTools.GetDelegateSignature(delType);
			Type[] tParams = delSig.ParameterTypes;
			Type retType = delSig.ReturnType;
			
			ParameterExpression[] parExp = tParams.Select(t => Expression.Parameter(t.UnderlyingSystemType)).ToArray();
			IEnumerable<Type> mParams = ctor.GetParameters().Select(p => p.ParameterType);

			IEnumerable<Expression> parPass = parExp.Zip(mParams, ParamOrConvert);
			Expression exp = Expression.New(ctor, parPass);
			CheckRetType(ref exp, retType);
			return Expression.Lambda<TDelegate>(exp, parExp).Compile();
		}
		
		public static TDelegate GenerateFieldGetter<TDelegate>(Type type, string field) where TDelegate : TDelegateBase
		{
			Type delType = typeof(TDelegate);
			MethodSignature delSig = ReflectionTools.GetDelegateSignature(delType);
			Type[] tParams = delSig.ParameterTypes;
			Type retType = delSig.ReturnType;
			
			FieldInfo fi = type.GetField(field, privflags);
			Expression exp;
			ParameterExpression[] parExp;
			if(fi.IsStatic)
			{
				parExp = new ParameterExpression[0];
				exp = Expression.Field(null, fi);
			}else{
				ParameterExpression p1 = Expression.Parameter(tParams.Single());
				parExp = new ParameterExpression[]{p1};
				exp = Expression.Field(ParamOrConvert(p1, type), fi);
			}
			CheckRetType(ref exp, retType);
			return Expression.Lambda<TDelegate>(exp, parExp).Compile();
		}
		
		public static TDelegate GenerateFieldSetter<TDelegate>(Type type, params string[] fields) where TDelegate : TDelegateBase
		{
			Type delType = typeof(TDelegate);
			MethodSignature delSig = ReflectionTools.GetDelegateSignature(delType);
			Type[] tParams = delSig.ParameterTypes;
			
			FieldInfo[] fi = fields.Select(f => type.GetField(f, privflags)).ToArray();
			ParameterExpression[] parExp = tParams.Select(tp => Expression.Parameter(tp)).ToArray();
			Expression inst;
			int iadd;
			if(fi.All(f => f.IsStatic))
			{
				inst = null;
				iadd = 0;
			}else{
				inst = ParamOrConvert(parExp[0], type);
				iadd = 1;
			}
			Expression exp = Expression.Block(
				fi.Select((f,i) => Expression.Assign(Expression.Field(inst, f), ParamOrConvert(parExp[i+iadd], tParams[i+iadd])))
			);
			return Expression.Lambda<TDelegate>(exp, parExp).Compile();
		}
		
		private static Expression ParamOrConvert(ParameterExpression p, Type pType)
		{
			if(p.IsByRef ? p.Type.MakeByRefType() == pType : p.Type == pType)
			{
				return p;
			}else{
				return Expression.Convert(p, pType);
			}
		}
		
		private static void CheckRetType(ref Expression exp, Type retType)
		{
			if(exp.Type != retType)
			{
				exp = Expression.Convert(exp, retType);
			}
		}
	}
}
