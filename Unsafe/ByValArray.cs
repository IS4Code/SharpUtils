/* Date: 18.8.2017, Time: 13:57 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.Unsafe
{
	/// <summary>
	/// A dynamically sized array with value type semantics.
	/// </summary>
	public struct ByValArray<T> : IList<T>, IList
	{
		T[] data;
		
		public ByValArray(int length)
		{
			data = new T[length];
		}
		
		private void Update()
		{
			if(data == null) data = new T[0];
			data = ReferenceStorage.FindInstance(ref this, data);
		}
		
		public int Length{
			get{
				Update();
				return data.Length;
			}
		}
		
		int ICollection<T>.Count{
			get{
				return Length;
			}
		}
		
		int ICollection.Count{
			get{
				return Length;
			}
		}
		
		public T this[int index]
		{
			get{
				Update();
				return data[index];
			}
			set{
				Update();
				data[index] = value;
			}
		}
		
		public bool IsReadOnly{
			get{
				return false;
			}
		}
		
		int IList<T>.IndexOf(T item)
		{
			Update();
			return ((IList<T>)data).IndexOf(item);
		}
		
		void IList<T>.Insert(int index, T item)
		{
			Update();
			((IList<T>)data).Insert(index, item);
		}
		
		void IList<T>.RemoveAt(int index)
		{
			Update();
			((IList<T>)data).RemoveAt(index);
		}
		
		void IList.RemoveAt(int index)
		{
			Update();
			((IList)data).RemoveAt(index);
		}
		
		void ICollection<T>.Add(T item)
		{
			Update();
			((ICollection<T>)data).Add(item);
		}
		
		void ICollection<T>.Clear()
		{
			Update();
			((ICollection<T>)data).Clear();
		}
		
		void IList.Clear()
		{
			Update();
			((IList)data).Clear();
		}
		
		bool ICollection<T>.Contains(T item)
		{
			Update();
			return ((IList<T>)data).Contains(item);
		}
		
		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			Update();
			((IList<T>)data).CopyTo(array, arrayIndex);
		}
		
		bool ICollection<T>.Remove(T item)
		{
			Update();
			return ((ICollection<T>)data).Remove(item);
		}
		
		public IEnumerator<T> GetEnumerator()
		{
			Update();
			return ((IEnumerable<T>)data).GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			Update();
			return ((IEnumerable)data).GetEnumerator();
		}
		
		object IList.this[int index]
		{
			get{
				Update();
				return ((IList)data)[index];
			}
			set{
				Update();
				((IList)data)[index] = value;
			}
		}
		
		bool IList.IsFixedSize{
			get{
				Update();
				return ((IList)data).IsFixedSize;
			}
		}
		
		object ICollection.SyncRoot{
			get{
				return null;
			}
		}
		
		bool ICollection.IsSynchronized{
			get{
				return false;
			}
		}
		
		int IList.Add(object value)
		{
			Update();
			return ((IList)data).Add(value);
		}
		
		bool IList.Contains(object value)
		{
			Update();
			return ((IList)data).Contains(value);
		}
		
		int IList.IndexOf(object value)
		{
			Update();
			return ((IList)data).IndexOf(value);
		}
		
		void IList.Insert(int index, object value)
		{
			Update();
			((IList)data).Insert(index, value);
		}
		
		void IList.Remove(object value)
		{
			Update();
			((IList)data).Remove(value);
		}
		
		void ICollection.CopyTo(Array array, int index)
		{
			Update();
			((ICollection)data).CopyTo(array, index);
		}
	}
}
