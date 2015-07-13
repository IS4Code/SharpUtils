/* Date: 29.4.2015, Time: 23:02 */
using System;

namespace IllidanS4.SharpUtils.Accessing
{
	/// <summary>
	/// Represents an indexable collections to which's elements references can be obtained.
	/// </summary>
	public interface IIndexRefReference<in TKey, TValue> : IIndexGetSet<TKey, TValue>, IIndexOutReference<TKey, TValue>, IIndexTypedReference<TKey>
	{
		TRet GetReference<TRet>(TKey index, Reference.RefFunc<TValue, TRet> func);
	}
	
	public interface IIndexOutReference<in TKey, TValue> : IIndexSet<TKey, TValue>
	{
		TRet GetReference<TRet>(TKey index, Reference.OutFunc<TValue, TRet> func);
	}
	
	public interface IIndexTypedReference<in TKey> : IIndexTypedReference
	{
		TRet GetReference<TRet>(TKey index, Func<SafeReference, TRet> func);
	}
	
	public interface IIndexTypedReference
	{
		TRet GetReference<TRet>(object index, Func<SafeReference, TRet> func);
	}
}
