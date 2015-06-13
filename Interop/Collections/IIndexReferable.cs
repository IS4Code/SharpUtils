/* Date: 29.4.2015, Time: 23:02 */
using System;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Accessing;
using IllidanS4.SharpUtils.Metadata;

namespace IllidanS4.SharpUtils.Interop.Collections
{
	/// <summary>
	/// Represents an indexable collections to which's elements references can be obtained.
	/// </summary>
	public interface IIndexRefReferable<TKey, TValue> : IIndexableSetter<TKey, TValue>, IIndexOutReferable<TKey, TValue>//, IIndexTypedRefReferable
	{
		TRet GetReference<TRet>(TKey index, Reference.RefFunc<TValue, TRet> func);
	}
	
	public interface IIndexOutReferable<TKey, TValue> : IIndexableGetter<TKey, TValue>
	{
		TRet GetReference<TRet>(TKey index, Reference.OutFunc<TValue, TRet> func);
	}
	
	[CLSCompliant(false)]
	public interface IIndexTypedRefReferable
	{
		TRet GetReference<TRet>(object index, TypedReferenceTools.TypedRefFunc<TRet> func);
	}
}
