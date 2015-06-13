/* Date: 29.4.2015, Time: 23:02 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Metadata;

namespace IllidanS4.SharpUtils.Interop.Collections
{
	public class ReferenceList<T> : IList<T>, IIndexRefReferable<int, T>
	{
		private T[] _items;
		private int count;
		
		public ReferenceList(IEnumerable<T> values)
		{
			_items = values.ToArray();
			count = _items.Length;
		}
		
		public ReferenceList(int capacity)
		{
			_items = new T[capacity];
		}
		
		public T this[int index]
		{
			get{
				if(index < 0 || index >= count) throw new ArgumentOutOfRangeException("index");
				return _items[index];
			}
			set{
				if(index < 0 || index >= count) throw new ArgumentOutOfRangeException("index");
				_items[index] = value;
			}
		}
		
		public int Count{
			get{
				return count;
			}
		}
		
		public bool IsReadOnly{
			get{
				return false;
			}
		}
		
		public int IndexOf(T item)
		{
			for(int i = 0; i < count; i++)
			{
				if(_items[i].Equals(item)) return i;
			}
			return -1;
		}
		
		public void Insert(int index, T item)
		{
			if(count >= _items.Length)
			{
				Array.Resize(ref _items, _items.Length*2);
			}
			Array.Copy(_items, index, _items, index+1, count-index);
			_items[index] = item;
			count++;
		}
		
		public void RemoveAt(int index)
		{
			if(index < 0 || index >= count) throw new ArgumentOutOfRangeException("index");
			Array.Copy(_items, index+1, _items, index, count-index-1);
			count--;
		}
		
		public void Add(T item)
		{
			if(count >= _items.Length)
			{
				Array.Resize(ref _items, _items.Length*2);
			}
			_items[count] = item;
			count++;
		}
		
		public void Clear()
		{
			Array.Clear(_items, 0, _items.Length);
			count = 0;
		}
		
		public bool Contains(T item)
		{
			foreach(T val in this) if(val.Equals(item)) return true;
			return false;
		}
		
		public void CopyTo(T[] array, int arrayIndex)
		{
			for(int i = 0; i < count; i++)
			{
				array[arrayIndex+1] = _items[i];
			}
		}
		
		public bool Remove(T item)
		{
			for(int i = 0; i < count; i++)
			{
				if(_items[i].Equals(item))
				{
					RemoveAt(i);
					count--;
					return true;
				}
			}
			return false;
		}
		
		public IEnumerator<T> GetEnumerator()
		{
			for(int i = 0; i < count; i++)
			{
				yield return _items[i];
			}
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		
		public TRet GetReference<TRet>(int index, Reference.RefFunc<T, TRet> func)
		{
			if(index < 0 || index >= count) throw new ArgumentOutOfRangeException("index");
			return func(ref _items[index]);
		}
		
		public TRet GetReference<TRet>(int index, Reference.OutFunc<T, TRet> func)
		{
			if(index < 0 || index >= count) throw new ArgumentOutOfRangeException("index");
			return func(out _items[index]);
		}
		
		[CLSCompliant(false)]
		public TRet GetReference<TRet>(object index, TypedReferenceTools.TypedRefFunc<TRet> func)
		{
			int idx = (int)index;
			if(idx < 0 || idx >= count) throw new ArgumentOutOfRangeException("index");
			return func(__makeref(_items[idx]));
		}
	}
}
