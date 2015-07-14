/* Date: 14.7.2015, Time: 0:42 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IllidanS4.SharpUtils.Collections
{
	/// <summary>
	/// Represents an array which can change its size.
	/// </summary>
	public class FlexibleArray<T> : IList<T>
	{
		T[] arr;
		
		public FlexibleArray(int size)
		{
			arr = new T[size];
		}
		
		public FlexibleArray(long size)
		{
			arr = new T[size];
		}
		
		public T this[int index]{
			get{
				return arr[index];
			}
			set{
				arr[index] = value;
			}
		}
		
		public T this[long index]{
			get{
				return arr[index];
			}
			set{
				arr[index] = value;
			}
		}
		
		public int Length{
			get{
				return arr.Length;
			}
			set{
				Resize(value);
			}
		}
		
		public long LongLength{
			get{
				return arr.LongLength;
			}
		}
		
		public void Resize(int newSize)
		{
			Array.Resize(ref arr, newSize);
		}
		
		int ICollection<T>.Count{
			get{
				return Length;
			}
		}
		
		public bool IsReadOnly{
			get{
				return arr.IsReadOnly;
			}
		}
		
		public int IndexOf(T item)
		{
			return ((IList<T>)arr).IndexOf(item);
		}
		
		public void Insert(int index, T item)
		{
			((IList<T>)arr).Insert(index, item);
		}
		
		public void RemoveAt(int index)
		{
			((IList<T>)arr).RemoveAt(index);
		}
		
		public void Add(T item)
		{
			((IList<T>)arr).Add(item);
		}
		
		public void Clear()
		{
			((IList<T>)arr).Clear();
		}
		
		public bool Contains(T item)
		{
			return arr.Contains(item);
		}
		
		public void CopyTo(T[] array, int arrayIndex)
		{
			arr.CopyTo(array, arrayIndex);
		}
		
		public bool Remove(T item)
		{
			return ((IList<T>)arr).Remove(item);
		}
		
		public IEnumerator<T> GetEnumerator()
		{
			foreach(T item in arr) yield return item;
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
