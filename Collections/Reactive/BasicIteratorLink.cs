/* Date: 31.10.2017, Time: 23:44 */
using System;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.Collections.Reactive
{
	public abstract class BasicIteratorLink<TSource, TIntermediate, TResult> : IIteratorLink<TSource, TResult>
	{
		readonly List<Handler> handlers = new List<Handler>();
		readonly IIteratorLink<TSource, TIntermediate> source;
		readonly IntermediateIterator inIterator;
		
		public BasicIteratorLink(IIteratorLink<TSource, TIntermediate> source)
		{
			this.source = source;
			inIterator = new IntermediateIterator(this);
		}
		
		protected class InitialIterator : IIterator<TSource>
		{
			readonly BasicIteratorLink<TSource, TIntermediate, TResult> parent;
			
			public InitialIterator(BasicIteratorLink<TSource, TIntermediate, TResult> parent)
			{
				this.parent = parent;
			}
			
			public bool OnNext(TSource value)
			{
				return parent.OnNext(value);
			}
			
			public void OnCompleted()
			{
				parent.OnCompleted();
			}
		}
		
		protected class IntermediateIterator : IIterator<TIntermediate>
		{
			readonly BasicIteratorLink<TSource, TIntermediate, TResult> parent;
			
			public IntermediateIterator(BasicIteratorLink<TSource, TIntermediate, TResult> parent)
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
		
		protected class FinalIterator : IIterator<TResult>
		{
			readonly BasicIteratorLink<TSource, TIntermediate, TResult> parent;
			
			public FinalIterator(BasicIteratorLink<TSource, TIntermediate, TResult> parent)
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
			if(source == null)
			{
				return OnNextInternal(To<TIntermediate>.Cast(value));
			}else using(var handle = source.Subscribe(inIterator))
			{
				return source.OnNext(value);
			}
		}
		
		protected virtual bool OnNextInternal(TIntermediate value)
		{
			return OnNextFinal(To<TResult>.Cast(value));
		}
		
		public void OnCompleted()
		{
			if(source == null)
			{
				OnCompletedInternal();
			}else using(var handle = source.Subscribe(inIterator))
			{
				source.OnCompleted();
			}
		}
		
		protected virtual void OnCompletedInternal()
		{
			OnCompletedFinal();
		}
		
		protected virtual bool OnNextFinal(TResult value)
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
		
		protected virtual void OnCompletedFinal()
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
		
		~BasicIteratorLink()
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
