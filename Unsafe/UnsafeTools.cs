using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using IllidanS4.SharpUtils.Interop;
using IllidanS4.SharpUtils.Metadata;
using IllidanS4.SharpUtils.Reflection.Linq;

namespace IllidanS4.SharpUtils.Unsafe
{
	/// <summary>
	/// Many methods in this class operate directly with object memory layout. Use at your own risk.
	/// </summary>
	public static class UnsafeTools
	{
		/// <summary>
		/// Returns the CLR-size of a value.
		/// The result should be consistent with <see cref="DynamicSizeOf"/>.
		/// </summary>
		/// <param name="type">The type of the value.</param>
		/// <returns>The size of the value.</returns>
		public static int SizeOf(Type type)
		{
			if(!type.IsValueType) return IntPtr.Size;//throw new ArgumentException(Extensions.GetResourceString("Argument_NeedStructWithNoRefs"));
			return (int)_SizeOf(type);
		}
		
		/// <summary>
		/// Returns the size of a marshalled type corresponding to a CLR type.
		/// </summary>
		/// <param name="t">The CLR type.</param>
		/// <returns>The unmanaged size.</returns>
		public static int UnmanagedSizeOf(Type t)
		{
			return UnmanagedSizeOf(t, true);
		}
		
		/// <summary>
		/// Returns the size of a marshalled type corresponding to a CLR type.
		/// </summary>
		/// <param name="t">The CLR type.</param>
		/// <param name="throwIfNotMarshalable">If true, an exception is thrown if the type cannot be marshalled.</param>
		/// <returns>The unmanaged size.</returns>
		public static int UnmanagedSizeOf(Type t, bool throwIfNotMarshalable)
		{
			return _UnmanagedSizeOf(t, throwIfNotMarshalable);
		}
		
		/// <summary>
		/// Returns the size defined in the type's metadata.
		/// </summary>
		/// <param name="t">The type.</param>
		/// <returns>The defined size.</returns>
		public static int DefinedSizeOf(Type t)
		{
			StructLayoutAttribute attr = t.StructLayoutAttribute;
			if(attr == null)
			{
				throw new ArgumentException("Type does not have StructLayoutAttribute attribute.", "t");
			}
			return attr.Size;
		}
		
		/// <summary>
		/// Returns the size of a value, returned by "sizeof" IL instruction.
		/// The result should be consistent with <see cref="SizeOf"/>.
		/// </summary>
		/// <param name="t">The type of the value.</param>
		/// <returns>The size of the value.</returns>
		public static int DynamicSizeOf(Type t)
		{
			return (int)typeof(SizeOfClass<>).MakeGenericType(t).GetField("Size").GetValue(null);
		}
		
		/// <summary>
		/// Returns the size of a value, returned by "sizeof" IL instruction.
		/// The result should be consistent with <see cref="SizeOf"/>.
		/// </summary>
		/// <returns>The size of the value.</returns>
		public static int DynamicSizeOf<T>()
		{
			return SizeOfClass<T>.Size;
		}
		
		private static class SizeOfClass<T>
		{
			public static readonly int Size = LinqEmit.CreateDynamicMethod<Func<int>>(
				new Instruction(OpCodes.Sizeof, TypeOf<T>.TypeID),
				OpCodes.Ret
			).Invoke();
		}
		
		/// <summary>
		/// Returns the minimum size of an instance of a specific type. The type is boxed for value types.
		/// </summary>
		/// <param name="t">The type of the instance.</param>
		/// <returns>The size of the instance.</returns>
		public static int BaseInstanceSizeOf(Type t)
		{
			return Marshal.ReadInt32(t.TypeHandle.Value, 4);
		}
		
		private static readonly Func<Type,uint> _SizeOf = Hacks.GetInvoker<Func<Type,uint>>(typeof(Marshal), "SizeOfType", false);
		
