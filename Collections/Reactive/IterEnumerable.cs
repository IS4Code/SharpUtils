/* Date: 29.10.2017, Time: 2:49 */
using System;
using System.Collections.Generic;
using IllidanS4.SharpUtils.Threads;

namespace IllidanS4.SharpUtils.Collections.Reactive
{
	/// <summary>
	/// Provides a way to represent a push iterator as a pull iterator, using fibers.
	/// </summary>
	public class IterEnumerable<T> : IEnumerable<T>
	{
		readonly Action<Func<T, bool>> iterProc;
		readonly IFiberFactory fiberFactory;
		
		public IterEnumerable(Action<Func<T, bool>> iterProc, IFiberFactory fiberFactory)
		{
			this.iterProc = iterProc;
			this.fiberFactory = fiberFactory;
		}
		
		public IterEnumerable(IIterable<T> iterable, IFiberFactory fiberFactory) : this(f => iterable.Iterate(Iterator.Create(f)), fiberFactory)
		{
			
		}
		
		public IEnumerator<T> GetEnumerator()
		{
			return new Enumerator(iterProc, fiberFactory);
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		sealed class Enumerator : IEnumerator<T>
		{
			readonly FiberBase enumFiber;
			readonly IFiberFactory fiberFactory;
			
			
			int state;
			
			public T Current{
				get; private set;
			}
			
			FiberBase mainFiber;
			
			public Enumerator(Action<Func<T, bool>> enumProc, IFiberFactory fiberFactory)
			{
				this.fiberFactory = fiberFactory;
				enumFiber = fiberFactory.CreateNew(
					()=>{
						enumProc(FiberNext);
						state = -1;
						mainFiber.Switch();
					}
				);
			}
			
			bool FiberNext(T obj)
			{
				Current = obj;
				state = 1;
				mainFiber.Switch();
				return state != -1;
			}
			
			object System.Collections.IEnumerator.Current{
				get{
					return state == 1 ? (object)Current : null;
				}
			}
			
			public bool MoveNext()
			{
				if(state == 0 || state == 1)
				{
					mainFiber = fiberFactory.CurrentFiber;
					try{
						enumFiber.Switch();
					}finally{
						mainFiber = null;
					}
					
					if(state == 1)
					{
						return true;
					}
					enumFiber.Dispose();
					return false;
				}
				return false;
			}
			
			public void Dispose()
			{
				if(state == 0)
				{
					state = -1;
					enumFiber.Dispose();
				}else if(state != -1)
				{
					state = -1;
					mainFiber = fiberFactory.CurrentFiber;
					try{
						enumFiber.Switch();
					}finally{
						mainFiber = null;
					}
					enumFiber.Dispose();
				}
			}
			
			public void Reset()
			{
				throw new NotSupportedException();
			}
		}
	}
	
	public static class IterEnumerable
	{
		public static IterEnumerable<T> Create<T>(Action<Func<T, bool>> iterProc, IFiberFactory fiberFactory)
		{
			return new IterEnumerable<T>(iterProc, fiberFactory);
		}
		
		public static IterEnumerable<T> Create<T>(IIterable<T> iterable, IFiberFactory fiberFactory)
		{
			return new IterEnumerable<T>(iterable, fiberFactory);
		}
	}
}
