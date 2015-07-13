/* Date: 3.4.2015, Time: 20:53 */
using System;
using System.Collections.Generic;
using System.Reflection;
using IllidanS4.SharpUtils.Interop;

namespace IllidanS4.SharpUtils.Accessing
{
	public static class Accessor
	{
		public static ArrayAccessor<T> Access<T>(this T[] array, int index)
		{
			return new ArrayAccessor<T>(array, index);
		}
		
		public static ReadListAccessor<T> Access<T>(this IList<T> list, int index)
		{
			if(list.IsReadOnly)
			{
				return new ReadListAccessor<T>(list, index);
			}else{
				return new ListAccessor<T>(list, index);
			}
		}
		
		public static ReadDictionaryAccessor<TKey, TValue> Access<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			if(dictionary.IsReadOnly)
			{
				return new ReadDictionaryAccessor<TKey, TValue>(dictionary, key);
			}else{
				return new DictionaryAccessor<TKey, TValue>(dictionary, key);
			}
		}
		
		/*public static ReferenceAccessor<T> Access<T>(ref T value)
		{
			return new ReferenceAccessor<T>(ref value);
		}*/
		
		public static IndexGetAccessor<TKey, TValue> Access<TKey, TValue>(this IIndexGet<TKey, TValue> indexable, TKey key)
		{
			return new IndexGetAccessor<TKey, TValue>(indexable, key);
		}
		
		public static IndexSetAccessor<TKey, TValue> Access<TKey, TValue>(this IIndexSet<TKey, TValue> indexable, TKey key)
		{
			return new IndexSetAccessor<TKey, TValue>(indexable, key);
		}
		
		public static IndexGetSetAccessor<TKey, TValue> Access<TKey, TValue>(this IIndexGetSet<TKey, TValue> indexable, TKey key)
		{
			return new IndexGetSetAccessor<TKey, TValue>(indexable, key);
		}
		
		public static ReadFieldAccessor<T> Access<T>(this FieldInfo field, object target)
		{
			if(field.IsInitOnly)
			{
				return new ReadFieldAccessor<T>(field, target);
			}else{
				return new FieldAccessor<T>(field, target);
			}
		}
		
		public static void GetReference<T>(this IRefReference<T> r, Reference.RefAction<T> act)
		{
			r.GetReference<Unit>((ref T rf)=>{act(ref rf); return 0;});
		}
		
		public static void GetReference<T>(this IOutReference<T> r, Reference.OutAction<T> act)
		{
			r.GetReference<Unit>((out T rf)=>{act(out rf); return 0;});
		}
		
		public static void GetReference(this ITypedReference tr, Action<SafeReference> act)
		{
			tr.GetReference<Unit>(r => {act(r); return 0;});
		}
	}
}
