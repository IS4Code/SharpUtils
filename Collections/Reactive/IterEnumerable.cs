/* Date: 29.10.2017, Time: 2:49 */
using System;
using System.Collections.Generic;
using IllidanS4.SharpUtils.Threads;

namespace IllidanS4.SharpUtils.Collections.Reactive
{
	/// <summary>
	/// Provides a way to represent a push iterator as a pull iterator.
	/// </summary>
	public class IterEnumerable<T> : IEnumerable<T>
	{
		readonly Action<Func<T, bool>> iterProc;
		
		public IterEnumerable(Action<Func<T, bool>> iterProc)
		{
			this.iterProc = iterProc;
		}
		
		public IterEnumerable(IIterable<T> iterable) : this(f => iterable.Iterate(Iterator.Create(f)))
		{
			
		}
		
		public IEnumerator<T> GetEnumerator()
		{
			return new Enumerator(iterProc);
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		sealed class Enumerator : IEnumerator<T>
		{
			readonly Fiber enumFiber;
			
			
			int state;
			
			public T Current{
				get; private set;
			}
			
			Fiber mainFiber;
			
			public Enumerator(Action<Func<T, bool>> enumProc)
			{
				enumFiber = new Fiber(
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
					mainFiber = Fiber.CurrentFiber;
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
					mainFiber = Fiber.CurrentFiber;
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
		public static IterEnumerable<T> Create<T>(Action<Func<T, bool>> iterProc)
		{
			return new IterEnumerable<T>(iterProc);
		}
		
		public static IterEnumerable<T> Create<T>(IIterable<T> iterable)
		{
			return new IterEnumerable<T>(iterable);
		}
	}
}