		private static readonly Func<Type,bool,int> _UnmanagedSizeOf = Hacks.GetInvoker<Func<Type,bool,int>>(typeof(Marshal), "SizeOfHelper", false);
		
		
		/*private static readonly Func<object,IntPtr> GetPtr = CreateUnsafeCast<object,IntPtr>();
		private static readonly Func<IntPtr,object> GetO = CreateUnsafeCast<IntPtr,object>();
		
		public static Func<TFrom,TTo> CreateUnsafeCast<TFrom,TTo>()
		{
			var dm = new DynamicMethod("Cast", TypeOf<TTo>.TypeID, new[]{TypeOf<TFrom>.TypeID}, thisType, true);
			var il = dm.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ret);
			return (Func<TFrom,TTo>)dm.CreateDelegate(TypeOf<Func<TFrom,TTo>>.TypeID);
		}*/
		
		private static readonly Func<object,IntPtr> GetPtr = LinqEmit.CreateDynamicMethod<Func<object,IntPtr>>(
			//return (IntPtr)(void*)obj
			OpCodes.Ldarg_0,
			OpCodes.Conv_I,
			new Instruction(OpCodes.Newobj, typeof(IntPtr).GetConstructor(new[]{typeof(void*)})),
			OpCodes.Ret
		);
		
		private static readonly Func<IntPtr,object> GetO = LinqEmit.CreateDynamicMethod<Func<IntPtr,object>>(
			//return (object)(void*)ptr
			new Instruction(OpCodes.Ldarga_S, 0),
			new Instruction(OpCodes.Call, typeof(IntPtr).GetMethod("ToPointer")),
			OpCodes.Conv_U,
			new Instruction(OpCodes.Castclass, typeof(object)),
			OpCodes.Ret
		);
		
		/// <summary>
		/// Returns the internal pointer to a boxed instance's value. DOESN'T pin the reference.
		/// </summary>
		/// <param name="obj">The boxed instance.</param>
		/// <returns>The internal pointer.</returns>
		public static unsafe IntPtr GetDataPointer(ValueType obj)
		{
			TypedReference tr;
			TypedReferenceTools.MakeTypedReference(&tr, obj);
			return tr.ToPointer();
		}
		
		/// <summary>
		/// Converts an object reference to a pointer. DOESN'T pin the reference.
		/// </summary>
		/// <param name="o">The object which's pointer to obtain.</param>
		/// <returns>The address to the object's data (beginning with a type reference).</returns>
		public static IntPtr GetAddress(object o)
		{
			return GetPtr(o);
		}
		
		/// <summary>
		/// Converts a pointer back into an object reference.
		/// </summary>
		/// <param name="address">The pointer to the object.</param>
		/// <returns>The object reference.</returns>
		public static object GetObject(IntPtr address)
		{
			return GetO(address);
		}
		
		/// <summary>
		/// Converts a pointer back into an object reference.
		/// </summary>
		/// <param name="address">The pointer to the object.</param>
		/// <returns>The object reference.</returns>
		[CLSCompliant(false)]
		public static unsafe object GetObject(void* address)
		{
			return GetO((IntPtr)address);
		}
		
		/// <summary>
		/// Converts a pointer to a reference.
		/// </summary>
		/// <param name="ptr">The pointer to convert.</param>
		/// <param name="act">The action to receive the reference.</param>
		public static void GetReference<T>(Pointer<T> ptr, Reference.RefAction<T> act) where T : struct
		{
			GetReference<T>(ptr.ToIntPtr(), act);
		}
		
		/// <summary>
		/// Converts a pointer to a reference.
		/// </summary>
		/// <param name="ptr">The pointer to convert.</param>
		/// <param name="func">The function to receive the reference.</param>
		/// <returns>The value returned by <paramref name="func"/></returns>
		public static TRet GetReference<T, TRet>(Pointer<T> ptr, Reference.RefFunc<T, TRet> func) where T : struct
		{
			return GetReference<T, TRet>(ptr.ToIntPtr(), func);
		}
		
		/// <summary>
		/// Converts a pointer to a reference.
		/// </summary>
		/// <param name="ptr">The pointer to convert.</param>
		/// <param name="act">The action to receive the reference.</param>
		public static void GetReference<T>(IntPtr ptr, Reference.RefAction<T> act) where T : struct
		{
			Reference.CacheHelper<T>.FromPtr(ptr, act);
		}
		
