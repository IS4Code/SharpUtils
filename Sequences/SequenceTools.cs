/* Date: 1.8.2015, Time: 19:05 */
using System;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.Sequences
{
	public static class SequenceTools
	{
		/// <summary>
		/// Merges an infinite sequence of infinite sequences.
		/// </summary>
		public static IEnumerable<T> MergeInfinite<T>(IEnumerable<IEnumerable<T>> source)
		{
			var enums = new List<IEnumerator<T>>();
			var senum = source.GetEnumerator();
			while(true)
			{
				bool next = false;
				if(senum.MoveNext())
				{
					enums.Add(senum.Current.GetEnumerator());
					next = true;
				}
				foreach(var e in enums)
				{
					if(e.MoveNext())
					{
						yield return e.Current;
						next = true;
					}
				}
				if(!next) yield break;
			}
		}
		
		public static IEnumerable<T> MergeFinite<T>(params IEnumerable<T>[] source)
		{
			return MergeFinite((IEnumerable<IEnumerable<T>>)source);
		}
		
		/// <summary>
		/// Merges a finite sequence of infinite sequences.
		/// </summary>
		public static IEnumerable<T> MergeFinite<T>(IEnumerable<IEnumerable<T>> source)
		{
			var enums = new List<IEnumerator<T>>();
			foreach(var e in source)
			{
				enums.Add(e.GetEnumerator());
			}
			while(true)
			{
				bool next = false;
				foreach(var e in enums)
				{
					if(e.MoveNext())
					{
						yield return e.Current;
						next = true;
					}
				}
				if(!next) yield break;
			}
		}
		
		public static ISequence<T> RepeatInfinite<T>(T value)
		{
			return Sequence.Infinite(RepeatInfiniteInner(value));
		}
		
		private static IEnumerable<T> RepeatInfiniteInner<T>(T value)
		{
			while(true) yield return value;
		}
	}
}
