/* Date: 28.12.2014, Time: 17:56 */
using System;

namespace IllidanS4.SharpUtils.Accessing
{
	public class AtomicContainer<T> : IReadAccessor<T>, IWriteAccessor<T>
	{
		public T Value{get; set;}
		
		public AtomicContainer()
		{
			
		}
		
		public AtomicContainer(T value)
		{
			Value = value;
		}
	}
}
