using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using IllidanS4.SharpUtils.Interop;
using IllidanS4.SharpUtils.Metadata;

namespace IllidanS4.SharpUtils.Unsafe
{
	/// <summary>
	/// Many methods in this class operate directly with object memory layout. Use at your own risk.
	/// </summary>
	public static class UnsafeTools
	{
		private static readonly Type thisType = typeof(UnsafeTools);
		
		public static int SizeOf(Type type)
		{
			if(!type.IsValueType) return IntPtr.Size;//throw new ArgumentException(Extensions.GetResourceString("Argument_NeedStructWithNoRefs"));
			return (int)_SizeOf(type);
		}
		
		public static int UnmanagedSizeOf(Type t)
		{
			return UnmanagedSizeOf(t, true);
		}
		
		public static int UnmanagedSizeOf(Type t, bool throwIfNotMarshalable)
		{
			return _UnmanagedSizeOf(t, throwIfNotMarshalable);
		}
		
		public static int DefinedSizeOf(Type t)
		{
			StructLayoutAttribute attr = t.StructLayoutAttribute;
			if(attr == null)
			{
				throw new ArgumentException("Type does not have StructLayoutAttribute attribute.", "t");
			}
			return attr.Size;
		}
		
		public static int DynamicSizeOf(Type t)
		{
			return (int)typeof(SizeOfClass<>).MakeGenericType(t).GetField("Size").GetValue(null);
		}
		
		public static int DynamicSizeOf<T>()
		{
			return SizeOfClass<T>.Size;
		}
		
		private static class SizeOfClass<T>
		{
			public static readonly int Size;
			
			static SizeOfClass()
			{
				DynamicMethod method = new DynamicMethod("sizeof", TypeOf<int>.TypeID, Type.EmptyTypes);
				var il = method.GetILGenerator();
				il.Emit(OpCodes.Sizeof, TypeOf<T>.TypeID);
				il.Emit(OpCodes.Ret);
				Size = (int)method.Invoke(null, null);
			}
		}
		
		public static int BaseInstanceSizeOf(Type t)
		{
			return Marshal.ReadInt32(t.TypeHandle.Value, 4);
		}
		
		//Marshal.SizeOfType
		private static readonly Func<Type,uint> _SizeOf;
		
		//Marshal.SizeOfHelper
		private static readonly Func<Type,bool,int> _UnmanagedSizeOf;
		
		static UnsafeTools()
		{
			MethodInfo mi = typeof(Marshal).GetMethod("SizeOfType", BindingFlags.Static | BindingFlags.NonPublic, null, new[]{TypeOf<Type>.TypeID}, null);
			_SizeOf = (Func<Type,uint>)Delegate.CreateDelegate(TypeOf<Func<Type,uint>>.TypeID, null, mi);
			mi = typeof(Marshal).GetMethod("SizeOfHelper", BindingFlags.Static | BindingFlags.NonPublic, null, new[]{TypeOf<Type>.TypeID, TypeOf<bool>.TypeID}, null);
			_UnmanagedSizeOf = (Func<Type,bool,int>)Delegate.CreateDelegate(TypeOf<Func<Type,bool,int>>.TypeID, null, mi);
		}
		
		private static readonly Func<object,IntPtr> GetPtr = CreateUnsafeCast<object,IntPtr>();
		private static readonly Func<IntPtr,object> GetO = CreateUnsafeCast<IntPtr,object>();
		
		public static Func<TFrom,TTo> CreateUnsafeCast<TFrom,TTo>()
		{
			var dm = new DynamicMethod("Cast", TypeOf<TTo>.TypeID, new[]{TypeOf<TFrom>.TypeID}, thisType, true);
			var il = dm.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ret);
			return (Func<TFrom,TTo>)dm.CreateDelegate(TypeOf<Func<TFrom,TTo>>.TypeID);
		}
		
		public static unsafe IntPtr GetDataPointer(ValueType obj)
		{
			TypedReference tr;
			TypedReferenceTools.MakeTypedReference(&tr, obj);
			return tr.ToPointer();
		}
		
		/// <summary>
		/// Converts an object reference to a pointer.
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
		
		public static void GetReference<T>(Pointer<T> ptr, Reference.RefAction<T> act) where T : struct
		{
			GetReference<T>(ptr.ToIntPtr(), act);
		}
		
		public static TRet GetReference<T, TRet>(Pointer<T> ptr, Reference.RefFunc<T, TRet> func) where T : struct
		{
			return GetReference<T, TRet>(ptr.ToIntPtr(), func);
		}
		
		public static void GetReference<T>(IntPtr ptr, Reference.RefAction<T> act) where T : struct
		{
			Reference.CacheHelper<T>.FromPtr(ptr, act);
		}
		
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
		
		public static TUnion CreateUnion<TUnion>() where TUnion : IMemoryUnion
		{
			return UnionCache<TUnion>.CreateNew();
		}
		
		private struct NullableContainer<T> where T : struct
		{
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