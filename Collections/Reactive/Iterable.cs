/* Date: 30.10.2017, Time: 16:20 */
using System;

namespace IllidanS4.SharpUtils.Collections.Reactive
{
	public static class Iterable
	{
		private interface ILinkIterable<out TResult> : IIterable<TResult>
		{
			ILinkIterable<TNewResult> Link<TNewResult>(IIteratorLink<TResult, TNewResult> link);
			
			ILinkIterable<TNewResult> Select<TNewResult>(Func<TResult, TNewResult> selector);
			
			ILinkIterable<TResult> Where(Func<TResult, bool> predicate);
			
			ILinkIterable<TNewResult> Aggregate<TAccumulate, TNewResult>(TAccumulate seed, Func<TAccumulate, TResult, TAccumulate> func, Func<TAccumulate, TNewResult> resultSelector);
			
			ILinkIterable<TResult> Sum();
		}
		
		private class LinkIterable<TSource, TResult> : ILinkIterable<TResult>
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
			
			public ILinkIterable<TNewResult> Link<TNewResult>(IIteratorLink<TResult, TNewResult> link)
			{
				return new LinkIterable<TSource, TNewResult>(iterable, Iterator.Link(this.link, link));
			}
			
			public ILinkIterable<TNewResult> Select<TNewResult>(Func<TResult, TNewResult> selector)
			{
				return new LinkIterable<TSource, TNewResult>(iterable, Iterator.Select(this.link, selector));
			}
			
			public ILinkIterable<TResult> Where(Func<TResult, bool> predicate)
			{
				return new LinkIterable<TSource, TResult>(iterable, Iterator.Where(this.link, predicate));
			}
			
			public ILinkIterable<TNewResult> Aggregate<TAccumulate, TNewResult>(TAccumulate seed, Func<TAccumulate, TResult, TAccumulate> func, Func<TAccumulate, TNewResult> resultSelector)
			{
				return new LinkIterable<TSource, TNewResult>(iterable, Iterator.Aggregate(this.link, seed, func, resultSelector));
			}
			
			public ILinkIterable<TResult> Sum()
			{
				Type type = typeof(TResult);
				Type underlyingType = Nullable.GetUnderlyingType(type);
				
				bool nullable;
				TypeCode typeCode;
				if(underlyingType != null)
				{
					nullable = true;
					typeCode = Type.GetTypeCode(underlyingType);
				}else{
					nullable = false;
					typeCode = Type.GetTypeCode(type);
				}
				
				switch(typeCode)
				{
					case TypeCode.Int32:
						return nullable ? (ILinkIterable<TResult>)(object)new LinkIterable<TSource, int?>(iterable, Iterator.Sum((IIteratorLink<TSource, int?>)this.link)) : (ILinkIterable<TResult>)(object)new LinkIterable<TSource, int>(iterable, Iterator.Sum((IIteratorLink<TSource, int>)this.link));
					case TypeCode.Int64:
						return nullable ? (ILinkIterable<TResult>)(object)new LinkIterable<TSource, long?>(iterable, Iterator.Sum((IIteratorLink<TSource, long?>)this.link)) : (ILinkIterable<TResult>)(object)new LinkIterable<TSource, long>(iterable, Iterator.Sum((IIteratorLink<TSource, long>)this.link));
					case TypeCode.Single:
						return nullable ? (ILinkIterable<TResult>)(object)new LinkIterable<TSource, float?>(iterable, Iterator.Sum((IIteratorLink<TSource, float?>)this.link)) : (ILinkIterable<TResult>)(object)new LinkIterable<TSource, float>(iterable, Iterator.Sum((IIteratorLink<TSource, float>)this.link));
					case TypeCode.Double:
						return nullable ? (ILinkIterable<TResult>)(object)new LinkIterable<TSource, double?>(iterable, Iterator.Sum((IIteratorLink<TSource, double?>)this.link)) : (ILinkIterable<TResult>)(object)new LinkIterable<TSource, double>(iterable, Iterator.Sum((IIteratorLink<TSource, double>)this.link));
					case TypeCode.Decimal:
						return nullable ? (ILinkIterable<TResult>)(object)new LinkIterable<TSource, decimal?>(iterable, Iterator.Sum((IIteratorLink<TSource, decimal?>)this.link)) : (ILinkIterable<TResult>)(object)new LinkIterable<TSource, decimal>(iterable, Iterator.Sum((IIteratorLink<TSource, decimal>)this.link));
					default:
						throw new NotImplementedException();
				}
			}
			
			public ILinkIterable<TNewResult> Process<TNewResult>(Func<Func<TNewResult, bool>, Action, bool> processingFunc)
			{
				return new LinkIterable<TSource, TNewResult>(iterable, Iterator.Process(this.link, processingFunc));
			}
			
			public ILinkIterable<TNewResult> Process<TNewResult>(Action<Func<TNewResult, bool>, Action> processingAction)
			{
				return new LinkIterable<TSource, TNewResult>(iterable, Iterator.Process(this.link, processingAction));
			}
		}
		
