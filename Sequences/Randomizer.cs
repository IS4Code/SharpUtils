/* Date: 29.7.2015, Time: 14:18 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.Sequences
{
	public static class Randomizer
	{
		public static ISequence<double> Double()
		{
			return Sequence.Infinite(DoubleBase(new Random()));
		}
		
		public static ISequence<double> Double(int seed)
		{
			return Sequence.Infinite(DoubleBase(new Random(seed)));
		}
		
		private static IEnumerable<double> DoubleBase(Random rnd)
		{
			while(true)
			{
				yield return rnd.NextDouble();
			}
		}
		
		
		public static ISequence<byte[]> Bytes(int count)
		{
			return Sequence.Infinite(BytesBase(count, new Random()));
		}
		
		public static ISequence<byte[]> Bytes(int count, int seed)
		{
			return Sequence.Infinite(BytesBase(count, new Random(seed)));
		}
		
		private static IEnumerable<byte[]> BytesBase(int count, Random rnd)
		{
			while(true)
			{
				byte[] arr = new byte[count];
				rnd.NextBytes(arr);
				yield return arr;
			}
		}
		
		
		public static ISequence<byte[]> Bytes(IEnumerable<int> counts)
		{
			var seq = counts as ISequence<int>;
			bool finite = seq != null && seq.IsFinite;
			return Sequence.Create(BytesBase(counts, new Random()), finite);
		}
		
		public static ISequence<byte[]> Bytes(IEnumerable<int> counts, int seed)
		{
			var seq = counts as ISequence<int>;
			bool finite = seq != null && seq.IsFinite;
			return Sequence.Create(BytesBase(counts, new Random(seed)), finite);
		}
		
		private static IEnumerable<byte[]> BytesBase(IEnumerable<int> counts, Random rnd)
		{
			foreach(int count in counts)
			{
				byte[] arr = new byte[count];
				rnd.NextBytes(arr);
				yield return arr;
			}
		}
		
		
		public static ISequence<int> Integers()
		{
			return Integers(0, Int32.MaxValue);
		}
		
		public static ISequence<int> Integers(int maxValue)
		{
			return Integers(0, maxValue);
		}
		
		public static ISequence<int> Integers(int minValue, int maxValue)
		{
			return Sequence.Infinite(IntegersBase(maxValue, minValue, new Random()));
		}
		
		public static ISequence<int> Integers(int minValue, int maxValue, int seed)
		{
			return Sequence.Infinite(IntegersBase(maxValue, minValue, new Random(seed)));
		}
		
		private static IEnumerable<int> IntegersBase(int minValue, int maxValue, Random rnd)
		{
			while(true)
			{
				yield return rnd.Next(minValue, maxValue);
			}
		}
	}
}