		/// <summary>
		/// Converts a pointer to a reference.
		/// </summary>
		/// <param name="ptr">The pointer to convert.</param>
		/// <param name="func">The function to receive the reference.</param>
		/// <returns>The value returned by <paramref name="func"/></returns>
		public static TRet GetReference<T, TRet>(IntPtr ptr, Reference.RefFunc<T, TRet> func) where T : struct
		{
			return Reference.CacheHelper<T>.WithRet<TRet>.FromPtr(ptr, func);
		}
		
		public static void GetPointer<T>(out T reference, Action<IntPtr> act) where T : struct
		{
			GetPointer<T>(out reference, ptr => act(ptr.ToIntPtr()));
		}
		
		public static TRet GetPointer<T, TRet>(out T reference, Func<IntPtr, TRet> func) where T : struct
		{
			return GetPointer<T, TRet>(out reference, ptr => func(ptr.ToIntPtr()));
		}
		
		public static unsafe void GetPointer<T>(out T reference, Action<Pointer<T>> act) where T : struct
		{
			Reference.CacheHelper<T>.ToPtr(out reference, ptr => act(new Pointer<T>(ptr)));
		}
		
		public static unsafe TRet GetPointer<T, TRet>(out T reference, Func<Pointer<T>, TRet> func) where T : struct
		{
			return Reference.CacheHelper<T>.WithRet<TRet>.ToPtr(out reference, ptr => func(new Pointer<T>(ptr)));
		}
		
		[CLSCompliant(false)]
		public static void GetPointer<T>(TypedReference tr, Action<IntPtr> act) where T : struct
		{
			GetPointer<T>(tr, ptr => act(ptr.ToIntPtr()));
		}
		
		[CLSCompliant(false)]
		public static TRet GetPointer<T, TRet>(TypedReference tr, Func<IntPtr, TRet> func) where T : struct
		{
			return GetPointer<T, TRet>(tr, ptr => func(ptr.ToIntPtr()));
		}
		
		[CLSCompliant(false)]
		public static void GetPointer<T>(TypedReference tr, Action<Pointer<T>> act) where T : struct
		{
			tr.AsRef((ref T r)=>GetPointer<T>(out r, act));
		}
		
		[CLSCompliant(false)]
		public static TRet GetPointer<T, TRet>(TypedReference tr, Func<Pointer<T>, TRet> func) where T : struct
		{
			return tr.AsRef((ref T r)=>GetPointer<T, TRet>(out r, func));
		}
		// I wonder if it's necessary not to return the actual pointer, since these functions
		// don't attempt to pin the reference at all. I am not sure if the CLR would move it.
		
		/// <summary>
		/// Changes the type of a <see cref="SafeReference"/> to a new one.
		/// </summary>
		/// <param name="target">The target reference.</param>
		/// <param name="newType">The new type of the reference.</param>
		public static unsafe void ChangeType(SafeReference target, Type newType)
		{
			target.ChangeType(newType);
		}
		
		/// <summary>
		/// Returns the address of a <see cref="SafeReference"/>.
		/// </summary>
		/// <param name="target">The target reference.</param>
		/// <returns>The address of the reference.</returns>
		public static IntPtr GetAddress(SafeReference target)
		{
			return target.GetAddress();
		}
		
		/// <summary>
		/// Changes the address of a <see cref="SafeReference"/>.
		/// </summary>
		/// <param name="target">The target reference.</param>
		/// <param name="addr">The new address.</param>
		public static void ChangeAddress(SafeReference target, IntPtr addr)
		{
			target.ChangeAddress(addr);
		}
		
		
		// Boxing these non-nullable types is actually safer now thanks to GetUninitializedObject.
		
		/// <summary>
		/// Boxes a <see cref="System.TypedReference"/>.
		/// </summary>
		/// <param name="arg">The value to box.</param>
		/// <returns>The boxed value.</returns>
		[CLSCompliant(false)]
		[return: Boxed(typeof(TypedReference))]
		public static unsafe ValueType Box(TypedReference arg)
		{
			ValueType empty = (ValueType)FormatterServices.GetUninitializedObject(typeof(TypedReference));
			TypedReference innerRef; //TypedReference to a TypedReference...
			TypedReferenceTools.MakeTypedReference(&innerRef, empty);
			__refvalue(innerRef, TypedReference) = arg;
			return empty;
		}
		
