/* Date: 30.10.2017, Time: 1:29 */
using System;

namespace IllidanS4.SharpUtils.Collections.Reactive
{
	/// <summary>
	/// Provides a push-based imperative iterator for a collection.
	/// </summary>
	public interface IIterable<T>
	{
		/// <summary>
		/// Iterates the collection using a <see cref="IIterator"/>.
		/// </summary>
		/// <param name="iterator">The iterator which gets notified of the elements in the collection.</param>
		void Iterate(IIterator<T> iterator);
	}
}
