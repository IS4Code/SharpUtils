/* Date: 8.1.2017, Time: 21:51 */
using System;

namespace IllidanS4.SharpUtils.Interop.Unmanaged
{
	public abstract class MemoryContext : MarshalByRefObject
	{
		public abstract byte[] ReadMemory(long address, int size);
		public abstract int WriteMemory(long address, byte[] data);
		public abstract void Unlock(long address, int size);
		public abstract void Lock(long address, int size);
		public abstract int PointerSize{get;}
	}
}