		/// <summary>
		/// Boxes a <see cref="System.RuntimeArgumentHandle"/>.
		/// </summary>
		/// <param name="arg">The value to box.</param>
		/// <returns>The boxed value.</returns>
		[return: Boxed(typeof(RuntimeArgumentHandle))]
		public static unsafe ValueType Box(RuntimeArgumentHandle arg)
		{
			ValueType empty = (ValueType)FormatterServices.GetUninitializedObject(typeof(RuntimeArgumentHandle));
			TypedReference innerRef;
			TypedReferenceTools.MakeTypedReference(&innerRef, empty);
			__refvalue(innerRef, RuntimeArgumentHandle) = arg;
			return empty;
		}
		
		/// <summary>
		/// Boxes a <see cref="System.ArgIterator"/>.
		/// </summary>
		/// <param name="arg">The value to box.</param>
		/// <returns>The boxed value.</returns>
		[return: Boxed(typeof(ArgIterator))]
		public static unsafe ValueType Box(ArgIterator arg)
		{
			ValueType empty = (ValueType)FormatterServices.GetUninitializedObject(typeof(ArgIterator));
			TypedReference innerRef;
			TypedReferenceTools.MakeTypedReference(&innerRef, empty);
			__refvalue(innerRef, ArgIterator) = arg;
			return empty;
		}
		
		/// <summary>
		/// Boxes a nullable value to an object of the nullable type.
		/// </summary>
		/// <param name="nullable">The nullable value to box.</param>
		/// <returns>The boxed value.</returns>
		[return: Boxed(typeof(Nullable<>))]
		public static unsafe ValueType Box<T>(T? nullable) where T : struct
		{
			ValueType scam = new NullableContainer<T>(nullable);
			TypedReference tr = __makeref(scam);
			IntPtr typehandle = TypeOf<Nullable<T>>.TypeID.TypeHandle.Value;
			IntPtr* trstruct = (IntPtr*)&tr;
			**((IntPtr**)trstruct[0]) = typehandle;
			return scam;
		}
		
		/// <summary>
		/// Boxes a structure.
		/// </summary>
		/// <param name="arg">The value to box.</param>
		/// <returns>The boxed value.</returns>
		public static ValueType Box<T>(T arg) where T : struct
		{
			return arg;
		}
		
		#region CreateUnion
		public static TUnion CreateUnion<TUnion>() where TUnion : IMemoryUnion
		{
			return UnionCache<TUnion>.CreateNew();
		}
		
		public static IMemoryUnion<T1, T2> CreateUnion<T1, T2>()
		{
			return UnionCache<IMemoryUnion<T1, T2>>.CreateNew();
		}
		
		public static IMemoryUnion<T1, T2, T3> CreateUnion<T1, T2, T3>()
		{
			return UnionCache<IMemoryUnion<T1, T2, T3>>.CreateNew();
		}
		
		public static IMemoryUnion<T1, T2, T3, T4> CreateUnion<T1, T2, T3, T4>()
		{
			return UnionCache<IMemoryUnion<T1, T2, T3, T4>>.CreateNew();
		}
		
		public static IMemoryUnion<T1, T2, T3, T4, T5> CreateUnion<T1, T2, T3, T4, T5>()
		{
			return UnionCache<IMemoryUnion<T1, T2, T3, T4, T5>>.CreateNew();
		}
		
		public static IMemoryUnion<T1, T2, T3, T4, T5, T6> CreateUnion<T1, T2, T3, T4, T5, T6>()
		{
			return UnionCache<IMemoryUnion<T1, T2, T3, T4, T5, T6>>.CreateNew();
		}
		
		public static IMemoryUnion<T1, T2, T3, T4, T5, T6, T7> CreateUnion<T1, T2, T3, T4, T5, T6, T7>()
		{
			return UnionCache<IMemoryUnion<T1, T2, T3, T4, T5, T6, T7>>.CreateNew();
		}
		
		public static IMemoryUnion<T1, T2, T3, T4, T5, T6, T7, T8> CreateUnion<T1, T2, T3, T4, T5, T6, T7, T8>()
		{
			return UnionCache<IMemoryUnion<T1, T2, T3, T4, T5, T6, T7, T8>>.CreateNew();
		}
		#endregion
		
