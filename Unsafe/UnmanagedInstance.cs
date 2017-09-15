using System;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Unsafe
{
	/// <summary>
	/// Class used to create instances of managed objects in the unmanaged memory.
	/// Use with caution!
	/// </summary>
	public static class UnmanagedInstance<T> where T : class
	{
		static readonly IntPtr tptr;
		static readonly int tsize;
		static readonly byte[] init;
		
		/// <summary>
		/// Allocates unmanaged memory for an object.
		/// </summary>
		public static T Allocate()
		{
			IntPtr handle = Marshal.AllocHGlobal(tsize);
			Marshal.Copy(init, 0, handle, tsize);
			IntPtr ptr = handle+4;
			return UnsafeTools.GetObject(ptr) as T;
		}
		
		/// <summary>
		/// Frees the unmanaged memory associated with an object.
		/// </summary>
		public static void Free(T obj)
		{
			IntPtr ptr = UnsafeTools.GetAddress(obj);
			IntPtr handle = ptr-4;
			Marshal.FreeHGlobal(handle);
		}
		
		static UnmanagedInstance()
		{
			var type = TypeOf<T>.TypeID;
			tptr = type.TypeHandle.Value;
			tsize = UnsafeTools.BaseInstanceSizeOf(type);
			init = new byte[tsize];
			BitConverter.GetBytes((long)tptr).CopyTo(init, 4);
		}
	}
}