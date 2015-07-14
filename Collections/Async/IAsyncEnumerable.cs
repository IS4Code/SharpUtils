/* Date: 14.7.2015, Time: 0:38 */
using System;

namespace IllidanS4.SharpUtils.Collections.Async
{
	public interface IAsyncEnumerable
	{
		IAsyncEnumerator GetEnumerator();
	}
	
	public interface IAsyncEnumerable<T> : IAsyncEnumerable
	{
		new IAsyncEnumerator<T> GetEnumerator();
	}
}
