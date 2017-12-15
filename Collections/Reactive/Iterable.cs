/* Date: 30.10.2017, Time: 16:20 */
using System;

namespace IllidanS4.SharpUtils.Collections.Reactive
{
	public static class Iterable
	{
		private abstract class LinkIterable<TResult> : IIterable<TResult>
		{
			public abstract void Iterate(IIterator<TResult> iterator);
			
			public abstract LinkIterable<TNewResult> Link<TNewResult>(IIteratorLink<TResult, TNewResult> link);
			
			public abstract LinkIterable<TNewResult> Select<TNewResult>(Func<TResult, TNewResult> selector);
			
			public abstract LinkIterable<TResult> Where(Func<TResult, bool> predicate);
			
			public abstract LinkIterable<TNewResult> Aggregate<TAccumulate, TNewResult>(TAccumulate seed, Func<TAccumulate, TResult, TAccumulate> func, Func<TAccumulate, TNewResult> resultSelector);
		}
		
		private class LinkIterable<TSource, TResult> : LinkIterable<TResult>
		{
			readonly IIterable<TSource> iterable;
			readonly IIteratorLink<TSource, TResult> link;
			
			public LinkIterable(IIterable<TSource> iterable, IIteratorLink<TSource, TResult> link)
			{
				this.iterable = iterable;
				this.link = link;
			}
			
			public override void Iterate(IIterator<TResult> iterator)
			{
				using(var handle = link.Subscribe(iterator))
				{
					iterable.Iterate(link);
				}
			}
			
			public override LinkIterable<TNewResult> Link<TNewResult>(IIteratorLink<TResult, TNewResult> link)
			{
				return new LinkIterable<TSource, TNewResult>(iterable, Iterator.Link(this.link, link));
			}
			
			public override LinkIterable<TNewResult> Select<TNewResult>(Func<TResult, TNewResult> selector)
			{
				return new LinkIterable<TSource, TNewResult>(iterable, Iterator.Select(this.link, selector));
			}
			
			public override LinkIterable<TResult> Where(Func<TResult, bool> predicate)
			{
				return new LinkIterable<TSource, TResult>(iterable, Iterator.Where(this.link, predicate));
			}
			
			public override LinkIterable<TNewResult> Aggregate<TAccumulate, TNewResult>(TAccumulate seed, Func<TAccumulate, TResult, TAccumulate> func, Func<TAccumulate, TNewResult> resultSelector)
			{
				return new LinkIterable<TSource, TNewResult>(iterable, Iterator.Aggregate(this.link, seed, func, resultSelector));
			}
		}
		
		public static IIterable<TResult> Link<TSource, TResult>(this IIterable<TSource> source, IIteratorLink<TSource, TResult> link)
		{
			var linkIterable = source as LinkIterable<TSource>;
			if(linkIterable != null)
			{
				return linkIterable.Link(link);
			}
			return new LinkIterable<TSource, TResult>(source, link);
		}
		
		public static IIterable<TResult> Select<TSource, TResult>(this IIterable<TSource> source, Func<TSource, TResult> selector)
		{
			var linkIterable = source as LinkIterable<TSource>;
			if(linkIterable != null)
			{
				return linkIterable.Select(selector);
			}
			return new LinkIterable<TSource, TResult>(source, Iterator.Select(selector));
		}
		
		public static IIterable<TSource> Where<TSource>(this IIterable<TSource> source, Func<TSource, bool> predicate)
		{
			var linkIterable = source as LinkIterable<TSource>;
			if(linkIterable != null)
			{
				return linkIterable.Where(predicate);
			}
			return new LinkIterable<TSource, TSource>(source, Iterator.Where(predicate));
		}
		
		public static IIterable<TNewResult> Aggregate<TSource, TAccumulate, TNewResult>(this IIterable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TNewResult> resultSelector)
		{
			var linkIterable = source as LinkIterable<TSource>;
			if(linkIterable != null)
			{
				return linkIterable.Aggregate(seed, func, resultSelector);
			}
			return new LinkIterable<TSource, TNewResult>(source, Iterator.Aggregate(seed, func, resultSelector));
		}
	}
}
