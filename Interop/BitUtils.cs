/* Date: 11.10.2014, Time: 23:21 */
using System;

namespace IllidanS4.SharpUtils.Interop
{
	public static class BitUtils
	{
		public static unsafe int SingleToInt32Bits(float f)
		{
			return *(int*)(&f);
		}
		
		public static unsafe float Int32BitsToSingle(int i)
		{
			return *(float*)(&i);
		}
	}
}
