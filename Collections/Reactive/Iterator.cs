/* Date: 30.10.2017, Time: 1:38 */
using System;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.Collections.Reactive
{
	public static class Iterator
	{
		public static IIterator<T> Create<T>(Func<T, bool> onNext=null, Action onCompleted=null)
		{
			return new ProcIterator<T>(onNext, onCompleted);
		}
		
		private class ProcIterator<T> : IIterator<T>
		{
			readonly Func<T, bool> onNext;
			readonly Action onCompleted;
			
			public ProcIterator(Func<T, bool> onNext, Action onCompleted)
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
		
		public static IIteratorLink<TSource, TResult> Select<TSource, TArgument, TResult>(this IIteratorLink<TSource, TArgument> source, Func<TArgument, TResult> selector)
		{
			return new SelectIterator<TSource, TArgument, TResult>(source, selector);
		}
		
		private class SelectIterator<TSource, TArgument, TResult> : IIteratorLink<TSource, TResult>
		{
			readonly List<Handler> handlers = new List<Handler>();
			readonly IIteratorLink<TSource, TArgument> source;
			readonly Func<TArgument, TResult> selector;
			readonly ArgumentIterator argIterator;
			
			public SelectIterator(IIteratorLink<TSource, TArgument> source, Func<TArgument, TResult> selector)
			{
				this.selector = selector;
				this.source = source;
				argIterator = new ArgumentIterator(this);
			}
			
			private class ArgumentIterator : IIterator<TArgument>
			{
				readonly SelectIterator<TSource, TArgument, TResult> parent;
				
				public ArgumentIterator(SelectIterator<TSource, TArgument, TResult> parent)
				{
					this.parent = parent;
				}
				
				public bool OnNext(TArgument value)
				{
					return parent.OnNextInternal(value);
				}
				
				public void OnCompleted()
				{
					parent.OnCompletedInternal();
				}
			}
			
			public bool OnNext(TSource value)
			{
				if(source == null)
				{
					return OnNextInternal(To<TArgument>.Cast(value));
				}else using(var handle = source.Subscribe(argIterator))
				{
					return source.OnNext(value);
				}
			}
			
			private bool OnNextInternal(TArgument value)
			{
				bool next = false;
				var obj = selector(value);
				foreach(var handler in handlers)
				{
					if(handler.Handle)
					{
						if(handler.Iterator.OnNext(obj))
						{
							next = true;
						}else{
							handler.Iterator.OnCompleted();
							handler.Close();
						}
					}
				}
				return next;
			}
			
			public void OnCompleted()
			{
				if(source == null)
				{
					OnCompletedInternal();
				}else using(var handle = source.Subscribe(argIterator))
				{
					source.OnCompleted();
				}
			}
			
			private void OnCompletedInternal()
			{
				foreach(var handler in handlers)
				{
					if(handler.Handle)
					{
						handler.Iterator.OnCompleted();
					}else{
						handler.Open();
					}
				}
			}
			
			public IDisposable Subscribe(IIterator<TResult> iterator)
			{
				var handler = new Handler(iterator);
				handlers.Add(handler);
				return handler;
			}
			
			public void Dispose()
			{
				Dispose(true);
			}
			
			protected void Dispose(bool disposing)
			{
				if(disposing)
				{
					foreach(var handler in handlers)
					{
						handler.Dispose();
					}
					handlers.Clear();
				}
				GC.SuppressFinalize(this);
			}
			
			~SelectIterator()
			{
				Dispose(false);
			}
			
			private class Handler : IDisposable
			{
				public IIterator<TResult> Iterator{get; private set;}
				public bool Handle{get; private set;}
				
				public Handler(IIterator<TResult> iterator)
				{
					Handle = true;
					Iterator = iterator;
				}
				
				public void Close()
				{
					if(Iterator != null)
					{
						Handle = false;
					}
				}
				
				public void Open()
				{
					if(Iterator != null)
					{
						Handle = true;
					}
				}
				
				public void Dispose()
				{
					Dispose(true);
				}
				
				protected void Dispose(bool disposing)
				{
					Handle = false;
					if(disposing)
					{
						Iterator = null;
					}
					GC.SuppressFinalize(this);
				}
				
				~Handler()
				{
					Dispose(false);
				}
			}
		}
		
		public static IIteratorLink<TSource, TResult> Link<TSource, TIntermediate, TResult>(this IIteratorLink<TSource, TIntermediate> left, IIteratorLink<TIntermediate, TResult> right)
		{
			return new LinkIterator<TSource, TIntermediate, TResult>(left, right);
		}
		
		private class LinkIterator<TSource, TIntermediate, TResult> : IIteratorLink<TSource, TResult>
		{
			readonly List<Handler> handlers = new List<Handler>();
			readonly IIteratorLink<TSource, TIntermediate> left;
			readonly IIteratorLink<TIntermediate, TResult> right;
			readonly IntermediateIterator inIterator;
			readonly FinalIterator finIterator;
			
			public LinkIterator(IIteratorLink<TSource, TIntermediate> left, IIteratorLink<TIntermediate, TResult> right)
			{
				this.left = left;
				this.right = right;
				inIterator = new IntermediateIterator(this);
				finIterator = new FinalIterator(this);
			}
			
			private class IntermediateIterator : IIterator<TIntermediate>
			{
				readonly LinkIterator<TSource, TIntermediate, TResult> parent;
				
				public IntermediateIterator(LinkIterator<TSource, TIntermediate, TResult> parent)
				{
					this.parent = parent;
				}
				
				public bool OnNext(TIntermediate value)
				{
					return parent.OnNextInternal(value);
				}
				
				public void OnCompleted()
				{
					parent.OnCompletedInternal();
				}
			}
			
			private class FinalIterator : IIterator<TResult>
			{
				readonly LinkIterator<TSource, TIntermediate, TResult> parent;
				
				public FinalIterator(LinkIterator<TSource, TIntermediate, TResult> parent)
				{
					this.parent = parent;
				}
				
				public bool OnNext(TResult value)
				{
					return parent.OnNextFinal(value);
				}
				
				public void OnCompleted()
				{
					parent.OnCompletedFinal();
				}
			}
			
			public bool OnNext(TSource value)
			{
				using(var handle = left.Subscribe(inIterator))
				{
					return left.OnNext(value);
				}
			}
			
			private bool OnNextInternal(TIntermediate value)
			{
				using(var handle = right.Subscribe(finIterator))
				{
					return right.OnNext(value);
				}
			}
			
			private bool OnNextFinal(TResult value)
			{
				bool next = false;
				foreach(var handler in handlers)
				{
					if(handler.Handle)
					{
						if(handler.Iterator.OnNext(value))
						{
							next = true;
						}else{
							handler.Iterator.OnCompleted();
							handler.Close();
						}
					}
				}
				return next;
			}
			
			public void OnCompleted()
			{
				using(var handle = left.Subscribe(inIterator))
				{
					left.OnCompleted();
				}
			}
			
			private void OnCompletedInternal()
			{
				using(var handle = right.Subscribe(finIterator))
				{
					right.OnCompleted();
				}
			}
			
			private void OnCompletedFinal()
			{
				foreach(var handler in handlers)
				{
					if(handler.Handle)
					{
						handler.Iterator.OnCompleted();
					}else{
						handler.Open();
					}
				}
			}
			
			public IDisposable Subscribe(IIterator<TResult> iterator)
			{
				var handler = new Handler(iterator);
				handlers.Add(handler);
				return handler;
			}
			
			public void Dispose()
			{
				Dispose(true);
			}
			
			protected void Dispose(bool disposing)
			{
				if(disposing)
				{
					foreach(var handler in handlers)
					{
						handler.Dispose();
					}
					handlers.Clear();
				}
				GC.SuppressFinalize(this);
			}
			
			~LinkIterator()
			{
				Dispose(false);
			}
			
			private class Handler : IDisposable
			{
				public IIterator<TResult> Iterator{get; private set;}
				public bool Handle{get; private set;}
				
				public Handler(IIterator<TResult> iterator)
				{
					Handle = true;
					Iterator = iterator;
				}
				
				public void Close()
				{
					if(Iterator != null)
					{
						Handle = false;
					}
				}
				
				public void Open()
				{
					if(Iterator != null)
					{
						Handle = true;
					}
				}
				
				public void Dispose()
				{
					Dispose(true);
				}
				
				protected void Dispose(bool disposing)
				{
					Handle = false;
					if(disposing)
					{
						Iterator = null;
					}
					GC.SuppressFinalize(this);
				}
				
				~Handler()
				{
					Dispose(false);
				}
			}
		}
	}
}
