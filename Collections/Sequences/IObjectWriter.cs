/* Date: 21.7.2014, Time: 12:10 */
using System;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.Collections.Sequences
{
	public interface IObjectWriter<in T>
	{
		void Write(T obj);
		void Write(IEnumerable<T> objects);
	}
}
