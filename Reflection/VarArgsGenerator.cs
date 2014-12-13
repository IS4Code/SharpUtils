using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection
{
	/// <summary>
	/// From this class, you can generate methods which invoke varargs methods.
	/// </summary>
	public static class VarArgsGenerator
	{
		public delegate void VarArgsAction<in TArgs>(params TArgs[] args);
		public delegate TRet VarArgsFunction<in TArgs, out TRet>(params TArgs[] args);
		
		[CLSCompliant(false)]
		public unsafe delegate void VarArgsActionUnsafe(int argc, void* args);
		[CLSCompliant(false)]
		public unsafe delegate TRet VarArgsFunctionUnsafe<out TRet>(int argc, void* args);
		
		public static int MaximumArgumentCount = 16;
		
		private static Delegate GenerateWrapperBase(MethodInfo varargMethod, object target, Type delegateType, Type[] parameterTypes, Type returnType, bool typedref, Type varargType, int addArgs, bool asref)
		{
			if(varargMethod == null) throw new ArgumentNullException("method");
			if(!varargMethod.IsStatic && target == null) throw new ArgumentNullException("target", "Target can be NULL only if method is static.");
			if(varargMethod.IsStatic && target != null) throw new ArgumentException("Target must be NULL if method is static.", "target");
			if(varargMethod.ReturnType != returnType) throw new ArgumentException("Method's return type does not match type argument.", "returnType");
			
			Type[] realParameterTypes = parameterTypes;
			if(target!=null)
			{
				realParameterTypes = new Type[parameterTypes.Length+1];
				realParameterTypes[0] = target.GetType();
				parameterTypes.CopyTo(realParameterTypes, 1);
				addArgs++;
			}
			
			DynamicMethod method = new DynamicMethod(
				varargMethod.Name,
				returnType,
				realParameterTypes,
				varargMethod.Module,
				true
			);
			
			
			ILGenerator il = method.GetILGenerator();
			int loadArray;
			int loadCount;
			if(!typedref)
			{
				loadCount = -1;
				loadArray = addArgs;
			}else{
				loadCount = 0+addArgs;
				loadArray = 1+addArgs;
			}
			Label[] brchs = new Label[MaximumArgumentCount];
			for(int i = 0; i < MaximumArgumentCount; i++)
			{
				brchs[i] = il.DefineLabel();
			}
			
			for(int i = 0; i < addArgs; i++)
			{
				il.EmitLdarg(i);
			}
			if(!typedref)
			{
				il.EmitLdarg(loadArray);
				il.Emit(OpCodes.Ldlen);
				il.Emit(OpCodes.Conv_I4);
			}else{
				il.EmitLdarg(loadCount);
			}
			il.Emit(OpCodes.Switch, brchs);
			//default
			il.ThrowException(TypeOf<ArgumentOutOfRangeException>.TypeID);
			
			Type argsType = varargType;
			if(!typedref && asref)
			{
				argsType = varargType.MakeByRefType();
			}
			for(int i = 0; i < MaximumArgumentCount; i++)
			{
				il.MarkLabel(brchs[i]); //case i
				for(int j = 0; j < i; j++)
				{
					il.EmitLdarg(loadArray);
					if(!typedref)
					{
						il.Emit(OpCodes.Ldc_I4, j);
						il.Emit(OpCodes.Conv_I);
						il.Emit(asref?OpCodes.Ldelema:OpCodes.Ldelem, varargType);
					}else{
						il.Emit(OpCodes.Ldc_I4, j);
						il.Emit(OpCodes.Conv_I);
						il.Emit(OpCodes.Sizeof, varargType);
						il.Emit(OpCodes.Mul_Ovf);
						il.Emit(OpCodes.Add);
						il.Emit(OpCodes.Ldobj, varargType);
					}
				}
				il.EmitCall(OpCodes.Call, varargMethod, new Type[i].Populate(argsType));
				il.Emit(OpCodes.Ret);
			}
			return method.CreateDelegate(delegateType, target);
		}
		
		public static Delegate GenerateWrapper(MethodInfo varargMethod, Type delegateType, object target = null, bool asref = true)
		{
			MethodInfo invoke = delegateType.GetMethod("Invoke");
			var args = invoke.GetParameters();
			Type varargType = args.Last().ParameterType;
			var parameterTypes = args.Take(args.Length-1).Select(p => p.ParameterType).Concat(new[]{varargType}).ToArray();
			if(varargType.HasElementType) varargType = varargType.GetElementType();
			return GenerateWrapperBase(varargMethod, target, delegateType, parameterTypes, invoke.ReturnType, false, varargType, args.Length-1, asref);
		}
		
		public static TDelegate GenerateWrapper<TDelegate>(MethodInfo varargMethod, object target = null, bool asref = true)
		{
			return (TDelegate)(object)GenerateWrapper(varargMethod, TypeOf<TDelegate>.TypeID, target, asref);
		}
		
		public static VarArgsAction<T> GenerateActionWrapper<T>(MethodInfo varargMethod, object target = null, bool asref = true)
		{
			return (VarArgsAction<T>)GenerateWrapperBase(varargMethod, target, TypeOf<VarArgsAction<T>>.TypeID, new[]{typeof(T[])}, typeof(void), false, TypeOf<T>.TypeID, 0, asref);
		}
		
		public static VarArgsFunction<T, TRet> GenerateFunctionWrapper<T, TRet>(MethodInfo varargMethod, object target = null, bool asref = true)
		{
			return (VarArgsFunction<T, TRet>)GenerateWrapperBase(varargMethod, target, TypeOf<VarArgsFunction<T, TRet>>.TypeID, new[]{typeof(T[])}, TypeOf<TRet>.TypeID, false, TypeOf<T>.TypeID, 0, asref);
		}
		
		public static Delegate GenerateWrapperUnsafe(MethodInfo varargMethod, Type delegateType, object target = null)
		{
			//return (VarArgsActionUnsafe)GenerateWrapperBase(varargMethod, target, TypeOf<VarArgsActionUnsafe>.TypeID, new[]{TypeOf<int>.TypeID, VoidPtrType}, typeof(void), true, TypedReferenceType, 0, false);
			MethodInfo invoke = delegateType.GetMethod("Invoke");
			var args = invoke.GetParameters();
			var parameterTypes = args.Take(args.Length-2).Select(p => p.ParameterType).Concat(new[]{TypeOf<int>.TypeID, VoidPtrType}).ToArray();
			return GenerateWrapperBase(varargMethod, target, delegateType, parameterTypes, invoke.ReturnType, true, TypedReferenceType, args.Length-2, false);
		}
		
		public static TDelegate GenerateWrapperUnsafe<TDelegate>(MethodInfo varargMethod, object target = null)
		{
			return (TDelegate)(object)GenerateWrapperUnsafe(varargMethod, TypeOf<TDelegate>.TypeID, target);
		}
		
		private static readonly Type TypedReferenceType = typeof(TypedReference);
		private static readonly Type VoidPtrType = typeof(void*);
		[CLSCompliant(false)]
		public static VarArgsActionUnsafe GenerateActionWrapperUnsafe(MethodInfo varargMethod, object target = null)
		{
			return (VarArgsActionUnsafe)GenerateWrapperBase(varargMethod, target, TypeOf<VarArgsActionUnsafe>.TypeID, new[]{TypeOf<int>.TypeID, VoidPtrType}, typeof(void), true, TypedReferenceType, 0, false);
		}
		
		[CLSCompliant(false)]
		public static VarArgsFunctionUnsafe<TRet> GenerateFunctionWrapperUnsafe<TRet>(MethodInfo varargMethod, object target = null)
		{
			return (VarArgsFunctionUnsafe<TRet>)GenerateWrapperBase(varargMethod, target, TypeOf<VarArgsFunctionUnsafe<TRet>>.TypeID, new[]{TypeOf<int>.TypeID, VoidPtrType}, TypeOf<TRet>.TypeID, true, TypedReferenceType, 0, false);
		}
		
		public static TDelegate GenerateFixedWrapper<TDelegate>(MethodInfo varargMethod, object target = null) where TDelegate : class
		{
			return (TDelegate)(object)GenerateFixedWrapper(TypeOf<TDelegate>.TypeID, varargMethod, target);
		}
		
		public static Delegate GenerateFixedWrapper(Type tDelegate, MethodInfo varargMethod, object target = null)
		{
			if(!TypeOf<Delegate>.TypeID.IsAssignableFrom(tDelegate)) throw new ArgumentException("Argument must derive from System.Delegate.", "tDelegate");
			if(varargMethod == null) throw new ArgumentNullException("varargMethod");
			if(!varargMethod.IsStatic && target == null) throw new ArgumentNullException("target", "Target can be NULL only if method is static.");
			if(varargMethod.IsStatic && target != null) throw new ArgumentException("Target must be NULL if method is static.", "target");
			
			MethodInfo invokeMethod = tDelegate.GetMethod("Invoke");
			if(varargMethod.ReturnType != invokeMethod.ReturnType)
			{
				throw new ArgumentException("Method return type does not match.", "tDelegate");
			}
			Type[] parameterTypes = new Type[invokeMethod.GetParameters().Length];
			Type[] realParameterTypes = parameterTypes;
			if(target!=null)
			{
				realParameterTypes = new Type[invokeMethod.GetParameters().Length+1];
				realParameterTypes[0] = target.GetType();
			}
			foreach(var pair in invokeMethod.GetParameters().PairEnumerate())
			{
				parameterTypes[pair.Key] = pair.Value.ParameterType;
				if(target!=null)realParameterTypes[pair.Key+1] = pair.Value.ParameterType;
			}
			DynamicMethod method = new DynamicMethod(
				varargMethod.Name+parameterTypes.Length,
				invokeMethod.ReturnType,
				realParameterTypes,
				varargMethod.Module,
				true
			);
			if(target==null)
			{
				foreach(var pair in invokeMethod.GetParameters().PairEnumerate())
				{
					method.DefineParameter(pair.Key+1, pair.Value.Attributes, pair.Value.Name);
				}
			}else{
				foreach(var pair in invokeMethod.GetParameters().PairEnumerate())
				{
					method.DefineParameter(pair.Key+2, pair.Value.Attributes, pair.Value.Name);
				}
			}
			
			ILGenerator il = method.GetILGenerator();
			if(target == null)
			{
				for(int i = 0; i < parameterTypes.Length; i++)
				{
					il.Emit(OpCodes.Ldarg, i);
				}
			}else{
				il.Emit(OpCodes.Ldarg_0);
				for(int i = 1; i <= parameterTypes.Length; i++)
				{
					il.Emit(OpCodes.Ldarg, i);
				}
			}
			il.EmitCall(OpCodes.Call, varargMethod, parameterTypes);
			il.Emit(OpCodes.Ret);
			return method.CreateDelegate(tDelegate, target);
		}
	}
}