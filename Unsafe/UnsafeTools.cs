using System;
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
		
		public static int BaseInstaceSizeOf(Type t)
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
		
		private struct NullableContainer<T> where T : struct
		{
			public readonly T? Value;
			
			public NullableContainer(T? nullable)
			{
				Value = nullable;
			}
		}
	}
}