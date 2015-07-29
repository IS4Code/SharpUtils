/* Date: 21.7.2014, Time: 12:10 */
using System;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.Sequences
{
	public interface IObjectReader<out T> : IEnumerable<T>
	{
		T Read();
	}
}
