/* Date: 13.11.2014, Time: 22:04 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace IllidanS4.SharpUtils.Interop
{
	[CLSCompliant(false)]
	public static class TypedReferenceConvert
	{	
		const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
		static readonly Type thisType = typeof(TypedReferenceConvert);
		
		delegate void TypedReferenceSet(TypedReference tr, object value);
		static readonly Type TypedReferenceSet_t = typeof(TypedReferenceSet);
		static readonly MethodInfo SetTypedReference_m = thisType.GetMethod("SetTypedReference", flags);
		static readonly ConcurrentDictionary<Type,TypedReferenceSet> setcache = new ConcurrentDictionary<Type,TypedReferenceSet>();
		
		delegate void TypedReferenceSetRef(TypedReference tr, TypedReference value);
		static readonly Type TypedReferenceSetRef_t = typeof(TypedReferenceSetRef);
		static readonly MethodInfo SetTypedReferenceRef_m = thisType.GetMethod("SetTypedReferenceRef", flags);
		static readonly ConcurrentDictionary<Type,TypedReferenceSetRef> setcacheref = new ConcurrentDictionary<Type,TypedReferenceSetRef>();
		
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
		
		public static void SetValue<T>(this TypedReference target, T value)
		{
			__refvalue(target, T) = value;
		}
		
		private static void SetTypedReference<T>(TypedReference target, object value)
		{
			__refvalue(target, T) = (T)value;
		}
		
		private static void SetTypedReferenceRef<T>(TypedReference target, TypedReference value)
		{
			__refvalue(target, T) = __refvalue(value, T);
		}
		
		public static object GetValue(this TypedReference target)
		{
			return TypedReference.ToObject(target);
		}
		
		public static T GetValue<T>(this TypedReference target)
		{
			return __refvalue(target, T);
		}
	}
}
