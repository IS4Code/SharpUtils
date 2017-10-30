/* Date: 30.10.2017, Time: 1:44 */
using System;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.Collections.Reactive
{
	public class EnumIterable<T> : IIterable<T>
	{
		readonly IEnumerable<T> collection;
		
		public EnumIterable(IEnumerable<T> collection)
		{
			this.collection = collection;
		}
		
		public void Iterate(IIterator<T> iterator)
		{
			foreach(T value in collection)
			{
				if(!iterator.OnNext(value)) break;
			}
			iterator.OnCompleted();
		}
	}
	
	public static class EnumIterable
	{
		public static EnumIterable<T> Create<T>(IEnumerable<T> collection)
		{
			return new EnumIterable<T>(collection);
		}
	}
}
