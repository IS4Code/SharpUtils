using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using IllidanS4.SharpUtils.Interop;

namespace IllidanS4.SharpUtils.Unsafe
{
	/// <summary>
	/// Many methods in this class operate directly with memory. Use at your own risk.
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
			DynamicMethod dm;
			ILGenerator il;
			
			/*dm = new DynamicMethod("TRBoxer", TypeOf<object>.TypeID, new[]{typeof(TypedReference)}, thisType, true);
			il = dm.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Box, TypeOf<TR>.TypeID);
			il.Emit(OpCodes.Dup);
			il.Emit(OpCodes.Ldc_I8, (long)typeof(TypedReference).TypeHandle.Value);
			il.Emit(OpCodes.Conv_I);
			il.Emit(OpCodes.Stobj, TypeOf<IntPtr>.TypeID);
			il.Emit(OpCodes.Ret);
			TRBoxer = (TRBox)dm.CreateDelegate(TypeOf<TRBox>.TypeID);
			
			dm = new DynamicMethod("RAHBoxer", TypeOf<object>.TypeID, new[]{typeof(RuntimeArgumentHandle)}, thisType, true);
			il = dm.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Box, TypeOf<RAH>.TypeID);
			il.Emit(OpCodes.Dup);
			il.Emit(OpCodes.Ldc_I8, (long)typeof(RuntimeArgumentHandle).TypeHandle.Value);
			il.Emit(OpCodes.Conv_I);
			il.Emit(OpCodes.Stobj, TypeOf<IntPtr>.TypeID);
			il.Emit(OpCodes.Ret);
			RAHBoxer = (RAHBox)dm.CreateDelegate(TypeOf<RAHBox>.TypeID);
			
			dm = new DynamicMethod("AIBoxer", TypeOf<object>.TypeID, new[]{typeof(ArgIterator)}, thisType, true);
			il = dm.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Box, TypeOf<AI>.TypeID);
			il.Emit(OpCodes.Dup);
			il.Emit(OpCodes.Ldc_I8, (long)typeof(ArgIterator).TypeHandle.Value);
			il.Emit(OpCodes.Conv_I);
			il.Emit(OpCodes.Stobj, TypeOf<IntPtr>.TypeID);
			il.Emit(OpCodes.Ret);
			AIBoxer = (AIBox)dm.CreateDelegate(TypeOf<AIBox>.TypeID);*/
			
			dm = new DynamicMethod("GetO", TypeOf<object>.TypeID, new[]{TypeOf<IntPtr>.TypeID}, thisType, true);
			il = dm.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ret);
			GetO = dm.CreateDelegate(TypeOf<GetO_d>.TypeID) as GetO_d;
			
			dm = new DynamicMethod("GetPtr", TypeOf<IntPtr>.TypeID, new[]{TypeOf<object>.TypeID}, thisType, true);
			il = dm.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ret);
			GetPtr = dm.CreateDelegate(TypeOf<GetPtr_d>.TypeID) as GetPtr_d;
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
		
		private delegate IntPtr GetPtr_d(object obj);
		private delegate object GetO_d(IntPtr ptr);
		private static readonly GetPtr_d GetPtr;
		private static readonly GetO_d GetO;
		
		/// <summary>
		/// Boxes a <see cref="System.TypedReference"/>.
		/// </summary>
		/// <param name="arg">The value to box.</param>
		/// <returns>The boxed value.</returns>
		[CLSCompliant(false)]
		public static unsafe ValueType Box(TypedReference arg)
		{
			ValueType empty = (ValueType)FormatterServices.GetUninitializedObject(typeof(TypedReference));
			TypedReference innerRef; //TypedReference to a TypedReference...
			InteropTools.MakeTypedReference(&innerRef, empty);
			__refvalue(innerRef, TypedReference) = arg;
			return empty;
		}
		
		/// <summary>
		/// Boxes a <see cref="System.RuntimeArgumentHandle"/>.
		/// </summary>
		/// <param name="arg">The value to box.</param>
		/// <returns>The boxed value.</returns>
		public static unsafe ValueType Box(RuntimeArgumentHandle arg)
		{
			ValueType empty = (ValueType)FormatterServices.GetUninitializedObject(typeof(RuntimeArgumentHandle));
			TypedReference innerRef;
			InteropTools.MakeTypedReference(&innerRef, empty);
			__refvalue(innerRef, RuntimeArgumentHandle) = arg;
			return empty;
		}
		
		/// <summary>
		/// Boxes a <see cref="System.ArgIterator"/>.
		/// </summary>
		/// <param name="arg">The value to box.</param>
		/// <returns>The boxed value.</returns>
		public static unsafe ValueType Box(ArgIterator arg)
		{
			ValueType empty = (ValueType)FormatterServices.GetUninitializedObject(typeof(ArgIterator));
			TypedReference innerRef;
			InteropTools.MakeTypedReference(&innerRef, empty);
			__refvalue(innerRef, ArgIterator) = arg;
			return empty;
		}
		
		/// <summary>
		/// Boxes a nullable value to an object of the nullable type.
		/// </summary>
		/// <param name="nullable">The nullable value to box.</param>
		/// <returns>The boxed value.</returns>
		public static unsafe ValueType Box<T>(T? nullable) where T : struct
		{
			ValueType scam = new N<T>(nullable);
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
		
		/*public static object Box(ValueType arg)
		{
			return arg;
		}*/
		
		/*private delegate object TRBox(TypedReference arg);
		private delegate object RAHBox(RuntimeArgumentHandle arg);
		private delegate object AIBox(ArgIterator arg);
		
		private static readonly TRBox TRBoxer;
		private static readonly RAHBox RAHBoxer;
		private static readonly AIBox AIBoxer;
		
		#pragma warning disable 169
		private struct TR
		{
	        private IntPtr Value;
	        private IntPtr Type;
		}
		
		private struct RAH
		{
        	private IntPtr m_ptr;
		}
		
		private struct AI
		{
	        private IntPtr ArgCookie;
	        private IntPtr ArgPtr;
	        private int RemainingArgs;
	        private IntPtr sigPtr;
	        private IntPtr sigPtrLen;
		}*/
		
		private struct N<T> where T : struct
		{
			public readonly bool hasValue;
			private readonly T value;
			
			public N(T? nullable) : this()
			{
				if(nullable.HasValue)
				{
					hasValue = true;
					value = nullable.Value;
				}
			}
		}
		//#pragma warning restore 169
	}
}