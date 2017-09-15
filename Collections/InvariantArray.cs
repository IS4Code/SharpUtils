/* Date: 20.6.2015, Time: 17:31 */
using System;
using System.Collections.Generic;
using System.Linq;
using IllidanS4.SharpUtils.Accessing;

namespace IllidanS4.SharpUtils.Collections
{
	/// <summary>
	/// Because arrays are covariant, type checks on accessing elements may slow the program a bit. This array has about 50 % faster element assignments.
	/// </summary>
	/// <remarks>
	/// Because array covariance doesn't apply to arrays of value types, this solution creates an array of container value type.
	/// </remarks>
	public struct InvariantArray<T> : IList<T>, IEquatable<InvariantArray<T>>, IIndexRefReference<int, T> where T : class
	{
		static readonly EqualityComparer<T> Comparer = EqualityComparer<T>.Default;
		readonly Element[] array;
		
		/// <summary>
		/// Gets or sets an element in the array.
		/// </summary>
		public T this[int index]{
			get{
				return array[index].Value;
			}
			set{
				array[index].Value = value;
			}
		}
		
		/// <summary>
		/// Gets or sets an element in the array.
		/// </summary>
		public T this[long index]{
			get{
				return array[index].Value;
			}
			set{
				array[index].Value = value;
			}
		}
		
		/// <summary>
		/// Creates a new invariant array from its size.
		/// </summary>
		public InvariantArray(int size)
		{
			array = new Element[size];
		}
		
		/// <summary>
		/// Creates a new invariant array from its size.
		/// </summary>
		public InvariantArray(long size)
		{
			array = new Element[size];
		}
		
		/// <summary>
		/// Determines whether two invariant arrays are identical.
		/// </summary>
		public override bool Equals(object obj)
		{
			if(obj is InvariantArray<T>)
			{
				return Equals((InvariantArray<T>)obj);
			}else{
				return false;
			}
		}
		
		/// <summary>
		/// Determines whether two invariant arrays are identical.
		/// </summary>
		public bool Equals(InvariantArray<T> other)
		{
			return array.Equals(other.array);
		}
		
		/// <summary>
		/// Returns the hash code of this object.
		/// </summary>
		public override int GetHashCode()
		{
			return array.GetHashCode();
		}
		
		/// <summary>
		/// Return true if two invariant arrays are identical.
		/// </summary>
		public static bool operator ==(InvariantArray<T> left, InvariantArray<T> right)
		{
			return left.Equals(right);
		}
		
		/// <summary>
		/// Return true if two invariant arrays are not identical.
		/// </summary>
		public static bool operator !=(InvariantArray<T> left, InvariantArray<T> right)
		{
			return !left.Equals(right);
		}
		
		int ICollection<T>.Count{
			get{
				return array.Length;
			}
		}
		
		/// <summary>
		/// Gets the length of the array.
		/// </summary>
		public int Length{
			get{
				return array.Length;
			}
		}
		
		/// <summary>
		/// Gets the long length of the array.
		/// </summary>
		public long LongLength{
			get{
				return array.LongLength;
			}
		}
		
		bool ICollection<T>.IsReadOnly{
			get{
				return array.IsReadOnly;
			}
		}
		
		public int IndexOf(T item)
		{
			for(int i = 0; i < array.Length; i++)
			{
				if(Comparer.Equals(array[i].Value, item))
				{
					return i;
				}
			}
			return -1;
		}
		
		void IList<T>.Insert(int index, T item)
		{
			throw new NotSupportedException();
		}
		
		void IList<T>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}
		
		void ICollection<T>.Add(T item)
		{
			throw new NotSupportedException();
		}
		
		void ICollection<T>.Clear()
		{
			throw new NotSupportedException();
		}
		
		bool ICollection<T>.Contains(T item)
		{
			return IndexOf(item) != -1;
		}
		
		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			for(int i = 0; i < this.array.Length; i++)
			{
				array[i+arrayIndex] = this.array[i].Value;
			}
		}
		
		bool ICollection<T>.Remove(T item)
		{
			throw new NotSupportedException();
		}
		
		/// <summary>
		/// Returns the enumerator of this array.
		/// </summary>
		public IEnumerator<T> GetEnumerator()
		{
			foreach(var e in array)
			{
				yield return e.Value;
			}
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		struct Element
		{
			public T Value;
			
			public Element(T value)
			{
				Value = value;
			}
		}
		
		/// <summary>
		/// Obtains a reference to an element in this array.
		/// </summary>
		public TRet GetReference<TRet>(int index, Reference.RefFunc<T, TRet> func)
		{
			return func(ref array[index].Value);
		}
		
		/// <summary>
		/// Obtains a reference to an element in this array.
		/// </summary>
		public TRet GetReference<TRet>(int index, Reference.OutFunc<T, TRet> func)
		{
			return func(out array[index].Value);
		}
		
		/// <summary>
		/// Obtains a reference to an element in this array.
		/// </summary>
		public TRet GetReference<TRet>(int index, Func<SafeReference, TRet> func)
		{
			return SafeReference.Create(ref array[index].Value, func);
		}
		
		/// <summary>
		/// Obtains a reference to an element in this array.
		/// </summary>
		public TRet GetReference<TRet>(object index, Func<SafeReference, TRet> func)
		{
			return GetReference<TRet>((int)index, func);
		}
	}
}
