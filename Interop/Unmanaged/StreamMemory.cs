/* Date: 9.2.2017, Time: 2:01 */
using System;
using System.IO;

namespace IllidanS4.SharpUtils.Interop.Unmanaged
{
	public sealed class StreamMemory : MemoryContext
	{
		readonly Stream stream;
		
		public override bool CanLock{get{return false;}}
		
		public StreamMemory(Stream baseStream)
		{
			if(!baseStream.CanSeek) throw new ArgumentException("The base stream must support seeking.", "baseStream");
			stream = baseStream;
		}
		
		public override byte[] ReadMemory(long address, int size)
		{
			stream.Position = address;
			byte[] buffer = new byte[size];
			size = stream.Read(buffer, 0, size);
			Array.Resize(ref buffer, size);
			return buffer;
		}
		
		public override int WriteMemory(long address, byte[] data)
		{
			stream.Position = address;
			stream.Write(data, 0, data.Length);
			return data.Length;
		}
		
		public override int PointerSize{
			get{
				return sizeof(long);
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
		
		public override void Free(long address)
		{
			throw new NotImplementedException();
		}
		
		public override long Alloc(int size)
		{
			throw new NotImplementedException();
		}
	}
}
