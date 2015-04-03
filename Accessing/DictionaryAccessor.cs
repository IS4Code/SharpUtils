/* Date: 3.4.2015, Time: 20:41 */
using System;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.Accessing
{
	public class DictionaryAccessor<TKey, TValue> : ReadDictionaryAccessor<TKey, TValue>, IWriteAccessor<TValue>
	{
		public DictionaryAccessor(IDictionary<TKey, TValue> dictionary, TKey key) : base(dictionary, key)
		{
			if(dictionary.IsReadOnly)
			{
				throw new ArgumentException("This dictionary is read-only.", "dictionary");
			}
		}
		
		public new TValue Item{
			set{
				Dictionary[Key] = value;
			}
			get{
				return base.Item;
			}
		}
		
		object IWriteAccessor.Item{
			set{
				this.Item = (TValue)value;
			}
		}
	}
	
	public class ReadDictionaryAccessor<TKey, TValue> : BasicReadAccessor<TValue>
	{
		public IDictionary<TKey, TValue> Dictionary{get; private set;}
		public TKey Key{get; private set;}
		
		public ReadDictionaryAccessor(IDictionary<TKey, TValue> dictionary, TKey key)
		{
			Dictionary = dictionary;
			Key = key;
		}
		
		public override TValue Item{
			get{
				return Dictionary[Key];
			}
		}
	}
}
