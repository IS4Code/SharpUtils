/* Date: 8.1.2017, Time: 21:51 */
using System;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Interop.Unmanaged
{
	public sealed class LocalMemory : MemoryContext
	{
		public static readonly LocalMemory Instance = new LocalMemory();
		
		private LocalMemory()
		{
			
		}
		
		private static class Kernel32
		{
			[DllImport("kernel32.dll", SetLastError=true)]
			public static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);
		}
		
		public override int WriteMemory(long address, byte[] data)
		{
			Marshal.Copy(data, 0, (IntPtr)address, data.Length);
			return data.Length;
		}
		
		public override byte[] ReadMemory(long address, int size)
		{
			var data = new byte[size];
			Marshal.Copy((IntPtr)address, data, 0, size);
			return data;
		}
		
		public override int PointerSize{
			get{
				return IntPtr.Size;
			}
		}
		
		public override void Unlock(long address, int size)
		{
			throw new NotImplementedException();
		}
		
		public override void Lock(long address, int size)
		{
			throw new NotImplementedException();
		}
	}
}