		public static IIterable<TResult> Link<TSource, TResult>(this IIterable<TSource> source, IIteratorLink<TSource, TResult> link)
		{
			if(source == null) throw new ArgumentNullException("source");
			if(link == null) throw new ArgumentNullException("link");
			
			var linkIterable = source as ILinkIterable<TSource>;
			if(linkIterable != null)
			{
				return linkIterable.Link(link);
			}
			return new LinkIterable<TSource, TResult>(source, link);
		}
		
		public static IIterable<TResult> Select<TSource, TResult>(this IIterable<TSource> source, Func<TSource, TResult> selector)
		{
			if(source == null) throw new ArgumentNullException("source");
			if(selector == null) throw new ArgumentNullException("selector");
			
			var linkIterable = source as ILinkIterable<TSource>;
			if(linkIterable != null)
			{
				return linkIterable.Select(selector);
			}
			return new LinkIterable<TSource, TResult>(source, Iterator.Select(selector));
		}
		
		public static IIterable<TSource> Where<TSource>(this IIterable<TSource> source, Func<TSource, bool> predicate)
		{
			if(source == null) throw new ArgumentNullException("source");
			if(predicate == null) throw new ArgumentNullException("predicate");
			
			var linkIterable = source as ILinkIterable<TSource>;
			if(linkIterable != null)
			{
				return linkIterable.Where(predicate);
			}
			return new LinkIterable<TSource, TSource>(source, Iterator.Where(predicate));
		}
		
		public static IIterable<TNewResult> AggregateMany<TSource, TAccumulate, TNewResult>(this IIterable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TNewResult> resultSelector)
		{
			if(source == null) throw new ArgumentNullException("source");
			if(func == null) throw new ArgumentNullException("func");
			if(resultSelector == null) throw new ArgumentNullException("resultSelector");
			
			var linkIterable = source as ILinkIterable<TSource>;
			if(linkIterable != null)
			{
				return linkIterable.Aggregate(seed, func, resultSelector);
			}
			return new LinkIterable<TSource, TNewResult>(source, Iterator.Aggregate(seed, func, resultSelector));
		}
		
		public static TNewResult Aggregate<TSource, TAccumulate, TNewResult>(this IIterable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TNewResult> resultSelector)
		{
			if(source == null) throw new ArgumentNullException("source");
			if(func == null) throw new ArgumentNullException("func");
			if(resultSelector == null) throw new ArgumentNullException("resultSelector");
			
			source.ForEach(
				o => seed = func(seed, o)
			);
			
			return resultSelector(seed);
		}
		
		public static IIterable<int> SumMany(this IIterable<int> source)
		{
			if(source == null) throw new ArgumentNullException("source");
			
			var linkIterable = source as ILinkIterable<int>;
			if(linkIterable != null)
			{
				return linkIterable.Sum();
			}
			return new LinkIterable<int, int>(source, Iterator.Sum(default(IIteratorLink<int, int>)));
		}
		
		public static IIterable<long> SumMany(this IIterable<long> source)
		{
			if(source == null) throw new ArgumentNullException("source");
			
			var linkIterable = source as ILinkIterable<long>;
			if(linkIterable != null)
			{
				return linkIterable.Sum();
			}
			return new LinkIterable<long, long>(source, Iterator.Sum(default(IIteratorLink<long, long>)));
		}
		
		public static IIterable<float> SumMany(this IIterable<float> source)
		{
			if(source == null) throw new ArgumentNullException("source");
			
			var linkIterable = source as ILinkIterable<float>;
			if(linkIterable != null)
			{
				return linkIterable.Sum();
			}
			return new LinkIterable<float, float>(source, Iterator.Sum(default(IIteratorLink<float, float>)));
		}
		
		public static IIterable<double> SumMany(this IIterable<double> source)
		{
			if(source == null) throw new ArgumentNullException("source");
			
			var linkIterable = source as ILinkIterable<double>;
			if(linkIterable != null)
			{
				return linkIterable.Sum();
			}
			return new LinkIterable<double, double>(source, Iterator.Sum(default(IIteratorLink<double, double>)));
		}
		
		public static IIterable<decimal> SumMany(this IIterable<decimal> source)
		{
			if(source == null) throw new ArgumentNullException("source");
			
			var linkIterable = source as ILinkIterable<decimal>;
			if(linkIterable != null)
			{
				return linkIterable.Sum();
			}
			return new LinkIterable<decimal, decimal>(source, Iterator.Sum(default(IIteratorLink<decimal, decimal>)));
		}
		
		
		public static IIterable<int?> SumMany(this IIterable<int?> source)
		{
			if(source == null) throw new ArgumentNullException("source");
			
			var linkIterable = source as ILinkIterable<int?>;
			if(linkIterable != null)
			{
				return linkIterable.Sum();
			}
			return new LinkIterable<int?, int?>(source, Iterator.Sum(default(IIteratorLink<int?, int?>)));
		}
		
