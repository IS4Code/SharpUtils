/* Date: 30.10.2017, Time: 16:20 */
using System;

namespace IllidanS4.SharpUtils.Collections.Reactive
{
	public static class Iterable
	{
		public static IIterable<TResult> Create<TSource, TResult>(IIterable<TSource> iterable, IIteratorLink<TSource, TResult> link)
		{
			return new LinkIterable<TSource, TResult>(iterable, link);
		}
		
		private class LinkIterable<TSource, TResult> : IIterable<TResult>
		{
			readonly IIterable<TSource> iterable;
			readonly IIteratorLink<TSource, TResult> link;
			
			public LinkIterable(IIterable<TSource> iterable, IIteratorLink<TSource, TResult> link)
			{
				this.iterable = iterable;
				this.link = link;
			}
			
			public void Iterate(IIterator<TResult> iterator)
			{
				using(var handle = link.Subscribe(iterator))
				{
					iterable.Iterate(link);
				}
			}
		}
	}
}
