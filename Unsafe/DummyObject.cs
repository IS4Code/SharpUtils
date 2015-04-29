using System;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Unsafe
{
	public sealed class DummyObject<T> : IDisposable
	{
		private IntPtr handle;
		public T Value{get; private set;}
		
		internal DummyObject(IntPtr handle, T value)
		{
			this.handle = handle;
			Value = value;
		}
		
		~DummyObject()
		{
			this.Dispose();
		}
		
		public void Dispose()
		{
			if(handle != IntPtr.Zero)
			{
				Value = default(T);
				Marshal.FreeHGlobal(handle);
				handle = IntPtr.Zero;
			}
			
			GC.SuppressFinalize(this);
		}
	}
}