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
	[CLSCompliant(false)]
	public interface IIndexReferable<TKey, TValue> : IIndexableSetter<TKey, TValue>, IIndexableGetter<TKey, TValue>
	{
		unsafe void GetReference(TKey index, [Out]TypedReference* tref);
		[return: Boxed(typeof(TypedReference))]
		ValueType GetReference(TKey index);
	}
}
