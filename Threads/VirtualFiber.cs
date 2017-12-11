/* Date: 25.11.2017, Time: 13:01 */
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace IllidanS4.SharpUtils.Threads
{
	public class VirtualFiber : FiberBase
	{
		/// <summary>
		/// The factory object that can be used to create new fibers.
		/// </summary>
		public static readonly IFiberFactory Factory = new FiberFactory();
		
		/// <summary>
		/// Gets the currently executing fiber.
		/// </summary>
		public static VirtualFiber CurrentFiber{
			get{
				return ThreadFibers.Value;
			}
		}
		
		Thread m_thread;
		AutoResetEvent reset;
		ManualResetEvent done;
		
		private static readonly ConcurrentDictionary<Thread, VirtualFiber> FiberMap = new ConcurrentDictionary<Thread, VirtualFiber>();
		private static readonly ThreadLocal<VirtualFiber> ThreadFibers = new ThreadLocal<VirtualFiber>(FindFiber);
		
		private static VirtualFiber FindFiber()
		{
			VirtualFiber fiber;
			if(FiberMap.TryGetValue(Thread.CurrentThread, out fiber))
			{
				return fiber;
			}
			return new VirtualFiber();
		}
		
		public VirtualFiber(ThreadStart start) : this(start, 0)
		{
			
		}
		
		public VirtualFiber(ThreadStart start, int maxStackSize)
		{
			m_thread = new Thread(()=>ThreadRoutine(start), maxStackSize);
			m_thread.IsBackground = true;
			reset = new AutoResetEvent(false);
			done = new ManualResetEvent(false);
			FiberMap[m_thread] = this;
			state = FiberState.Suspended;
			m_thread.Start();
		}
		
		public VirtualFiber(ParameterizedThreadStart start, object parameter) : this(start, 0, parameter)
		{
			
		}
		
		public VirtualFiber(ParameterizedThreadStart start, int maxStackSize, object parameter) : this(()=>start(parameter), maxStackSize)
		{
			
		}
		
		private VirtualFiber()
		{
			m_thread = Thread.CurrentThread;
			reset = new AutoResetEvent(false);
			done = new ManualResetEvent(false);
			state = FiberState.Running;
		}
		
		FiberState state;
		
		public override FiberState State{
			get{
				return state;
			}
		}
		
		private void Wait(params WaitHandle[] waitHandles)
		{
			bool background = Thread.CurrentThread.IsBackground;
			try{
				Thread.CurrentThread.IsBackground = true;
				CurrentFiber.state = FiberState.Suspended;
				WaitHandle.WaitAny(waitHandles);
				CurrentFiber.state = FiberState.Running;
			}catch(ThreadAbortException)
			{
				CurrentFiber.state = FiberState.Terminated;
				m_thread = null;
				done.Set();
			}finally{
				Thread.CurrentThread.IsBackground = background;
			}
		}
		
		private void Wait()
		{
			Wait(reset);
		}
		
		private void WaitOrJoin(VirtualFiber fiber)
		{
			Wait(reset, fiber.done);
		}
		
		private void ThreadRoutine(ThreadStart start)
		{
			Wait();
			if(m_thread == null) return;
			state = FiberState.Running;
			m_thread.IsBackground = false;
			start();
			state = FiberState.Terminated;
			reset = null;
			done.Set();
			m_thread = null;
			Dispose();
		}
		
		public override void Switch()
		{
			if(reset == null)
			{
				throw new ObjectDisposedException(null);
			}
			state = FiberState.Running;
			reset.Set();
			CurrentFiber.WaitOrJoin(this);
		}
		
		public override void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				state = FiberState.Terminated;
				reset = null;
				if(m_thread != null)
				{
					m_thread.IsBackground = true;
					m_thread.Abort();
					m_thread = null;
				}
			}
		}
		
		~VirtualFiber()
		{
			Dispose(false);
		}
		
		class FiberFactory : IFiberFactory
		{
			public FiberBase CurrentFiber{
				get{
					return VirtualFiber.CurrentFiber;
				}
			}
			
			public FiberBase CreateNew(ThreadStart start)
			{
				return new VirtualFiber(start);
			}
			
			public FiberBase CreateNew(ThreadStart start, int maxStackSize)
			{
				return new VirtualFiber(start, maxStackSize);
			}
			
			public FiberBase CreateNew(ParameterizedThreadStart start, object parameter)
			{
				return new VirtualFiber(start, parameter);
			}
			
			public FiberBase CreateNew(ParameterizedThreadStart start, int maxStackSize, object parameter)
			{
				return new VirtualFiber(start, maxStackSize, parameter);
			}
		}
	}
}
