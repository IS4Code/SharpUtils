/* Date: 11.10.2014, Time: 23:21 */
using System;

namespace IllidanS4.SharpUtils.Interop
{
	/// <summary>
	/// Bit-operating tools.
	/// </summary>
	public static class BitUtils
	{
		/// <summary>
		/// Converts a Single value to its Int32 binary representation.
		/// </summary>
		public static unsafe int SingleToInt32Bits(float f)
		{
			return *(int*)(&f);
		}
		
		/// <summary>
		/// Returns a Single value from its Int32 binary representation.
		/// </summary>
		public static unsafe float Int32BitsToSingle(int i)
		{
			return *(float*)(&i);
		}
	}
}
