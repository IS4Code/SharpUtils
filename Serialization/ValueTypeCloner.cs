/* Date: 23.3.2016, Time: 22:32 */
using System;

namespace IllidanS4.SharpUtils.Serialization
{
	public class ValueTypeCloner<T> : ICloner<T> where T : struct
	{
		public static T Clone(T instance)
		{
			return instance;
		}
		
		T ICloner<T>.Clone(T instance)
		{
			return Clone(instance);
		}
	}
}
