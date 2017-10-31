/* Date: 30.10.2017, Time: 16:18 */
using System;

namespace IllidanS4.SharpUtils.Collections.Reactive
{
	/// <summary>
	/// Represents a link that filters and repeats values to other iterators.
	/// </summary>
	public interface IIteratorLink<TSource, TResult> : IIterator<TSource>, IIteratorLink<TResult>
	{
		
	}
	
	/// <summary>
	/// Represents a link that filters and repeats values to other iterators.
	/// </summary>
	public interface IIteratorLink<TResult>
	{
		/// <summary>
		/// Subscribes a new iterator to receive values from the iterator link.
		/// </summary>
		/// <param name="iterator">The iterator that should be sent messages from the link.</param>
		/// <returns>A disposable handle to the subscribtion that can be used to unsubscribe the iterator.</returns>
		IDisposable Subscribe(IIterator<TResult> iterator);
	}
}
