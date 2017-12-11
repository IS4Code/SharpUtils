/* Date: 29.10.2017, Time: 2:43 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using IllidanS4.SharpUtils.Interop;

namespace IllidanS4.SharpUtils.Threads
{
	/// <summary>
	/// Managed API for creating and using fibers.
	/// </summary>
	/// <remarks>
	/// Fiber is a component of a thread with its own stack, whose execution is scheduled manually.
	/// </remarks>
	public class Fiber : FiberBase
	{
		/// <summary>
		/// The factory object that can be used to create new fibers.
		/// </summary>
		public static readonly IFiberFactory Factory = new FiberFactory();
		
		/// <summary>
		/// Gets the currently executing fiber.
		/// </summary>
		public static Fiber CurrentFiber{
			get{
				return StartupFibers();
			}
		}
		
		private static readonly ThreadLocal<List<Fiber>> ThreadFibers = new ThreadLocal<List<Fiber>>(()=>new List<Fiber>());
		
		protected IntPtr Handle{get; private set;}
		
		/// <summary>
		/// Gets the thread in which the fiber was created.
		/// </summary>
		public Thread Thread{get; private set;}
		
		protected object Parameter{get; private set;}
		
		private IntPtr m_fiber;
		
		public Fiber(ThreadStart start) : this(start, 0)
		{
			
		}
		
		public Fiber(ThreadStart start, int maxStackSize) : this()
		{
			StartupFibers();
			
			m_fiber = Kernel32.CreateFiber(maxStackSize, _=>{start(); state= FiberState.Terminated;}, Handle);
			state = FiberState.Suspended;
		}
		
		public Fiber(ParameterizedThreadStart start, object parameter) : this(start, 0, parameter)
		{
			
		}
		
		public Fiber(ParameterizedThreadStart start, int maxStackSize, object parameter) : this(()=>start(CurrentFiber.Parameter), maxStackSize)
		{
			Parameter = parameter;
		}
		
		private Fiber()
		{
			var handle = GCHandle.Alloc(this, GCHandleType.Weak);
			Handle = GCHandle.ToIntPtr(handle);
			Thread = Thread.CurrentThread;
			state = FiberState.Running;
			ThreadFibers.Value.Add(this);
		}
		
		FiberState state;
		
		public override FiberState State{
			get{
				return state;
			}
		}
		
		public override void Switch()
		{
			if(m_fiber == IntPtr.Zero) throw new ObjectDisposedException(null);
			state = FiberState.Running;
			CurrentFiber.state = FiberState.Suspended;
			Kernel32.SwitchToFiber(m_fiber);
		}
		
		public override void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if(Handle != IntPtr.Zero)
			{
				var handle = GCHandle.FromIntPtr(Handle);
				handle.Free();
				Handle = IntPtr.Zero;
				
				if(m_fiber != IntPtr.Zero)
				{
					Kernel32.DeleteFiber(m_fiber);
					m_fiber = IntPtr.Zero;
				}
				
				state = FiberState.Terminated;
				
				try{
					ThreadFibers.Value.Remove(this);
					CleanupFibers();
				}catch(ObjectDisposedException)
				{
					
				}
			}
		}
		
		~Fiber()
		{
			Dispose(false);
		}
		
		private static Fiber StartupFibers()
		{
			var initFiber = new Fiber();
			try{
				IntPtr fiber = Kernel32.ConvertThreadToFiber(initFiber.Handle);
				if(fiber == IntPtr.Zero)
				{
					initFiber.Dispose();
					IntPtr handle = Kernel32.GetFiberData();
					initFiber = (Fiber)GCHandle.FromIntPtr(handle).Target;
					return initFiber;
				}
				initFiber.m_fiber = fiber;
				return initFiber;
			}catch{
				initFiber.Dispose();
				throw;
			}
		}
		
		private static void CleanupFibers()
		{
			var list = ThreadFibers.Value;
			if(list.Count > 1)
			{
				return;
			}else if(list.Count == 1)
			{
				var fiber = list[0];
				if(fiber.m_fiber != Kernel32.GetCurrentFiber()) return;
				fiber.m_fiber = IntPtr.Zero;
				fiber.Dispose();
				return;
			}
			Kernel32.ConvertFiberToThread();
		}
		
		static class Kernel32
		{
			[DllImport("kernel32.dll", SetLastError=true, EntryPoint="SwitchToFiber")]
			private static extern void _SwitchToFiber(IntPtr lpFiber);
			
			public static void SwitchToFiber(IntPtr lpFiber)
			{
				if(lpFiber == GetCurrentFiber())
				{
					throw new ArgumentException("Cannot switch to the current fiber.", "lpFiber");
				}
				_SwitchToFiber(lpFiber);
			}
			
			[DllImport("kernel32.dll", SetLastError=true, EntryPoint="ConvertFiberToThread")]
			private static extern bool _ConvertFiberToThread();
			
			public static bool ConvertFiberToThread()
			{
				bool ok = _ConvertFiberToThread();
				if(!ok)
				{
		    		int error = Marshal.GetLastWin32Error();
		    		if(error != 0x501) throw new Win32Exception(error);
				}
				return ok;
			}
			
			[DllImport("kernel32.dll", SetLastError=true)]
			public static extern void DeleteFiber(IntPtr lpFiber);
			
			public delegate void FiberProc(IntPtr lpParameter);
			
			[DllImport("kernel32.dll", SetLastError=true)]
			private static extern IntPtr CreateFiberEx(int dwStackCommitSize, int dwStackReserveSize, int dwFlags, FiberProc lpStartAddress, IntPtr lpParameter);
			
			public static IntPtr CreateFiber(int dwStackSize, FiberProc lpStartAddress, IntPtr lpParameter)
			{
				IntPtr fiber = CreateFiberEx(dwStackSize, dwStackSize, 1, lpStartAddress, lpParameter);
				if(fiber == IntPtr.Zero)
				{
					throw new Win32Exception();
				}
				return fiber;
			}
			
			[DllImport("kernel32.dll", SetLastError=true)]
			private static extern IntPtr ConvertThreadToFiberEx(IntPtr lpParameter, int dwFlags);
			
			public static IntPtr ConvertThreadToFiber(IntPtr lpParameter)
			{
				IntPtr fiber = ConvertThreadToFiberEx(lpParameter, 1);
				if(fiber == IntPtr.Zero)
				{
		    		int error = Marshal.GetLastWin32Error();
		    		if(error != 0x0500) throw new Win32Exception(error);
				}
				return fiber;
			}
			
			public static IntPtr GetCurrentFiber()
			{
				return _GetCurrentFiber.Delegate();
			}
			
			public static IntPtr GetFiberData()
			{
				return Marshal.ReadIntPtr(GetCurrentFiber());
			}
			
			private static readonly NativeFunction<GetCurrentFiberDelegate> _GetCurrentFiber = new NativeFunction<GetCurrentFiberDelegate>(
				m => {
					if(m != ImageFileMachine.I386) throw new NotSupportedException();
					return new byte[]{
						0x64, 0xA1, 0x10, 0x00, 0x00, 0x00, //mov eax, fs:10h
						0xC3 //ret
					};
				}
			);
			private delegate IntPtr GetCurrentFiberDelegate();
		}
		
		class FiberFactory : IFiberFactory
		{
			public FiberBase CurrentFiber{
				get{
					return Fiber.CurrentFiber;
				}
			}
			
			public FiberBase CreateNew(ThreadStart start)
			{
				return new Fiber(start);
			}
			
			public FiberBase CreateNew(ThreadStart start, int maxStackSize)
			{
				return new Fiber(start, maxStackSize);
			}
			
			public FiberBase CreateNew(ParameterizedThreadStart start, object parameter)
			{
				return new Fiber(start, parameter);
			}
			
			public FiberBase CreateNew(ParameterizedThreadStart start, int maxStackSize, object parameter)
			{
				return new Fiber(start, maxStackSize, parameter);
			}
		}
	}
}
