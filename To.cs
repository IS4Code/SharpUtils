/* Date: 22.9.2017, Time: 11:46 */
using System;

namespace IllidanS4.SharpUtils
{
	public static class To<TTo>
	{
		public static TTo Cast<TFrom>(TFrom arg)
		{
			return Extensions.FastCast<TFrom, TTo>(arg);
		}
	}
}
