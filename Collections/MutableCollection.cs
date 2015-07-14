/* Date: 13.7.2015, Time: 23:41 */
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using IllidanS4.SharpUtils.Accessing;

namespace IllidanS4.SharpUtils.Collections
{
	/// <summary>
	/// Exposes a reference to an immutable collection as a mutable one.
	/// </summary>
	public abstract class MutableCollection<T,TList> : IList<T> where TList : IImmutableList<T>
	{
		static readonly EqualityComparer<T> Comparer = EqualityComparer<T>.Default;
		readonly IReadWriteAccessor<TList> acc;
		public TList List{
			get{
				return acc.Item;
			}
			set{
				acc.Item = value;
			}
		}
		
		protected MutableCollection(TList list) : this(new AtomicContainer<TList>(list))
		{
			
		}
		
		public MutableCollection(IReadWriteAccessor<TList> list)
		{
			acc = list;
		}
		
		public virtual int IndexOf(T item)
		{
			var list = List;
			return list.IndexOf(item, 0, list.Count, Comparer);
		}
		public virtual void Insert(int index, T item)
		{
			List = (TList)List.Insert(index, item);
		}
		public virtual void RemoveAt(int index)
		{
			List = (TList)List.RemoveAt(index);
		}
		public virtual T this[int index] {
			get{
				return List[index];
			}
			set{
				List = (TList)List.SetItem(index, value);
			}
		}
		
		public virtual void Add(T item)
		{
			List = (TList)List.Add(item);
		}
		public virtual void Clear()
		{
			List = (TList)List.Clear();
		}
		
		public virtual bool Contains(T item)
		{
			return List.Contains(item, Comparer);
			/*foreach(var elem in List)
			{
				if(Comparer.Equals(elem, item)) return true;
			}
			return false;*/
		}
		
		public virtual void CopyTo(T[] array, int arrayIndex)
		{
			var list = List;
			for(int i = 0; i < list.Count; i++)
			{
				array[arrayIndex+i] = list[i];
			}
		}
		
		public virtual bool Remove(T item)
		{
			var list = List;
			var removed = (TList)list.Remove(item, Comparer);
			if(removed.Equals(list)) return false;
			List = removed;
			return true;
		}
		
		public virtual int Count{
			get{
				return List.Count;
			}
		}
		public virtual bool IsReadOnly{
			get{
				return false;
			}
		}
		
		public virtual IEnumerator<T> GetEnumerator()
		{
			return List.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
