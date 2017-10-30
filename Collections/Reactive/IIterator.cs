/* Date: 30.10.2017, Time: 1:29 */
using System;

namespace IllidanS4.SharpUtils.Collections.Reactive
{
	/// <summary>
	/// Provides an object for receiving notifications from a <see cref="IIterable"/>.
	/// </summary>
	public interface IIterator<T>
	{
		/// <summary>
		/// Called when a new value is received.
		/// </summary>
		/// <param name="value">The next value in the collextion.</param>
		/// <returns>True if more values should be received.</returns>
		bool OnNext(T value);
		
		/// <summary>
		/// Called when the collection has reached its end or no new values will be received.
		/// </summary>
		void OnCompleted();
	}
}
