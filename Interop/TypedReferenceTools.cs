/* Date: 13.11.2014, Time: 22:04 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Metadata;
using IllidanS4.SharpUtils.Reflection;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils.Interop
{
	public static class TypedReferenceTools
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
		
		/// <summary>
		/// Changes the type information stored in the TypedReference object.
		/// </summary>
		/// <param name="target">Pointer to the typed reference.</param>
		/// <param name="newType">The new type to be set.</param>
		[CLSCompliant(false)]
		public static unsafe void ChangeType(TypedReference* target, Type newType)
		{
			((IntPtr*)target)[1] = newType.TypeHandle.Value;
		}
		
		public static unsafe void ChangeType([Boxed(typeof(TypedReference))]ValueType target, Type newType)
		{
			TypedReference* ptr = (TypedReference*)UnsafeTools.GetDataPointer(target);
			ChangeType(ptr, newType);
		}
		
		[CLSCompliant(false)]
		public static Type GetType(this TypedReference target)
		{
			return __reftype(target);
		}
		
		public static Type GetType([Boxed(typeof(TypedReference))]ValueType target)
		{
			return __reftype((TypedReference)target);
		}
		
		[CLSCompliant(false)]
		public static unsafe IntPtr ToPointer(this TypedReference target)
		{
			return ((IntPtr*)(&target))[0];
		}
		
		public static IntPtr GetReferencePointer([Boxed(typeof(TypedReference))]ValueType target)
		{
			return ToPointer((TypedReference)target);
		}
		
		[CLSCompliant(false)]
		public static unsafe void ChangePointer(TypedReference* target, IntPtr ptr)
		{
			((IntPtr*)target)[0] = ptr;
		}
		
		public static unsafe void ChangePointer([Boxed(typeof(TypedReference))]ValueType target, IntPtr ptr)
		{
			TypedReference* trptr = (TypedReference*)UnsafeTools.GetDataPointer(target);
			ChangePointer(trptr, ptr);
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
		
		public static void SetValue([Boxed(typeof(TypedReference))]ValueType target, object value)
		{
			SetValue((TypedReference)target, value);
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
		
		public static void SetValue([Boxed(typeof(TypedReference))]ValueType target, [Boxed(typeof(TypedReference))]ValueType value)
		{
			SetValue((TypedReference)target, (TypedReference)value);
		}
		
		[CLSCompliant(false)]
		public static void SetValue<T>(this TypedReference target, T value)
		{
			__refvalue(target, T) = value;
		}
		
		public static void SetValue<T>([Boxed(typeof(TypedReference))]ValueType target, T value)
		{
			SetValue<T>((TypedReference)target, value);
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
		public static object GetValue(this TypedReference target)
		{
			return TypedReference.ToObject(target);
		}
		
		public static object GetValue([Boxed(typeof(TypedReference))]ValueType target)
		{
			return GetValue((TypedReference)target);
		}
		
		[CLSCompliant(false)]
		public static T GetValue<T>(this TypedReference target)
		{
			return __refvalue(target, T);
		}
		
		public static T GetValue<T>([Boxed(typeof(TypedReference))]ValueType target)
		{
			return GetValue<T>((TypedReference)target);
		}
		
		[CLSCompliant(false)]
		public static unsafe void GetTypedReference<T>(ref T reference, [Out]TypedReference* tr)
		{
			*tr = __makeref(reference);
		}
		
		[return: Boxed(typeof(TypedReference))]
		public static ValueType GetTypedReference<T>(ref T reference)
		{
			TypedReference tr = __makeref(reference);
			return UnsafeTools.Box(tr);
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
			IntPtr* a = ((IntPtr*)&tr);
			IntPtr* b = ((IntPtr*)&other);
			return a[0] == b[0] && a[1] == b[1];
		}
		
		public static bool Equals([Boxed(typeof(TypedReference))]ValueType tr, [Boxed(typeof(TypedReference))]ValueType other)
		{
			return Equals((TypedReference)tr, (TypedReference)other);
		}
		
		[CLSCompliant(false)]
		public static bool IsEmpty(this TypedReference tr)
		{
			return __reftype(tr) == null;
		}
		
		public static bool IsEmpty([Boxed(typeof(TypedReference))]ValueType tr)
		{
			return IsEmpty((TypedReference)tr);
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
			IntPtr[] flds = new IntPtr[fields.Length];
			Type lastType = target.GetType();
			for(int i = 0; i < fields.Length; i++)
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
		
		[return: Boxed(typeof(TypedReference))]
		public unsafe static ValueType MakeTypedReference(object target, params FieldInfo[] fields)
		{
			TypedReference tr;
			MakeTypedReference(&tr, target, fields);
			return UnsafeTools.Box(tr);
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
	}
}