		private struct NullableContainer<T> where T : struct
		{
			//It's layout should be indistinguishable from Nullable<T>.
			public readonly T? Value;
			
			public NullableContainer(T? nullable)
			{
				Value = nullable;
			}
		}
		
		private static class UnionCache<TUnion> where TUnion : IMemoryUnion
		{
			public static readonly Type UnionType;
			
			static UnionCache()
			{
				Type infType = TypeOf<TUnion>.TypeID;
				TypeBuilder tb = Resources.DefineDynamicType(TypeAttributes.Sealed | TypeAttributes.Public | TypeAttributes.ExplicitLayout);
				tb.SetParent(TypeOf<ValueType>.TypeID);
				tb.AddInterfaceImplementation(infType);
				ImplementInterfaces(tb, infType);
				ImplementDataType(tb);
				ImplementSize(tb);
				UnionType = tb.CreateType();
			}
			
			private static void ImplementDataType(TypeBuilder tb)
			{
				var mb = tb.DefineMethod("get_DataType", MethodAttributes.Public | MethodAttributes.Virtual, TypeOf<Type>.TypeID, null);
				var il = mb.GetILGenerator();
				il.Emit(OpCodes.Ldtoken, tb);
				il.Emit(OpCodes.Call, TypeOf<Type>.TypeID.GetMethod("GetTypeFromHandle"));
				il.Emit(OpCodes.Ret);
				tb.DefineMethodOverride(mb, TypeOf<IMemoryUnion>.TypeID.GetProperty("DataType").GetMethod);
			}
			
			private static void ImplementSize(TypeBuilder tb)
			{
				var mb = tb.DefineMethod("get_Size", MethodAttributes.Public | MethodAttributes.Virtual, TypeOf<int>.TypeID, null);
				var il = mb.GetILGenerator();
				il.Emit(OpCodes.Sizeof, tb);
				il.Emit(OpCodes.Ret);
				tb.DefineMethodOverride(mb, TypeOf<IMemoryUnion>.TypeID.GetProperty("Size").GetMethod);
			}
			
			private static void ImplementInterfaces(TypeBuilder tb, Type type)
			{
				foreach(var pi in type.GetProperties())
				{
					var pt = pi.PropertyType;
					if(TypeOf<IMemoryUnion>.TypeID.IsAssignableFrom(pt))
					{
						try{
							pt = (Type)typeof(UnionCache<>).MakeGenericType(pt).GetField("UnionType").GetValue(null);
						}catch(TargetInvocationException e)
						{
							Exception e2 = e.InnerException;
							if(e2 is TypeInitializationException)
							{
								throw e2.InnerException;
							}else{
								throw e2;
							}
						}
					}
					FieldBuilder fb = tb.DefineField(pi.Name+"@"+type, pt, FieldAttributes.Public);
					fb.SetOffset(0);
					ILGenerator il;
					MethodBuilder get = tb.DefineMethod("get_"+fb.Name, MethodAttributes.Public | MethodAttributes.Virtual, pi.PropertyType, Type.EmptyTypes);
					MethodBuilder set = tb.DefineMethod("set_"+fb.Name, MethodAttributes.Public | MethodAttributes.Virtual, null, new[]{pi.PropertyType});
					il = get.GetILGenerator();
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldfld, fb);
					if(pt != pi.PropertyType)
					{
						il.Emit(OpCodes.Box, pi.PropertyType);
					}
					il.Emit(OpCodes.Ret);
					il = set.GetILGenerator();
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldarg_1);
					if(pt != pi.PropertyType)
					{
						il.Emit(OpCodes.Unbox_Any);
					}
					il.Emit(OpCodes.Stfld, fb);
					il.Emit(OpCodes.Ret);
					tb.DefineMethodOverride(get, pi.GetMethod);
					tb.DefineMethodOverride(set, pi.SetMethod);
				}
				foreach(var infType in type.GetInterfaces())
				{
					if(infType != TypeOf<IMemoryUnion>.TypeID)
					{
						ImplementInterfaces(tb, infType);
					}
				}
			}
			
			public static TUnion CreateNew()
			{
				return (TUnion)Activator.CreateInstance(UnionType);
			}
		}
	}
}