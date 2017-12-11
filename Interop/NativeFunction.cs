/* Date: 10.12.2017, Time: 22:12 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Interop
{
	/// <summary>
	/// Used to run platform-specific native code.
	/// </summary>
	public class NativeFunction<TDelegate> : NativeFunction where TDelegate : class
	{
		/// <summary>
		/// Constructs a new instance using a provider function.
		/// </summary>
		/// <param name="codeProvider">
		/// This function provides a platform-specific code based
		/// on the platform type specified by its argument.
		/// It is called lazily when the native function is to be called,
		/// and its result is cached.
		/// </param>
		public NativeFunction(Func<ImageFileMachine, byte[]> codeProvider) : base(codeProvider, typeof(TDelegate))
		{
			
		}
		
		/// <summary>
		/// Obtains the delegate that can be used to run the native function for the
		/// current platform.
		/// </summary>
		public new TDelegate Delegate{
			get{
				return (TDelegate)(object)base.Delegate;
			}
		}
	}
	
	public class NativeFunction : IDisposable
	{
		readonly Dictionary<ImageFileMachine, PlatformNativeFunction> cache = new Dictionary<ImageFileMachine, PlatformNativeFunction>();
		readonly Func<ImageFileMachine, byte[]> codeProvider;
		readonly Type delegateType;
		bool disposed;
		
		/// <summary>
		/// Constructs a new instance using a provider function.
		/// </summary>
		/// <param name="codeProvider">
		/// This function provides a platform-specific code based
		/// on the platform type specified by its argument.
		/// It is called lazily when the native function is to be called,
		/// and its result is cached.
		/// </param>
		/// <param name="delegateType">The type of the delegate that will be created.</param>
		public NativeFunction(Func<ImageFileMachine, byte[]> codeProvider, Type delegateType)
		{
			this.codeProvider = codeProvider;
			this.delegateType = delegateType;
		}
		
		/// <summary>
		/// Obtains the delegate that can be used to run the native function for the
		/// current platform.
		/// </summary>
		public Delegate Delegate{
			get{
				PortableExecutableKinds pe;
				ImageFileMachine im;
				typeof(object).Module.GetPEKind(out pe, out im);
				
				PlatformNativeFunction pfunc;
				if(!cache.TryGetValue(im, out pfunc))
				{
					byte[] code = codeProvider(im);
					if(code == null) throw new NotSupportedException();
					
					cache[im] = pfunc = new ManagedMemoryPlatformNativeFunction(code, delegateType);
				}
				
				return pfunc.Delegate;
			}
		}
		
		/// <summary>
		/// Releases all unmanaged resources held by this instace.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if(disposed) throw new ObjectDisposedException(null);
			disposed = true;
			if(disposing)
			{
				foreach(PlatformNativeFunction func in cache.Values)
				{
					func.Dispose();
				}
				cache.Clear();
			}
		}
		
		~NativeFunction()
		{
			Dispose(false);
		}
	}
	
	/// <summary>
	/// Represents a function that can be run only on certain platforms.
	/// </summary>
	public abstract class PlatformNativeFunction : IDisposable
	{
		bool disposed;
		
		public Type Type{
			get; private set;
		}
		
		public abstract Delegate Delegate{
			get;
		}
		
		public PlatformNativeFunction(Type delegateType)
		{
			Type = delegateType;
		}
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if(disposed) throw new ObjectDisposedException(null);
			disposed = true;
		}
		
		~PlatformNativeFunction()
		{
			Dispose(false);
		}
	}
	
	/// <summary>
	/// Represents a native function that has its code present in a fixed memory location.
	/// </summary>
	public class MemoryPlatformNativeFunction : PlatformNativeFunction
	{
		Delegate funcDelegate;
		
		public override Delegate Delegate{
			get{
				return funcDelegate;
			}
		}
		
		public MemoryPlatformNativeFunction(IntPtr code, Type delegateType) : base(delegateType)
		{
			funcDelegate = Marshal.GetDelegateForFunctionPointer(code, delegateType);
		}
	
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			funcDelegate = null;
		}
	}
	
	/// <summary>
	/// A native function that unprotects its memory for execution.
	/// </summary>
	public class ProtectedMemoryPlatformNativeFunction : MemoryPlatformNativeFunction
	{
		readonly IntPtr code;
		readonly int size;
		readonly int oldProtect;
		
		public ProtectedMemoryPlatformNativeFunction(IntPtr code, int size, Type delegateType) : base(code, delegateType)
		{
			this.code = code;
			this.size = size;
			oldProtect = VirtualProtect(code, (UIntPtr)size, 0x40);
		}
	
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			
			VirtualProtect(code, (UIntPtr)size, oldProtect);
		}
	
		[DllImport("kernel32.dll", SetLastError=true, EntryPoint="VirtualProtect")]
		private static extern bool _VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, int flNewProtect, out int lpflOldProtect);
		
		private static int VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, int flNewProtect)
		{
			int lpflOldProtect;
			bool ok = _VirtualProtect(lpAddress, dwSize, flNewProtect, out lpflOldProtect);
			if(!ok) throw new Win32Exception();
			return lpflOldProtect;
		}
	}
	
	/// <summary>
	/// A native function that stores pin handles to managed memory.
	/// </summary>
	public class PinnedMemoryPlatformNativeFunction : ProtectedMemoryPlatformNativeFunction
	{
		GCHandle handle;
		
		public PinnedMemoryPlatformNativeFunction(GCHandle handle, int size, Type delegateType) : base(handle.AddrOfPinnedObject(), size, delegateType)
		{
			this.handle = handle;
		}
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			handle.Free();
		}
	}
	
	/// <summary>
	/// A native function that has its code located in the managed memory.
	/// </summary>
	public class ManagedMemoryPlatformNativeFunction : PinnedMemoryPlatformNativeFunction
	{
		public ManagedMemoryPlatformNativeFunction(byte[] code, Type delegateType) : base(GCHandle.Alloc(code, GCHandleType.Pinned), code.Length, delegateType)
		{
			
		}
		
		public ManagedMemoryPlatformNativeFunction(ValueType codeStruct, Type delegateType) : base(GCHandle.Alloc(codeStruct, GCHandleType.Pinned), Marshal.SizeOf(codeStruct), delegateType)
		{
			
		}
	}
}
