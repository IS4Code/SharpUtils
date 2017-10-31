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
		
		public static IIteratorLink<TSource, TResult> Select<TSource, TResult>(this IIterable<TSource> source, Func<TSource, TResult> selector)
		{
			return Iterator.Select(selector);
		}
		
		public static IIteratorLink<TSource, TSource> Where<TSource>(this IIterable<TSource> source, Func<TSource, bool> predicate)
		{
			return Iterator.Where(predicate);
		}
		
		public static IIteratorLink<TSource, TResult> Aggregate<TSource, TAccumulate, TResult>(this IIterable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
		{
			return Iterator.Aggregate(seed, func, resultSelector);
		}
	}
}
