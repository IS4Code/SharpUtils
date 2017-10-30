/* Date: 29.10.2017, Time: 2:49 */
using System;
using System.Collections.Generic;
using IllidanS4.SharpUtils.Threads;

namespace IllidanS4.SharpUtils.Collections.Reactive
{
	/// <summary>
	/// Provides a way to represent a push iterator as a pull iterator.
	/// </summary>
	public class ProcEnumerable<T> : IEnumerable<T>
	{
		readonly Action<Func<T, bool>> enumProc;
		
		public ProcEnumerable(Action<Func<T, bool>> enumProc)
		{
			this.enumProc = enumProc;
		}
		
		public IEnumerator<T> GetEnumerator()
		{
			return new ProcEnumerator(enumProc);
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		sealed class ProcEnumerator : IEnumerator<T>
		{
			readonly Fiber enumFiber;
			
			
			int state;
			
			public T Current{
				get; private set;
			}
			
			Fiber mainFiber;
			
			public ProcEnumerator(Action<Func<T, bool>> enumProc)
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
				if(state != -1)
				{
					state = -1;
					mainFiber = Fiber.CurrentFiber;
					try{
						enumFiber.Switch();
					}finally{
						mainFiber = null;
					}
				}
			}
			
			public void Reset()
			{
				throw new NotSupportedException();
			}
		}
	}
}
