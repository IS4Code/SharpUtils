using System;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Unsafe
{
	/// <summary>
	/// Represents a handle to a managed object in the unmanaged memory.
	/// </summary>
	public class ObjectHandle<T> : IDisposable where T : class
	{
		bool freed;
		readonly IntPtr handle;
		readonly T value;
		readonly IntPtr tptr;
		
		public ObjectHandle() : this(TypeOf<T>.TypeID)
		{
			
		}
		
		public ObjectHandle(Type t)
		{
			tptr = t.TypeHandle.Value;
			int size = UnsafeTools.BaseInstanceSizeOf(t);
			handle = Marshal.AllocHGlobal(size);
			byte[] zero = new byte[size];
			Marshal.Copy(zero, 0, handle, size);
			IntPtr ptr = handle+4;
			Marshal.WriteIntPtr(ptr, tptr);//write type ptr
			value = (T)UnsafeTools.GetObject(ptr);
		}
		
		public T Value{
			get{
				return value;
			}
		}
		
		public bool Valid{
			get{
				return Marshal.ReadIntPtr(handle, 4) == tptr && !freed;
			}
		}
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if(!freed)
			{
				Marshal.FreeHGlobal(handle);
				freed = true;
			}
		}
		
		~ObjectHandle()
		{
			Dispose(false);
		}
	}
}