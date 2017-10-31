/* Date: 30.10.2017, Time: 1:38 */
using System;

namespace IllidanS4.SharpUtils.Collections.Reactive
{
	public static class Iterator
	{
		public static IIterator<T> Create<T>(Func<T, bool> onNext=null, Action onCompleted=null)
		{
			return new FuncIterator<T>(onNext, onCompleted);
		}
		
		private class FuncIterator<T> : IIterator<T>
		{
			readonly Func<T, bool> onNext;
			readonly Action onCompleted;
			
			public FuncIterator(Func<T, bool> onNext, Action onCompleted)
			{
				this.onNext = onNext;
				this.onCompleted = onCompleted;
			}
			
			public bool OnNext(T value)
			{
				if(onNext != null) return onNext(value);
				return true;
			}
			
			public void OnCompleted()
			{
				if(onCompleted != null) onCompleted();
			}
		}
		
		public static IIteratorLink<TSource, TResult> Select<TSource, TResult>(Func<TSource, TResult> selector)
		{
			return Select<TSource, TSource, TResult>(null, selector);
		}
		
		public static IIteratorLink<TSource, TResult> Select<TSource, TIntermediate, TResult>(this IIteratorLink<TSource, TIntermediate> source, Func<TIntermediate, TResult> selector)
		{
			return new SelectIterator<TSource, TIntermediate, TResult>(source, selector);
		}
		
		private class SelectIterator<TSource, TIntermediate, TResult> : BasicIteratorLink<TSource, TIntermediate, TResult>
		{
			readonly Func<TIntermediate, TResult> selector;
			
			public SelectIterator(IIteratorLink<TSource, TIntermediate> source, Func<TIntermediate, TResult> selector) : base(source)
			{
				this.selector = selector;
			}
			
			protected override bool OnNextInternal(TIntermediate value)
			{
				return OnNextFinal(selector(value));
			}
		}
		
		public static IIteratorLink<TSource, TResult> Link<TSource, TIntermediate, TResult>(this IIteratorLink<TSource, TIntermediate> left, IIteratorLink<TIntermediate, TResult> right)
		{
			return new LinkIterator<TSource, TIntermediate, TResult>(left, right);
		}
		
		private class LinkIterator<TSource, TIntermediate, TResult> : BasicIteratorLink<TSource, TIntermediate, TResult>
		{
			readonly IIteratorLink<TIntermediate, TResult> target;
			readonly FinalIterator finIterator;
			
			public LinkIterator(IIteratorLink<TSource, TIntermediate> source, IIteratorLink<TIntermediate, TResult> target) : base(source)
			{
				this.target = target;
				finIterator = new FinalIterator(this);
			}
			
			protected override bool OnNextInternal(TIntermediate value)
			{
				using(var handle = target.Subscribe(finIterator))
				{
					return target.OnNext(value);
				}
			}
			
			protected override void OnCompletedInternal()
			{
				using(var handle = target.Subscribe(finIterator))
				{
					target.OnCompleted();
				}
			}
		}
		
		public static IIteratorLink<TSource, TSource> Where<TSource>(Func<TSource, bool> predicate)
		{
			return Where<TSource, TSource>(null, predicate);
		}
		
		public static IIteratorLink<TSource, TResult> Where<TSource, TResult>(this IIteratorLink<TSource, TResult> source, Func<TResult, bool> predicate)
		{
			return new WhereIterator<TSource, TResult>(source, predicate);
		}
		
		private class WhereIterator<TSource, TResult> : BasicIteratorLink<TSource, TResult, TResult>
		{
			readonly Func<TResult, bool> predicate;
			
			public WhereIterator(IIteratorLink<TSource, TResult> source, Func<TResult, bool> predicate) : base(source)
			{
				this.predicate = predicate;
			}
			
			protected override bool OnNextInternal(TResult value)
			{
				if(predicate(value))
				{
					return OnNextFinal(value);
				}else{
					return true;
				}
			}
		}
		
		public static IIteratorLink<TSource, TResult> Aggregate<TSource, TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
		{
			return Aggregate<TSource, TSource, TAccumulate, TResult>(null, seed, func, resultSelector);
		}
		
		public static IIteratorLink<TSource, TResult> Aggregate<TSource, TIntermediate, TAccumulate, TResult>(this IIteratorLink<TSource, TIntermediate> source, TAccumulate seed, Func<TAccumulate, TIntermediate, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
		{
			return new AggregateIterator<TSource, TIntermediate, TAccumulate, TResult>(source, seed, func, resultSelector);
		}
		
		private class AggregateIterator<TSource, TIntermediate, TAccumulate, TResult> : BasicIteratorLink<TSource, TIntermediate, TResult>
		{
			readonly TAccumulate seed;
			readonly Func<TAccumulate, TIntermediate, TAccumulate> func;
			readonly Func<TAccumulate, TResult> resultSelector;
			
			TAccumulate accumulate;
			
			public AggregateIterator(IIteratorLink<TSource, TIntermediate> source, TAccumulate seed, Func<TAccumulate, TIntermediate, TAccumulate> func, Func<TAccumulate, TResult> resultSelector) : base(source)
			{
				this.seed = seed;
				this.func = func;
				this.resultSelector = resultSelector;
				
				accumulate = seed;
			}
			
			protected override bool OnNextInternal(TIntermediate value)
			{
				accumulate = func(accumulate, value);
				return true;
			}
			
			protected override void OnCompletedInternal()
			{
				OnNextFinal(resultSelector(accumulate));
				OnCompletedFinal();
				accumulate = seed;
			}
		}
	}
}
