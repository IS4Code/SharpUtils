/* Date: 3.4.2015, Time: 21:15 */
using System;

namespace IllidanS4.SharpUtils.Accessing
{
	public class IndexGetAccessor<TKey, TValue> : BasicReadAccessor<TValue>
	{
		public IIndexableGetter<TKey, TValue> Indexable{get; private set;}
		public TKey Key{get; private set;}
		
		public IndexGetAccessor(IIndexableGetter<TKey, TValue> indexable, TKey key)
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
		public IIndexableSetter<TKey, TValue> Indexable{get; private set;}
		public TKey Key{get; private set;}
		
		public IndexSetAccessor(IIndexableSetter<TKey, TValue> indexable, TKey key)
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
	
	public class IndexGetSetAccessor<TKey, TValue, TIndexable> : BasicReadWriteAccessor<TValue> where TIndexable : IIndexableGetter<TKey, TValue>, IIndexableSetter<TKey, TValue>
	{
		public TIndexable Indexable{get; private set;}
		public TKey Key{get; private set;}
		
		public IndexGetSetAccessor(TIndexable indexable, TKey key)
		{
			Indexable = indexable;
			Key = key;
		}
		
		public override TValue Item{
			set{
				((IIndexableSetter<TKey, TValue>)Indexable)[Key] = value;
			}
			get{
				return ((IIndexableGetter<TKey, TValue>)Indexable)[Key];
			}
		}
	}
}
