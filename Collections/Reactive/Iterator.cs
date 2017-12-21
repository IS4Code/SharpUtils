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
			return new CustomAggregateIterator<TSource, TIntermediate, TAccumulate, TResult>(source, seed, func, resultSelector);
		}
		
		private abstract class AggregateIterator<TSource, TIntermediate, TAccumulate, TResult> : BasicIteratorLink<TSource, TIntermediate, TResult>
		{
			protected TAccumulate accumulate;
			
			public AggregateIterator(IIteratorLink<TSource, TIntermediate> source) : base(source)
			{
				
			}
			
			protected abstract override bool OnNextInternal(TIntermediate value);
			
			protected abstract override void OnCompletedInternal();
		}
		
		private class CustomAggregateIterator<TSource, TIntermediate, TAccumulate, TResult> : AggregateIterator<TSource, TIntermediate, TAccumulate, TResult>
		{
			readonly TAccumulate seed;
			readonly Func<TAccumulate, TIntermediate, TAccumulate> func;
			readonly Func<TAccumulate, TResult> resultSelector;
			
			public CustomAggregateIterator(IIteratorLink<TSource, TIntermediate> source, TAccumulate seed, Func<TAccumulate, TIntermediate, TAccumulate> func, Func<TAccumulate, TResult> resultSelector) : base(source)
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
		
		public static IIteratorLink<TSource, int> Sum<TSource>(this IIteratorLink<TSource, int> source)
		{
			return new Int32SumIterator<TSource>(source);
		}
		
		public static IIteratorLink<TSource, long> Sum<TSource>(this IIteratorLink<TSource, long> source)
		{
			return new Int64SumIterator<TSource>(source);
		}
		
		public static IIteratorLink<TSource, float> Sum<TSource>(this IIteratorLink<TSource, float> source)
		{
			return new SingleSumIterator<TSource>(source);
		}
		
		public static IIteratorLink<TSource, double> Sum<TSource>(this IIteratorLink<TSource, double> source)
		{
			return new DoubleSumIterator<TSource>(source);
		}
		
		public static IIteratorLink<TSource, decimal> Sum<TSource>(this IIteratorLink<TSource, decimal> source)
		{
			return new DecimalSumIterator<TSource>(source);
		}
		
		public static IIteratorLink<TSource, int?> Sum<TSource>(this IIteratorLink<TSource, int?> source)
		{
			return new NullableInt32SumIterator<TSource>(source);
		}
		
		public static IIteratorLink<TSource, long?> Sum<TSource>(this IIteratorLink<TSource, long?> source)
		{
			return new NullableInt64SumIterator<TSource>(source);
		}
		
		public static IIteratorLink<TSource, float?> Sum<TSource>(this IIteratorLink<TSource, float?> source)
		{
			return new NullableSingleSumIterator<TSource>(source);
		}
		
		public static IIteratorLink<TSource, double?> Sum<TSource>(this IIteratorLink<TSource, double?> source)
		{
			return new NullableDoubleSumIterator<TSource>(source);
		}
		
		public static IIteratorLink<TSource, decimal?> Sum<TSource>(this IIteratorLink<TSource, decimal?> source)
		{
			return new NullableDecimalSumIterator<TSource>(source);
		}
		
		private abstract class SumIterator<TSource, TElement> : AggregateIterator<TSource, TElement, TElement, TElement>
		{
			public SumIterator(IIteratorLink<TSource, TElement> source) : base(source)
			{
				
			}
			
			protected abstract TElement Add(TElement a, TElement b);
			
			protected sealed override bool OnNextInternal(TElement value)
			{
				accumulate = Add(accumulate, value);
				return true;
			}
			
			protected sealed override void OnCompletedInternal()
			{
				OnNextFinal(accumulate);
				OnCompletedFinal();
				accumulate = default(TElement);
			}
		}
		
		private abstract class NullableSumIterator<TSource, TElement> : AggregateIterator<TSource, TElement?, TElement, TElement?> where TElement : struct
		{
			public NullableSumIterator(IIteratorLink<TSource, TElement?> source) : base(source)
			{
				
			}
			
			protected abstract TElement Add(TElement a, TElement b);
			
			protected sealed override bool OnNextInternal(TElement? value)
			{
				if(value != null)
				{
					accumulate = Add(accumulate, (TElement)value);
				}
				return true;
			}
			
			protected sealed override void OnCompletedInternal()
			{
				OnNextFinal(accumulate);
				OnCompletedFinal();
				accumulate = default(TElement);
			}
		}
		
		private class Int32SumIterator<TSource> : SumIterator<TSource, int>
		{
			public Int32SumIterator(IIteratorLink<TSource, int> source) : base(source)
			{
				
			}
			
			protected override int Add(int a, int b)
			{
				return checked(a + b);
			}
		}
		
		private class Int64SumIterator<TSource> : SumIterator<TSource, long>
		{
			public Int64SumIterator(IIteratorLink<TSource, long> source) : base(source)
			{
				
			}
			
			protected override long Add(long a, long b)
			{
				return checked(a + b);
			}
		}
		
		private class SingleSumIterator<TSource> : SumIterator<TSource, float>
		{
			public SingleSumIterator(IIteratorLink<TSource, float> source) : base(source)
			{
				
			}
			
			protected override float Add(float a, float b)
			{
				return a + b;
			}
		}
		
		private class DoubleSumIterator<TSource> : SumIterator<TSource, double>
		{
			public DoubleSumIterator(IIteratorLink<TSource, double> source) : base(source)
			{
				
			}
			
			protected override double Add(double a, double b)
			{
				return a + b;
			}
		}
		
		private class DecimalSumIterator<TSource> : SumIterator<TSource, decimal>
		{
			public DecimalSumIterator(IIteratorLink<TSource, decimal> source) : base(source)
			{
				
			}
			
			protected override decimal Add(decimal a, decimal b)
			{
				return a + b;
			}
		}
		
		private class NullableInt32SumIterator<TSource> : NullableSumIterator<TSource, int>
		{
			public NullableInt32SumIterator(IIteratorLink<TSource, int?> source) : base(source)
			{
				
			}
			
			protected override int Add(int a, int b)
			{
				return checked(a + b);
			}
		}
		
		private class NullableInt64SumIterator<TSource> : NullableSumIterator<TSource, long>
		{
			public NullableInt64SumIterator(IIteratorLink<TSource, long?> source) : base(source)
			{
				
			}
			
			protected override long Add(long a, long b)
			{
				return checked(a + b);
			}
		}
		
		private class NullableSingleSumIterator<TSource> : NullableSumIterator<TSource, float>
		{
			public NullableSingleSumIterator(IIteratorLink<TSource, float?> source) : base(source)
			{
				
			}
			
			protected override float Add(float a, float b)
			{
				return a + b;
			}
		}
		
		private class NullableDoubleSumIterator<TSource> : NullableSumIterator<TSource, double>
		{
			public NullableDoubleSumIterator(IIteratorLink<TSource, double?> source) : base(source)
			{
				
			}
			
			protected override double Add(double a, double b)
			{
				return a + b;
			}
		}
		
		private class NullableDecimalSumIterator<TSource> : NullableSumIterator<TSource, decimal>
		{
			public NullableDecimalSumIterator(IIteratorLink<TSource, decimal?> source) : base(source)
			{
				
			}
			
			protected override decimal Add(decimal a, decimal b)
			{
				return a + b;
			}
		}
		
		public delegate bool ProcessingFunc<in TSource, out TResult>(TSource value, Func<TResult, bool> onNext, Action onCompleted);
		public delegate void ProcessingAction<in TSource, out TResult>(TSource value, Func<TResult, bool> onNext, Action onCompleted);
		
		private class ProcessIterator<TSource, TIntermediate, TResult> : BasicIteratorLink<TSource, TIntermediate, TResult>
		{
			readonly ProcessingFunc<TIntermediate, TResult> processingFunc;
			
			public ProcessIterator(IIteratorLink<TSource, TIntermediate> source, ProcessingFunc<TIntermediate, TResult> processingFunc) : base(source)
			{
				if(processingFunc == null) throw new ArgumentNullException("processingFunc");
				
				this.processingFunc = processingFunc;
			}
			
			public ProcessIterator(IIteratorLink<TSource, TIntermediate> source, ProcessingAction<TIntermediate, TResult> processingAction) : this(source, (v, n, c) => {processingAction(v, n, c); return true;})
			{
				if(processingAction == null) throw new ArgumentNullException("processingAction");
			}
			
			protected override bool OnNextInternal(TIntermediate value)
			{
				return processingFunc(value, OnNextFinal, OnCompletedFinal);
			}
		}
		
		public static IIteratorLink<TSource, TResult> Process<TSource, TResult>(ProcessingFunc<TSource, TResult> processingFunc)
		{
			return Process<TSource, TSource, TResult>(null, processingFunc);
		}
		
		public static IIteratorLink<TSource, TResult> Process<TSource, TIntermediate, TResult>(this IIteratorLink<TSource, TIntermediate> source, ProcessingFunc<TIntermediate, TResult> processingFunc)
		{
			return new ProcessIterator<TSource, TIntermediate, TResult>(source, processingFunc);
		}
		
		public static IIteratorLink<TSource, TResult> Process<TSource, TResult>(ProcessingAction<TSource, TResult> processingAction)
		{
			return Process<TSource, TSource, TResult>(null, processingAction);
		}
		
		public static IIteratorLink<TSource, TResult> Process<TSource, TIntermediate, TResult>(this IIteratorLink<TSource, TIntermediate> source, ProcessingAction<TIntermediate, TResult> processingAction)
		{
			return new ProcessIterator<TSource, TIntermediate, TResult>(source, processingAction);
		}
	}
}
