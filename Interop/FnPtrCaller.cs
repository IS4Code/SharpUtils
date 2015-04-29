/* Date: 11.1.2015, Time: 12:38 */
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection;

namespace IllidanS4.SharpUtils.Interop
{
	/// <summary>
	/// Allows dynamic calling of function pointers with the specified signature. TDelegate must be a delegate type with IntPtr, pointing to the function, or MethodBase, as the first parameter.
	/// </summary>
	public static class FnPtrCaller<TDelegate> where TDelegate : class
	{
		public static readonly TDelegate Invoke;
		
		static FnPtrCaller()
		{
			Type tDel = TypeOf<TDelegate>.TypeID;
			var msig = ReflectionTools.GetDelegateSignature(tDel);
			var ptypes = msig.ParameterTypes;
			bool expr = false;
			if(ptypes[0] == TypeOf<MethodBase>.TypeID)
			{
				expr = true;
				ptypes[0] = TypeOf<IntPtr>.TypeID;
			}
			if(ptypes[0] != TypeOf<IntPtr>.TypeID)
				throw new ArgumentException("Delegate must have IntPtr or MethodBase as the first parameter.");
			DynamicMethod dyn = new DynamicMethod("Invoker", msig.ReturnType, ptypes, typeof(FnPtrCaller<TDelegate>), true);
			var il = dyn.GetILGenerator();
			int vastart = -1;
			int i = 1;
			foreach(Type t in ptypes.Skip(1))
			{
				if(t == TypeOf<Sentinel>.TypeID)
				{
					vastart = i;
					i++;
					continue;
				}
				il.EmitLdarg(i++);
			}
			il.Emit(OpCodes.Ldarg_0);
			Type[] newptypes;
			Type[] opttypes = msig.OptionalParameterTypes;
			if(vastart == -1)
			{
				newptypes = ptypes.Skip(1).ToArray();
			}else{
				newptypes = ptypes.Skip(1).Take(vastart-1).ToArray();
				opttypes = ptypes.Skip(vastart+1).ToArray();
			}
			if(msig.IsUnmanaged)
			{
				il.EmitCalli(OpCodes.Calli, msig.UnmanagedCallingConvention, msig.ReturnType, newptypes);
			}else{
				il.EmitCalli(OpCodes.Calli, vastart == -1 ? msig.CallingConvention : CallingConventions.VarArgs, msig.ReturnType, newptypes, opttypes);
			}
			il.Emit(OpCodes.Ret);
			
			if(!expr)
			{
				Invoke = (TDelegate)(object)dyn.CreateDelegate(tDel);
			}else{
				ParameterExpression[] pargs = ptypes.Select(p => Expression.Parameter(p)).ToArray();
				pargs[0] = Expression.Parameter(TypeOf<MethodBase>.TypeID);
				Expression exp = Expression.Call(
					dyn,
					Extensions.Once<Expression>(
						Expression.Call(
							Expression.Property(pargs[0], "MethodHandle"),
							"GetFunctionPointer", null
						)
					).Concat(pargs.Skip(1))
				);
				Invoke = Expression.Lambda<TDelegate>(exp, pargs).Compile();
			}
		}
	}
}
