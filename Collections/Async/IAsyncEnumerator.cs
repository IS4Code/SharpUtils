/* Date: 14.7.2015, Time: 0:38 */
using System;
using System.Threading.Tasks;

namespace IllidanS4.SharpUtils.Collections.Async
{
	public interface IAsyncEnumerator : IDisposable
	{
		object Current{get;}
		Task<bool> MoveNext();
		void Reset();
	}
	
	public interface IAsyncEnumerator<T> : IAsyncEnumerator
	{
		new T Current{get;}
	}
}
