/* Date: 13.7.2015, Time: 23:42 */
using System;
using System.Collections.Immutable;
using IllidanS4.SharpUtils.Accessing;

namespace IllidanS4.SharpUtils.Collections
{
	/// <summary>
	/// Exposes a reference to an immutable array as a mutable list.
	/// </summary>
	public class MutableArray<T> : MutableCollection<T, ImmutableArray<T>>
	{
		public MutableArray() : base(ImmutableArray.Create<T>())
		{
			
		}
		
		public MutableArray(int size) : base(ImmutableArray.Create(new T[size]))
		{
			
		}
		
		public MutableArray(IReadWriteAccessor<ImmutableArray<T>> arr) : base(arr)
		{
			
		}
		
		public override int IndexOf(T item)
		{
			return List.IndexOf(item);
		}
		
		public override void Insert(int index, T item)
		{
			List = List.Insert(index, item);
		}
		
		public override void RemoveAt(int index)
		{
			List = List.RemoveAt(index);
		}
		
		public override T this[int index] {
			get{
				return List[index];
			}
			set{
				List = List.SetItem(index, value);
			}
		}
		
		public override void Add(T item)
		{
			List = List.Add(item);
		}
		
		public override void Clear()
		{
			List = List.Clear();
		}
		
		public override void CopyTo(T[] array, int arrayIndex)
		{
			List.CopyTo(array, arrayIndex);
		}
		
		public override bool Remove(T item)
		{
			var removed = List.Remove(item);
			if(removed == List) return false;
			List = removed;
			return true;
		}
		
		public override bool Contains(T item)
		{
			return List.Contains(item);
		}
	}
}
