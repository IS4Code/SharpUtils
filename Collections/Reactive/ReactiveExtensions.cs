/* Date: 30.10.2017, Time: 1:41 */
using System;

namespace IllidanS4.SharpUtils.Collections.Reactive
{
	public static class ReactiveExtensions
	{
		public static void ForEach<T>(this IIterable<T> iterable, Func<T, bool> iterator)
		{
			iterable.Iterate(Iterator.Create(iterator));
		}
		
		public static void ForEach<T>(this IIterable<T> iterable, Action<T> iterator)
		{
			iterable.Iterate(Iterator.Create<T>(value => {iterator(value); return true;}));
		}
	}
}
