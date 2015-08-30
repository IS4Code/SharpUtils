/* Date: 29.7.2015, Time: 14:18 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace IllidanS4.SharpUtils.Sequences
{
	public static class NumberSequences
	{
		public static ISequence<double> RandomDouble()
		{
			return Sequence.Infinite(DoubleBase(new Random()));
		}
		
		public static ISequence<double> RandomDouble(int seed)
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
		
		
		public static ISequence<byte[]> RandomBytes(int count)
		{
			return Sequence.Infinite(BytesBase(count, new Random()));
		}
		
		public static ISequence<byte[]> RandomBytes(int count, int seed)
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
		
		
		public static ISequence<byte[]> RandomBytes(IEnumerable<int> counts)
		{
			var seq = counts as ISequence<int>;
			bool finite = seq != null && seq.IsFinite;
			return Sequence.Create(BytesBase(counts, new Random()), finite);
		}
		
		public static ISequence<byte[]> RandomBytes(IEnumerable<int> counts, int seed)
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
		
		
		public static ISequence<int> RandomIntegers()
		{
			return RandomIntegers(0, Int32.MaxValue);
		}
		
		public static ISequence<int> RandomIntegers(int maxValue)
		{
			return RandomIntegers(0, maxValue);
		}
		
		public static ISequence<int> RandomIntegers(int minValue, int maxValue)
		{
			return Sequence.Infinite(IntegersBase(maxValue, minValue, new Random()));
		}
		
		public static ISequence<int> RandomIntegers(int minValue, int maxValue, int seed)
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
		
		
		
		public static ISequence<BigInteger> AllIntegers()
		{
			return Sequence.Infinite(AllIntegersBase());
		}
		
		private static IEnumerable<BigInteger> AllIntegersBase()
		{
			yield return BigInteger.Zero;
			var i = BigInteger.One;
			while(true)
			{
				yield return i;
				yield return -i;
				i += BigInteger.One;
			}
		}
	}
}