		public static IIterable<long?> SumMany(this IIterable<long?> source)
		{
			if(source == null) throw new ArgumentNullException("source");
			
			var linkIterable = source as ILinkIterable<long?>;
			if(linkIterable != null)
			{
				return linkIterable.Sum();
			}
			return new LinkIterable<long?, long?>(source, Iterator.Sum(default(IIteratorLink<long?, long?>)));
		}
		
		public static IIterable<float?> SumMany(this IIterable<float?> source)
		{
			if(source == null) throw new ArgumentNullException("source");
			
			var linkIterable = source as ILinkIterable<float?>;
			if(linkIterable != null)
			{
				return linkIterable.Sum();
			}
			return new LinkIterable<float?, float?>(source, Iterator.Sum(default(IIteratorLink<float?, float?>)));
		}
		
		public static IIterable<double?> SumMany(this IIterable<double?> source)
		{
			if(source == null) throw new ArgumentNullException("source");
			
			var linkIterable = source as ILinkIterable<double?>;
			if(linkIterable != null)
			{
				return linkIterable.Sum();
			}
			return new LinkIterable<double?, double?>(source, Iterator.Sum(default(IIteratorLink<double?, double?>)));
		}
		
		public static IIterable<decimal?> SumMany(this IIterable<decimal?> source)
		{
			if(source == null) throw new ArgumentNullException("source");
			
			var linkIterable = source as ILinkIterable<decimal?>;
			if(linkIterable != null)
			{
				return linkIterable.Sum();
			}
			return new LinkIterable<decimal?, decimal?>(source, Iterator.Sum(default(IIteratorLink<decimal?, decimal?>)));
		}
		
		public static int Sum(this IIterable<int> source)
		{
			if(source == null) throw new ArgumentNullException("source");
			
			int sum = 0;
			source.ForEach(
				o => sum = checked(sum + o)
			);
			
			return sum;
		}
		
		public static long Sum(this IIterable<long> source)
		{
			if(source == null) throw new ArgumentNullException("source");
			
			long sum = 0;
			source.ForEach(
				o => sum = checked(sum + o)
			);
			
			return sum;
		}
		
		public static float Sum(this IIterable<float> source)
		{
			if(source == null) throw new ArgumentNullException("source");
			
			float sum = 0;
			source.ForEach(
				o => sum = sum + o
			);
			
			return sum;
		}
		
		public static double Sum(this IIterable<double> source)
		{
			if(source == null) throw new ArgumentNullException("source");
			
			double sum = 0;
			source.ForEach(
				o => sum = sum + o
			);
			
			return sum;
		}
		
		public static decimal Sum(this IIterable<decimal> source)
		{
			if(source == null) throw new ArgumentNullException("source");
			
			decimal sum = 0;
			source.ForEach(
				o => sum = sum + o
			);
			
			return sum;
		}
		
		
		public static int? Sum(this IIterable<int?> source)
		{
			if(source == null) throw new ArgumentNullException("source");
			
			int sum = 0;
			source.ForEach(
				o => sum = checked(sum + o ?? 0)
			);
			
			return sum;
		}
		
		public static long? Sum(this IIterable<long?> source)
		{
			if(source == null) throw new ArgumentNullException("source");
			
			long sum = 0;
			source.ForEach(
				o => sum = checked(sum + o ?? 0)
			);
			
			return sum;
		}
		
		public static float? Sum(this IIterable<float?> source)
		{
			if(source == null) throw new ArgumentNullException("source");
			
			float sum = 0;
			source.ForEach(
				o => sum = sum + o ?? 0
			);
			
			return sum;
		}
		
		public static double? Sum(this IIterable<double?> source)
		{
			if(source == null) throw new ArgumentNullException("source");
			
			double sum = 0;
			source.ForEach(
				o => sum = sum + o ?? 0
			);
			
			return sum;
		}
		
		public static decimal? Sum(this IIterable<decimal?> source)
		{
			if(source == null) throw new ArgumentNullException("source");
			
			decimal sum = 0;
			source.ForEach(
				o => sum = sum + o ?? 0
			);
			
			return sum;
		}
		
		public static IIterable<TResult> Process<TSource, TResult>(this IIterable<TSource> source, Func<Func<TResult, bool>, Action, bool> processingFunc)
		{
			if(source == null) throw new ArgumentNullException("source");
			if(processingFunc == null) throw new ArgumentNullException("processingFunc");
			
			var linkIterable = source as ILinkIterable<TSource>;
			if(linkIterable != null)
			{
				return linkIterable.Process(processingFunc);
			}
			return new LinkIterable<TSource, TResult>(source, Iterator.Process<TSource, TResult>(processingFunc));
		}
		
		public static IIterable<TResult> Process<TSource, TResult>(this IIterable<TSource> source, Action<Func<TResult, bool>, Action> processingAction)
		{
			if(source == null) throw new ArgumentNullException("source");
			if(processingAction == null) throw new ArgumentNullException("processingFunc");
			
			var linkIterable = source as ILinkIterable<TSource>;
			if(linkIterable != null)
			{
				return linkIterable.Process(processingAction);
			}
			return new LinkIterable<TSource, TResult>(source, Iterator.Process<TSource, TResult>(processingAction));
		}
	}
}
