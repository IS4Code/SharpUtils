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
		
		/// <summary>
		/// Creates a new flexible array from its size.
		/// </summary>
		public FlexibleArray(int size)
		{
			arr = new T[size];
		}
		
		/// <summary>
		/// Creates a new flexible array from its size.
		/// </summary>
		public FlexibleArray(long size)
		{
			arr = new T[size];
		}
		
		/// <summary>
		/// Gets or sets an element in the array.
		/// </summary>
		public T this[int index]{
			get{
				return arr[index];
			}
			set{
				arr[index] = value;
			}
		}
		
		/// <summary>
		/// Gets or sets an element in the array.
		/// </summary>
		public T this[long index]{
			get{
				return arr[index];
			}
			set{
				arr[index] = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the length of the array.
		/// </summary>
		public int Length{
			get{
				return arr.Length;
			}
			set{
				Resize(value);
			}
		}
		
		/// <summary>
		/// Gets the long length of the array.
		/// </summary>
		public long LongLength{
			get{
				return arr.LongLength;
			}
		}
		
		/// <summary>
		/// Resizes the array to a new size.
		/// </summary>
		public void Resize(int newSize)
		{
			Array.Resize(ref arr, newSize);
		}
		
		int ICollection<T>.Count{
			get{
				return Length;
			}
		}
		
		bool ICollection<T>.IsReadOnly{
			get{
				return arr.IsReadOnly;
			}
		}
		
		int IList<T>.IndexOf(T item)
		{
			return ((IList<T>)arr).IndexOf(item);
		}
		
		void IList<T>.Insert(int index, T item)
		{
			((IList<T>)arr).Insert(index, item);
		}
		
		void IList<T>.RemoveAt(int index)
		{
			((IList<T>)arr).RemoveAt(index);
		}
		
		void ICollection<T>.Add(T item)
		{
			((IList<T>)arr).Add(item);
		}
		
		void ICollection<T>.Clear()
		{
			((IList<T>)arr).Clear();
		}
		
		bool ICollection<T>.Contains(T item)
		{
			return arr.Contains(item);
		}
		
		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			arr.CopyTo(array, arrayIndex);
		}
		
		bool ICollection<T>.Remove(T item)
		{
			return ((IList<T>)arr).Remove(item);
		}
		
		/// <summary>
		/// Returns the enumerator of this array.
		/// </summary>
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
