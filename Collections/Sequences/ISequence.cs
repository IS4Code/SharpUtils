/* Date: 29.7.2015, Time: 14:18 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IllidanS4.SharpUtils.Metadata;

namespace IllidanS4.SharpUtils.Collections.Sequences
{
	/// <summary>
	/// Represents a sequence that guarantees to be finite or not.
	/// </summary>
	public interface ISequence<T> : IEnumerable<T>
	{
		/// <summary>
		/// True if the sequence is guaranteed to be finite, otherwise false.
		/// </summary>
		bool IsFinite{get;}
	}
	
	public sealed class Sequence<T> : ISequence<T>
	{
		readonly IEnumerable<T> seq;
		public bool IsFinite{get; private set;}
		
		public Sequence(IEnumerable<T> seq, bool finite)
		{
			this.seq = seq;
			IsFinite = finite;
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
	
	public static class Sequence
	{
		public static ISequence<T> Create<T>(ICollection<T> list)
		{
			return new Sequence<T>(list, true);
		}
		
		public static ISequence<T> Create<T>(IEnumerable<T> seq, bool finite)
		{
			if(seq is ICollection<T> || seq is ICollection)
			{
				if(!finite)
				{
					throw new ArgumentException("This collection is finite.", "seq");
				}
			}
			return new Sequence<T>(seq, finite);
		}
		
		public static ISequence<T> Infinite<T>(IEnumerable<T> seq)
		{
			return Create(seq, false);
		}
		
		public static ISequence<T> Finite<T>(IEnumerable<T> seq)
		{
			return Create(seq, true);
		}
		
		public static int Count<T>(this ISequence<T> source)
		{
			if(source.IsFinite)
			{
				return ((IEnumerable<T>)source).Count();
			}else{
				throw new ArgumentException("This sequence is infinite.", "source");
			}
		}
	}
}
