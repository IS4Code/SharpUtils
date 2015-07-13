/* Date: 3.4.2015, Time: 20:41 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace IllidanS4.SharpUtils.Accessing
{
	public class DictionaryAccessor<TKey, TValue> : ReadDictionaryAccessor<TKey, TValue>, IWriteAccessor<TValue>, IStrongBox
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
		
		object IStrongBox.Value{
			get{
				return Item;
			}
			set{
				Item = (TValue)value;
			}
		}
	}
	
	public class ReadDictionaryAccessor<TKey, TValue> : BasicReadAccessor<TValue>, IDictionaryAccessor
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
		
		IEnumerable IDictionaryAccessor.Dictionary{
			get{
				return Dictionary;
			}
		}
		
		object IDictionaryAccessor.Key{
			get{
				return Key;
			}
		}
		
		Type IDictionaryAccessor.KeyType{
			get{
				return typeof(TKey);
			}
		}
	}
	
	public interface IDictionaryAccessor : IReadAccessor
	{
		IEnumerable Dictionary{get;}
		object Key{get;}
		Type KeyType{get;}
	}
	
	public static class DictionaryAccessor
	{
		public static DictionaryAccessor<TKey, TValue> Create<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
		{
			return new DictionaryAccessor<TKey, TValue>(dictionary, key);
		}
		
		public static ReadDictionaryAccessor<TKey, TValue> CreateReadOnly<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
		{
			return new ReadDictionaryAccessor<TKey, TValue>(dictionary, key);
		}
	}
}
