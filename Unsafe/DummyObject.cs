using System;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Unsafe
{
	public sealed class DummyObject<T> : IDisposable
	{
		private readonly IntPtr handle;
		public readonly T Value;
		
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
			Marshal.FreeHGlobal(handle);
			
			GC.SuppressFinalize(this);
		}
	}
}