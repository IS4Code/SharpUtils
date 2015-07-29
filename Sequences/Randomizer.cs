/* Date: 29.7.2015, Time: 14:18 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.Sequences
{
	public static class Randomizer
	{
		public static IInfiniteSequence<double> Double()
		{
			return InfiniteSequence.Create(DoubleBase(new Random()));
		}
		
		public static IInfiniteSequence<double> Double(int seed)
		{
			return InfiniteSequence.Create(DoubleBase(new Random(seed)));
		}
		
		private static IEnumerable<double> DoubleBase(Random rnd)
		{
			while(true)
			{
				yield return rnd.NextDouble();
			}
		}
		
		
		public static IInfiniteSequence<byte[]> Bytes(int count)
		{
			return InfiniteSequence.Create(BytesBase(count, new Random()));
		}
		
		public static IInfiniteSequence<byte[]> Bytes(int count, int seed)
		{
			return InfiniteSequence.Create(BytesBase(count, new Random(seed)));
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
		
		
		public static IInfiniteSequence<byte[]> Bytes(IEnumerable<int> counts)
		{
			return InfiniteSequence.Create(BytesBase(counts, new Random()));
		}
		
		public static IInfiniteSequence<byte[]> Bytes(IEnumerable<int> counts, int seed)
		{
			return InfiniteSequence.Create(BytesBase(counts, new Random(seed)));
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
		
		
		public static IInfiniteSequence<int> Integers()
		{
			return Integers(0, Int32.MaxValue);
		}
		
		public static IInfiniteSequence<int> Integers(int maxValue)
		{
			return Integers(0, maxValue);
		}
		
		public static IInfiniteSequence<int> Integers(int minValue, int maxValue)
		{
			return InfiniteSequence.Create(IntegersBase(maxValue, minValue, new Random()));
		}
		
		public static IInfiniteSequence<int> Integers(int minValue, int maxValue, int seed)
		{
			return InfiniteSequence.Create(IntegersBase(maxValue, minValue, new Random(seed)));
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
