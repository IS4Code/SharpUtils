/* Date: 3.4.2015, Time: 21:15 */
using System;

namespace IllidanS4.SharpUtils.Accessing
{
	public class IndexGetAccessor<TKey, TValue> : BasicReadAccessor<TValue>
	{
		public IIndexGet<TKey, TValue> Indexable{get; private set;}
		public TKey Key{get; private set;}
		
		public IndexGetAccessor(IIndexGet<TKey, TValue> indexable, TKey key)
		{
			Indexable = indexable;
			Key = key;
		}
		
		public override TValue Item{
			get{
				return Indexable[Key];
			}
		}
	}
	
	public class IndexSetAccessor<TKey, TValue> : BasicWriteAccessor<TValue>
	{
		public IIndexSet<TKey, TValue> Indexable{get; private set;}
		public TKey Key{get; private set;}
		
		public IndexSetAccessor(IIndexSet<TKey, TValue> indexable, TKey key)
		{
			Indexable = indexable;
			Key = key;
		}
		
		public override TValue Item{
			set{
				Indexable[Key] = value;
			}
		}
	}
	
	public class IndexGetSetAccessor<TKey, TValue> : BasicReadWriteAccessor<TValue>
	{
		public IIndexGetSet<TKey, TValue> Indexable{get; private set;}
		public TKey Key{get; private set;}
		
		public IndexGetSetAccessor(IIndexGetSet<TKey, TValue> indexable, TKey key)
		{
			Indexable = indexable;
			Key = key;
		}
		
		public override TValue Item{
			set{
				Indexable[Key] = value;
			}
			get{
				return Indexable[Key];
			}
		}
	}
}
