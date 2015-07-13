/* Date: 13.11.2014, Time: 22:04 */
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Metadata;
using IllidanS4.SharpUtils.Reflection;
using IllidanS4.SharpUtils.Reflection.Linq;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils.Interop
{
	public sealed class TypedReferenceTools : TypedReferenceToolsBase<Array>
	{
		
	}
	
	public abstract class TypedReferenceToolsBase<TArrayBase> where TArrayBase : class, ICloneable, IList, ICollection, IEnumerable, IStructuralComparable, IStructuralEquatable
	{	
		internal TypedReferenceToolsBase(){}
		
		const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
		
		[CLSCompliant(false)]
		public delegate void TypedRefAction(TypedReference tr);
		[CLSCompliant(false)]
		public delegate TRet TypedRefFunc<TRet>(TypedReference tr);
		
		/// <summary>
		/// Changes the type information stored in the TypedReference object.
		/// </summary>
		/// <param name="target">Pointer to the typed reference.</param>
		/// <param name="newType">The new type to be set.</param>
		[CLSCompliant(false)]
		public static unsafe void ChangeType(TypedReference* target, Type newType)
		{
			((TypedRef*)target)->Type = newType.TypeHandle.Value;
		}
		
		[CLSCompliant(false)]
		public static unsafe void ChangePointer(TypedReference* target, IntPtr ptr)
		{
			((TypedRef*)target)->Value = ptr;
		}
		
		private static void SetTypedReference<T>(TypedReference target, object value)
		{
			__refvalue(target, T) = (T)value;
		}
		
		private static void SetTypedReferenceRef<T>(TypedReference target, TypedReference value)
		{
			__refvalue(target, T) = __refvalue(value, T);
		}
		
		[CLSCompliant(false)]
		public static unsafe void GetTypedReference<T>(ref T reference, [Out]TypedReference* tr)
		{
			*tr = __makeref(reference);
		}
		
		[CLSCompliant(false)]
		public static void GetTypedReference<T>(ref T reference, TypedRefAction act)
		{
			act(__makeref(reference));
		}
		
		[CLSCompliant(false)]
		public static TRet GetTypedReference<T, TRet>(ref T reference, TypedRefFunc<TRet> func)
		{
			return func(__makeref(reference));
		}
		
		#region Field typedref
		private unsafe delegate void InternalMakeTypedReferenceDelegate(void* result, object target, IntPtr[] flds, Type lastFieldType);
		private static readonly InternalMakeTypedReferenceDelegate InternalMakeTypedReference = GetInternalMakeTypedReference();
		private static InternalMakeTypedReferenceDelegate GetInternalMakeTypedReference()
		{
			MethodInfo mi = typeof(TypedReference).GetMethod("InternalMakeTypedReference", flags);
			DynamicMethod method = new DynamicMethod("InternalMakeTypedReference", typeof(void), new[]{typeof(void*), typeof(object), typeof(IntPtr[]), typeof(Type)}, typeof(InteropTools).Module, true);
			var il = method.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Ldarg_3);
			il.Emit(OpCodes.Castclass, Types.RuntimeType);
			il.EmitCall(OpCodes.Call, mi, null);
			il.Emit(OpCodes.Ret);
			return (InternalMakeTypedReferenceDelegate)method.CreateDelegate(typeof(InternalMakeTypedReferenceDelegate));
		}
		
		[CLSCompliant(false)]
		public unsafe static void MakeTypedReference([Out]TypedReference* result, object target, params FieldInfo[] fields)
		{
			if(target == null)
			{
				MakeStaticTypedReference(result, fields);
				return;
			}
			IntPtr[] flds = new IntPtr[fields==null?0:fields.Length];
			Type lastType = target.GetType();
			for(int i = 0; i < flds.Length; i++)
			{
				var field = fields[i];
				if(field.IsStatic)
				{
					throw new ArgumentException("Field cannot be static.", "fields");
				}
				flds[i] = field.FieldHandle.Value;
				lastType = field.FieldType;
			}
			InternalMakeTypedReference(result, target, flds, lastType);
		}
		
		[CLSCompliant(false)]
		public unsafe static void MakeTypedReference(object target, FieldInfo[] fields, TypedRefAction act)
		{
			TypedReference tr;
			MakeTypedReference(&tr, target, fields);
			act(tr);
		}
		
		[CLSCompliant(false)]
		public unsafe static TRet MakeTypedReference<TRet>(object target, FieldInfo[] fields, TypedRefFunc<TRet> func)
		{
			TypedReference tr;
			MakeTypedReference(&tr, target, fields);
			return func(tr);
		}
		
		[CLSCompliant(false)]
		public unsafe static void MakeTypedReference(object target, TypedRefAction act, params FieldInfo[] fields)
		{
			TypedReference tr;
			MakeTypedReference(&tr, target, fields);
			act(tr);
		}
		
		[CLSCompliant(false)]
		public unsafe static TRet MakeTypedReference<TRet>(object target, TypedRefFunc<TRet> func, params FieldInfo[] fields)
		{
			TypedReference tr;
			MakeTypedReference(&tr, target, fields);
			return func(tr);
		}
		
		private unsafe delegate void FieldTypedRef(void* result);
		private unsafe static void MakeStaticTypedReference([Out]TypedReference* result, params FieldInfo[] fields)
		{
			DynamicMethod mb = new DynamicMethod("GetStaticTypedReference", typeof(void), new[]{typeof(void*)}, typeof(InteropTools).Module, true);
			var il = mb.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			Type ftype = null;
			foreach(var field in fields)
			{
				OpCode opcode;
				if(ftype == null)
				{
					opcode = OpCodes.Ldsflda;
					if(!field.IsStatic)
					{
						throw new ArgumentException("First field must be static.", "fields");
					}
				}else{
					opcode = OpCodes.Ldflda;
					if(field.IsStatic)
					{
						throw new ArgumentException("Next field cannot be static.", "fields");
					}
				}
				ftype = field.FieldType;
				il.Emit(opcode, field);
			}
			il.Emit(OpCodes.Mkrefany, ftype);
			il.Emit(OpCodes.Stobj, typeof(TypedReference));
			il.Emit(OpCodes.Ret);
			FieldTypedRef del = (FieldTypedRef)mb.CreateDelegate(typeof(FieldTypedRef));
			del.Invoke(result);
		}
		#endregion
		
		/// <summary>
		/// Obtains a reference to an element in an array.
		/// </summary>
		/// <param name="arr">The array where the element is located.</param>
		/// <param name="tr">The pointer where the reference will be stored.</param>
		/// <param name="indices">The indices of the element.</param>
		[CLSCompliant(false)]
		public static unsafe void ArrayAddress<TArray>(TArray arr, [Out]TypedReference* tr, params int[] indices) where TArray : TArrayBase
		{
			ArrayAddressCache<TArray>.ArrayAddress.Invoke(arr, tr, indices);
		}
		
		[CLSCompliant(false)]
		public static unsafe void ArrayAddress<TArray>(TArray arr, int[] indices, TypedRefAction act) where TArray : TArrayBase
		{
			TypedReference tr;
			ArrayAddress<TArray>(arr, &tr, indices);
			act(tr);
		}
		
		[CLSCompliant(false)]
		public static unsafe TRet ArrayAddress<TArray, TRet>(TArray arr, int[] indices, TypedRefFunc<TRet> func) where TArray : TArrayBase
		{
			TypedReference tr;
			ArrayAddress<TArray>(arr, &tr, indices);
			return func(tr);
		}
		
		private static unsafe void ArrayAddressInternal<TArray>(TArray arr, [Out]void* tr, params int[] indices) where TArray : TArrayBase
		{
			ArrayAddressCache<TArray>.ArrayAddress.Invoke(arr, tr, indices);
		}
		
		private static readonly MethodInfo method_ArrayAddressCache = typeof(TypedReferenceTools).GetMethod("ArrayAddressInternal", flags);
		
		[CLSCompliant(false)]
		public static unsafe void ArrayAddress(Array arr, [Out]TypedReference* tr, params int[] indices)
		{
			Type arrayType = arr.GetType();
			method_ArrayAddressCache.MakeGenericMethod(arrayType).Invoke(null, new object[]{arr, (IntPtr)tr, indices});
		}
		
		private static unsafe class ArrayAddressCache<TArray> where TArray : TArrayBase
		{
			public delegate void ArrayAddressDelegate(TArray arr, void* tr, params int[] indices);
			public static readonly ArrayAddressDelegate ArrayAddress;
			
			static ArrayAddressCache()
			{
				Type arrType = typeof(TArray);
				if(!arrType.IsArray)
				{
					throw new InvalidOperationException("TArray must be an array type.");
				}
				Type indexType = arrType.GetElementType();
				
				int rank = arrType.GetArrayRank();
				MethodInfo addrMethod = arrType.GetMethod("Address", BindingFlags.Public | BindingFlags.Instance);
				
				DynamicMethod dyn = new DynamicMethod("DynamicAddress", null, new[]{typeof(TArray), typeof(void*), typeof(int[])}, typeof(ArrayAddressCache<TArray>), true);
				var il = dyn.GetILGenerator();
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldarg_0);
				for(int i = 0; i < rank; i++)
				{
					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Ldc_I4, i);
					il.Emit(OpCodes.Ldelem, typeof(int));
				}
				il.Emit(OpCodes.Callvirt, addrMethod);
				il.Emit(OpCodes.Mkrefany, indexType);
				il.Emit(OpCodes.Stobj, typeof(TypedReference));
				il.Emit(OpCodes.Ret);
				ArrayAddress = (ArrayAddressDelegate)dyn.CreateDelegate(typeof(ArrayAddressDelegate));
			}
		}
	}

	/// <summary>
	/// Structure with layout equal to <see cref="TypedReference"/>.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct TypedRef : IEquatable<TypedRef>
	{
		public IntPtr Value;
		public IntPtr Type;
		
		public override bool Equals(object obj)
		{
			return (obj is TypedRef) && Equals((TypedRef)obj);
		}
		
		public bool Equals(TypedRef other)
		{
			return this.Value == other.Value && this.Type == other.Type;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * Value.GetHashCode();
				hashCode += 1000000009 * Type.GetHashCode();
			}
			return hashCode;
		}
		
		public static bool operator ==(TypedRef tr1, TypedRef tr2)
		{
			return tr1.Equals(tr2);
		}
		
		public static bool operator !=(TypedRef tr1, TypedRef tr2)
		{
			return !(tr1==tr2);
		}
	}
	
	public static class TypedReferenceToolsExtensions
	{
		const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
		static readonly Type thisType = typeof(TypedReferenceTools);
		
		delegate void TypedReferenceSet(TypedReference tr, object value);
		static readonly Type TypedReferenceSet_t = typeof(TypedReferenceSet);
		static readonly MethodInfo SetTypedReference_m = thisType.GetMethod("SetTypedReference", flags);
		static readonly ConcurrentDictionary<Type,TypedReferenceSet> setcache = new ConcurrentDictionary<Type,TypedReferenceSet>();
		
		delegate void TypedReferenceSetRef(TypedReference tr, TypedReference value);
		static readonly Type TypedReferenceSetRef_t = typeof(TypedReferenceSetRef);
		static readonly MethodInfo SetTypedReferenceRef_m = thisType.GetMethod("SetTypedReferenceRef", flags);
		static readonly ConcurrentDictionary<Type,TypedReferenceSetRef> setcacheref = new ConcurrentDictionary<Type,TypedReferenceSetRef>();
		
		
		[CLSCompliant(false)]
		public static Type GetType(this TypedReference target)
		{
			return __reftype(target);
		}
		
		[CLSCompliant(false)]
		public static unsafe IntPtr ToPointer(this TypedReference target)
		{
			return ((TypedRef*)(&target))->Value;
		}
		
		[CLSCompliant(false)]
		public static void SetValue(this TypedReference target, object value)
		{
			TypedReferenceSet set;
			Type t = __reftype(target);
			if(!setcache.TryGetValue(t, out set))
			{
				set = setcache[t] = (TypedReferenceSet)Delegate.CreateDelegate(TypedReferenceSet_t, SetTypedReference_m.MakeGenericMethod(t));
			}
			set(target, value);
		}
		
		[CLSCompliant(false)]
		public static void SetValue(this TypedReference target, TypedReference value)
		{
			TypedReferenceSetRef set;
			Type t = __reftype(target);
			if(!setcacheref.TryGetValue(t, out set))
			{
				set = setcacheref[t] = (TypedReferenceSetRef)Delegate.CreateDelegate(TypedReferenceSetRef_t, SetTypedReferenceRef_m.MakeGenericMethod(t));
			}
			set(target, value);
		}
		
		[CLSCompliant(false)]
		public static void SetValue<T>(this TypedReference target, T value)
		{
			__refvalue(target, T) = value;
		}
		
		[CLSCompliant(false)]
		public static object GetValue(this TypedReference target)
		{
			return TypedReference.ToObject(target);
		}
		
		[CLSCompliant(false)]
		public static T GetValue<T>(this TypedReference target)
		{
			return __refvalue(target, T);
		}
		
		/// <summary>
		/// Compares two typed references for equality.
		/// </summary>
		/// <param name="tr">The first typed reference.</param>
		/// <param name="other">The second typed reference.</param>
		/// <returns>true if they are equal (point to the same location).</returns>
		[CLSCompliant(false)]
		public unsafe static bool Equals(this TypedReference tr, TypedReference other)
		{
			var a = ((TypedRef*)&tr);
			var b = ((TypedRef*)&other);
			return a->Value == b->Value && a->Type == b->Type;
		}
		
		[CLSCompliant(false)]
		public unsafe static bool IsNull(this TypedReference tr)
		{
			var ptr = ((TypedRef*)&tr);
			return ptr->Value == IntPtr.Zero;
		}
		
		[CLSCompliant(false)]
		public unsafe static bool IsEmpty(this TypedReference tr)
		{
			var ptr = ((TypedRef*)&tr);
			return ptr->Value == IntPtr.Zero && ptr->Type == IntPtr.Zero;
		}
		
		[CLSCompliant(false)]
		public static void AsRef<T>(this TypedReference tr, Reference.RefAction<T> act)
		{
			ConvHelper<T>.Convert(tr, act);
		}
		
		[CLSCompliant(false)]
		public static TRet AsRef<T, TRet>(this TypedReference tr, Reference.RefFunc<T, TRet> act)
		{
			return ConvHelper<T>.WithRet<TRet>.Convert(tr, act);
		}
		
		[CLSCompliant(false)]
		public static void Pin(this TypedReference tr, TypedReferenceTools.TypedRefAction act)
		{
			PinHelper.Pin(tr, act);
		}
		
		[CLSCompliant(false)]
		public static TRet Pin<TRet>(this TypedReference tr, TypedReferenceTools.TypedRefFunc<TRet> func)
		{
			return PinHelper.WithRet<TRet>.Pin(tr, func);
		}
		
		private static class PinHelper
		{
			private static readonly MethodInfo Invoke = typeof(TypedReferenceTools.TypedRefAction).GetMethod("Invoke");
			
			public delegate void Del(TypedReference tr, TypedReferenceTools.TypedRefAction act);
			public static readonly Del Pin = LinqEmit.CreateDynamicMethod<Del>(
				Instruction.DeclareLocal(typeof(void).MakeByRefType(), true),
				new Instruction(OpCodes.Ldarga_S, 0),
				OpCodes.Ldind_I,
				OpCodes.Conv_U,
				OpCodes.Stloc_0,
				OpCodes.Ldarg_1,
				OpCodes.Ldloc_0,
				new Instruction(OpCodes.Callvirt, Invoke),
				OpCodes.Ret
			);
			
			public static class WithRet<TRet>
			{
				private static readonly MethodInfo Invoke = typeof(TypedReferenceTools.TypedRefFunc<TRet>).GetMethod("Invoke");
				
				public delegate TRet Del(TypedReference tr, TypedReferenceTools.TypedRefFunc<TRet> func);
				public static readonly Del Pin = LinqEmit.CreateDynamicMethod<Del>(
					Instruction.DeclareLocal(typeof(void).MakeByRefType(), true),
					new Instruction(OpCodes.Ldarga_S, 0),
					OpCodes.Ldind_I,
					OpCodes.Conv_U,
					OpCodes.Stloc_0,
					OpCodes.Ldarg_1,
					OpCodes.Ldloc_0,
					new Instruction(OpCodes.Callvirt, Invoke),
					OpCodes.Ret
				);
			}
		}
		
		private static class ConvHelper<T>
		{
			private static readonly MethodInfo Invoke = typeof(Reference.RefAction<T>).GetMethod("Invoke");
			
			public delegate void Del(TypedReference tr, Reference.RefAction<T> act);
			public static readonly Del Convert = LinqEmit.CreateDynamicMethod<Del>(
				OpCodes.Ldarg_1,
				OpCodes.Ldarg_0,
				new Instruction(OpCodes.Refanyval, typeof(T)),
				new Instruction(OpCodes.Callvirt, Invoke),
				OpCodes.Ret
			);
			
			public static class WithRet<TRet>
			{
				private static readonly MethodInfo Invoke = typeof(Reference.RefFunc<T, TRet>).GetMethod("Invoke");
			
				public delegate TRet Del(TypedReference tr, Reference.RefFunc<T, TRet> act);
				public static readonly Del Convert = LinqEmit.CreateDynamicMethod<Del>(
					OpCodes.Ldarg_1,
					OpCodes.Ldarg_0,
					new Instruction(OpCodes.Refanyval, typeof(T)),
					new Instruction(OpCodes.Callvirt, Invoke),
					OpCodes.Ret
				);
			}
		}
	}
}
