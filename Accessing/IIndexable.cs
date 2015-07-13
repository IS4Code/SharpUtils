/* Date: 30.11.2014, Time: 11:13 */
using System;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.Accessing
{
	/// <summary>
	/// Represents an object which's index values can be obtained.
	/// </summary>
	public interface IIndexGet<in TKey, out TValue>
	{
		TValue this[TKey index]{
			get;
		}
	}
	
	/// <summary>
	/// Represents an object which's index values can be set.
	/// </summary>
	public interface IIndexSet<in TKey, in TValue>
	{
		TValue this[TKey index]{
			set;
		}
	}
	
	public interface IIndexGetSet<in TKey, TValue> : IIndexGet<TKey, TValue>, IIndexSet<TKey, TValue>
	{
		new TValue this[TKey index]{
			get; set;
		}
	}
	
	/// <summary>
	/// Creates an indexable wrapper around a dictionary.
	/// </summary>
	public class IndexDictionaryWrapper<TKey,TValue> : IIndexGetSet<TKey,TValue>
	{
		private readonly IDictionary<TKey,TValue> dict;
		
		public TValue this[TKey index]
		{
			get{
				return dict[index];
			}
			set{
				dict[index] = value;
			}
		}
		
		public IndexDictionaryWrapper(IDictionary<TKey,TValue> dict)
		{
			this.dict = dict;
		}
	}
	
	/// <summary>
	/// Creates an indexable wrapper around a list.
	/// </summary>
	public class IndexListWrapper<TElement> : IIndexGetSet<int,TElement>
	{
		private readonly IList<TElement> list;
		
		public TElement this[int index]
		{
			get{
				return list[index];
			}
			set{
				list[index] = value;
			}
		}
		
		public IndexListWrapper(IList<TElement> list)
		{
			this.list = list;
		}
	}
	
	/// <summary>
	/// Creates an indexable wrapper around a dynamic object.
	/// </summary>
	public class DynamicIndexWrapper<TKey,TValue> : IIndexGetSet<TKey,TValue>
	{
		private readonly dynamic obj;
		
		public DynamicIndexWrapper(dynamic obj)
		{
			this.obj = obj;
		}
		
		public TValue this[TKey index]
		{
			get{
				return obj[index];
			}
			set{
				obj[index] = value;
			}
		}
	}
}
