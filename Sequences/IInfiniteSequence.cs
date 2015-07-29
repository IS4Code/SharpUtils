/* Date: 29.7.2015, Time: 14:18 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.Sequences
{
	/// <summary>
	/// Represents a sequence that might never end.
	/// </summary>
	public interface IInfiniteSequence<T> : IEnumerable<T>
	{
		
	}
	
	public sealed class InfiniteSequence<T> : IInfiniteSequence<T>
	{
		readonly IEnumerable<T> seq;
		
		public InfiniteSequence(IEnumerable<T> seq)
		{
			this.seq = seq;
		}
		
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return seq.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)seq).GetEnumerator();
		}
	}
	
	public static class InfiniteSequence
	{
		public static IInfiniteSequence<T> Create<T>(IEnumerable<T> seq)
		{
			return new InfiniteSequence<T>(seq);
		}
	}
}
