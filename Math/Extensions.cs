/* Date: 11.10.2014, Time: 23:10 */
using System;
using IllidanS4.SharpUtils.Interop;

namespace IllidanS4.SharpUtils.Math
{
	public static class Extensions
	{
		#region Double
		/// <summary>
		/// Returns the smallest number greater than this.
		/// </summary>
		public static double Following(this double value)
		{
			return Following(value, 1L);
		}
		
		/// <summary>
		/// Returns the n-th smallest number greater than this.
		/// </summary>
		/// <param name="steps">n</param>
		public static double Following(this double value, long steps)
		{
			if(Double.IsPositiveInfinity(value) || Double.IsNaN(value)) return value;
			else if(Double.IsNegativeInfinity(value)) return Double.MinValue;
		    var longRep = BitConverter.DoubleToInt64Bits(value);
		    if(longRep >= 0)
		    {
		        longRep += steps;
		    }else if(longRep == Int64.MinValue) //-0
		    {
		        longRep = steps;
		    }else{
		        longRep -= steps;
		    }
		    return BitConverter.Int64BitsToDouble(longRep);
		}
		
		/// <summary>
		/// Returns the highest number less than this.
		/// </summary>
		public static double Preceding(this double value)
		{
			return Preceding(value, 1L);
		}
		/// <summary>
		/// Returns the n-th highest number less than this.
		/// </summary>
		/// <param name="steps">n</param>
		public static double Preceding(this double value, long steps)
		{
			if(Double.IsNegativeInfinity(value) || Double.IsNaN(value)) return value;
			else if(Double.IsPositiveInfinity(value)) return Double.MaxValue;
		    var longRep = BitConverter.DoubleToInt64Bits(value);
		    if(longRep >= 0)
		    {
		        longRep -= steps;
		    }else if(longRep == Int64.MinValue) //-0
		    {
		        longRep = steps;
		    }else{
		        longRep += steps;
		    }
		    return BitConverter.Int64BitsToDouble(longRep);
		}
		#endregion
		
		#region Single
		/// <summary>
		/// Returns the smallest number greater than this.
		/// </summary>
		public static float Following(this float value)
		{
			return Following(value, 1);
		}
		
		/// <summary>
		/// Returns the n-th smallest number greater than this.
		/// </summary>
		/// <param name="steps">n</param>
		public static float Following(this float value, int steps)
		{
			if(Single.IsPositiveInfinity(value) || Single.IsNaN(value)) return value;
			else if(Single.IsNegativeInfinity(value)) return Single.MinValue;
		    var intRep = BitUtils.SingleToInt32Bits(value);
		    if(intRep >= 0)
		    {
		        intRep += steps;
		    }else if(intRep == Int32.MinValue) //-0
		    {
		        intRep = steps;
		    }else{
		        intRep -= steps;
		    }
		    return BitUtils.Int32BitsToSingle(intRep);
		}
		
		/// <summary>
		/// Returns the highest number less than this.
		/// </summary>
		public static float Preceding(this float value)
		{
			return Preceding(value, 1);
		}
		/// <summary>
		/// Returns the n-th highest number less than this.
		/// </summary>
		/// <param name="steps">n</param>
		public static float Preceding(this float value, int steps)
		{
			if(Single.IsNegativeInfinity(value) || Single.IsNaN(value)) return value;
			else if(Single.IsPositiveInfinity(value)) return Single.MaxValue;
		    var intRep = BitUtils.SingleToInt32Bits(value);
		    if(intRep >= 0)
		    {
		        intRep -= steps;
		    }else if(intRep == Int32.MinValue) //-0
		    {
		        intRep = steps;
		    }else{
		        intRep += steps;
		    }
		    return BitUtils.Int32BitsToSingle(intRep);
		}
		#endregion
		
		#region FloatPtr
		
		private static readonly IntPtr IntPtrMinValue = IntPtr.Size==4?(IntPtr)Int32.MinValue:(IntPtr)Int64.MinValue;
		private static readonly IntPtr IntPtrMaxValue = IntPtr.Size==4?(IntPtr)Int32.MaxValue:(IntPtr)Int64.MaxValue;
		
		/// <summary>
		/// Returns the smallest number greater than this.
		/// </summary>
		public static FloatPtr Following(this FloatPtr value)
		{
			return Following(value, (IntPtr)1);
		}
		
		/// <summary>
		/// Returns the n-th smallest number greater than this.
		/// </summary>
		/// <param name="steps">n</param>
		public static FloatPtr Following(this FloatPtr value, IntPtr steps)
		{
			if(FloatPtr.IsPositiveInfinity(value) || FloatPtr.IsNaN(value)) return value;
			else if(FloatPtr.IsNegativeInfinity(value)) return Single.MinValue;
		    var ptrRep = FloatPtr.FloatPtrToIntPtrBits(value);
		    if((long)ptrRep >= 0)
		    {
		    	ptrRep = (IntPtr)((long)ptrRep+(long)steps);
		    }else if(ptrRep == IntPtrMinValue) //-0
		    {
		        ptrRep = steps;
		    }else{
		    	ptrRep = (IntPtr)((long)ptrRep-(long)steps);
		    }
		    return FloatPtr.IntPtrBitsToFloatPtr(ptrRep);
		}
		
		/// <summary>
		/// Returns the highest number less than this.
		/// </summary>
		public static FloatPtr Preceding(this FloatPtr value)
		{
			return Preceding(value, (IntPtr)1);
		}
		/// <summary>
		/// Returns the n-th highest number less than this.
		/// </summary>
		/// <param name="steps">n</param>
		public static FloatPtr Preceding(this FloatPtr value, IntPtr steps)
		{
			if(FloatPtr.IsNegativeInfinity(value) || FloatPtr.IsNaN(value)) return value;
			else if(FloatPtr.IsPositiveInfinity(value)) return Single.MaxValue;
		    var ptrRep = FloatPtr.FloatPtrToIntPtrBits(value);
		    if((long)ptrRep >= 0)
		    {
		    	ptrRep = (IntPtr)((long)ptrRep-(long)steps);
		    }else if(ptrRep == IntPtrMinValue) //-0
		    {
		        ptrRep = steps;
		    }else{
		    	ptrRep = (IntPtr)((long)ptrRep+(long)steps);
		    }
		    return FloatPtr.IntPtrBitsToFloatPtr(ptrRep);
		}
		#endregion
	}
}
