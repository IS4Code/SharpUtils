/* Date: 1.5.2015, Time: 11:55 */
using System;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils.Proxies.Dynamic
{
	public class UnsafeHandle : ObjectTypeHandle, IDisposable
	{
		private readonly GCHandle gcHandle;
		
		public UnsafeHandle(object obj) : base(obj)
		{
			try{
				gcHandle = GCHandle.Alloc(obj, GCHandleType.Pinned);
			}catch(ArgumentException)
			{
				gcHandle = GCHandle.Alloc(obj);
			}
		}
		
		public IntPtr Address{
			get{
				return GCHandle.ToIntPtr(gcHandle); //using GCHandle is "safer" than raw pointers
			}
		}
		
		public IntPtr GetDataAddress()
		{
			return gcHandle.AddrOfPinnedObject();
		}
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		
		delegate void RefGCHandleSetHandle(ref GCHandle gch, IntPtr handle);
		static readonly RefGCHandleSetHandle SetHandle = Hacks.GetFieldSetter<RefGCHandleSetHandle>(typeof(GCHandle), "m_handle");
		
		public static object GetObject(UnsafeHandle handle)
		{
			GCHandle gc = default(GCHandle);
			SetHandle(ref gc, handle.Address);
			return gc.Target;
		}
		
		protected void Dispose(bool disposing)
		{
			gcHandle.Free();
		}
		
		~UnsafeHandle()
		{
			Dispose(false);
		}
	}
}
