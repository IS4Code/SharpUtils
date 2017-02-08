/* Date: 21.5.2015, Time: 14:08 */
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection;

namespace IllidanS4.SharpUtils.Proxies.Dynamic
{
	/// <summary>
	/// This class allows the construction of implementations of abstract types that pass calls to their methods to a <see cref="StaticAdapter"/>.
	/// </summary>
	public static class ImplementationProxy<T>
	{
		static readonly Type proxyType = typeof(T);
		public static readonly Type Implementation;
		static readonly Func<StaticAdapter,T> constructor;
		
		static readonly MethodInfo getadapter = typeof(IAdapterProxy).GetProperty("Adapter").GetGetMethod();
		static readonly MethodInfo adapterCall = typeof(AdapterTools).GetMethod("Call");
		static readonly MethodInfo methodFromHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new[]{typeof(RuntimeMethodHandle)});
		static readonly MethodInfo finalize = typeof(object).GetMethod("Finalize", BindingFlags.NonPublic | BindingFlags.Instance);
		
		static ImplementationProxy()
		{
			var impl = Resources.DefineDynamicType(TypeAttributes.Public);
			var saType = typeof(StaticAdapter);
			
			var adapterField = impl.DefineField("adapter", saType, FieldAttributes.Private);
			impl.SetParent(typeof(Type));
			impl.AddInterfaceImplementation(getadapter.DeclaringType);
			{
				var mb = impl.DefineMethod(getadapter.Name, MethodAttributes.Private | MethodAttributes.Virtual | MethodAttributes.Final, saType, Type.EmptyTypes);
				var il = mb.GetILGenerator();
				il.Emit(OpCodes.Ldfld, adapterField);
				il.Emit(OpCodes.Ret);
				impl.DefineMethodOverride(mb, getadapter);
			}
			foreach(var mi in proxyType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => !m.IsPrivate && !m.IsAssembly && m.IsVirtual && !m.IsFinal))
			{
				if(mi.GetBaseDefinition() == finalize || mi == finalize) continue;
				
				var args = mi.GetParameters().Select(p => p.ParameterType).ToArray();
				var mb = impl.DefineMethod(mi.Name, mi.Attributes & (~MethodAttributes.Abstract) | MethodAttributes.Final, mi.ReturnType, args);
				var il = mb.GetILGenerator();
				var locArr = il.DeclareLocal(TypeOf<object[]>.TypeID);
				il.Emit(OpCodes.Ldc_I4, args.Length);
				il.Emit(OpCodes.Newarr, locArr.LocalType.GetElementType());
				il.Emit(OpCodes.Stloc, locArr);
				for(int i = 0; i < args.Length; i++)
				{
					il.Emit(OpCodes.Ldloc, locArr);
					il.Emit(OpCodes.Ldc_I4, i);
					il.Emit(OpCodes.Conv_I);
					
					var arg = args[i];
					il.EmitLdarg(i+1);
					if(arg.IsByRef)
					{
						il.Emit(OpCodes.Ldobj, arg = arg.GetElementType());
					}
					if(arg.IsValueType)
					{
						il.Emit(OpCodes.Box, arg);
					}
					
					il.Emit(OpCodes.Stelem_Ref);
				}
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldfld, adapterField);
				il.Emit(OpCodes.Ldtoken, mi);
				il.Emit(OpCodes.Call, methodFromHandle);
				il.Emit(OpCodes.Ldloc, locArr);
				il.Emit(OpCodes.Call, adapterCall);
				for(int i = 0; i < args.Length; i++)
				{
					var arg = args[i];
					if(!arg.IsByRef) continue;
					arg = arg.GetElementType();
					
					il.EmitLdarg(i+1);
					
					il.Emit(OpCodes.Ldloc, locArr);
					il.Emit(OpCodes.Ldc_I4, i);
					il.Emit(OpCodes.Conv_I);
					
					il.Emit(OpCodes.Ldelem_Ref);
					il.Emit(OpCodes.Unbox_Any, arg);
					
					il.Emit(OpCodes.Stobj, arg);
				}
				if(mi.ReturnType != typeof(void))
					il.Emit(OpCodes.Castclass, mi.ReturnType);
				il.Emit(OpCodes.Ret);
				
				impl.DefineMethodOverride(mb, mi);
			}
			
			{
				var ctor = impl.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[]{typeof(StaticAdapter)});
				var il = ctor.GetILGenerator();
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Call, impl.BaseType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null));
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Stfld, adapterField);
				il.Emit(OpCodes.Ret);
			}
			
			Implementation = impl.CreateType();
			constructor = Hacks.GetConstructor<Func<StaticAdapter,T>>(Implementation, 0);
		}
		
		public static T GetProxy(StaticAdapter adapter)
		{
			if(!proxyType.IsAssignableFrom(adapter.ProxyType)) throw new ArgumentException("Wrong adapter type.", "adapter");
			
			return constructor(adapter);
		}
	}
}
