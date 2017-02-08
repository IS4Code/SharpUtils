/* Date: 8.1.2017, Time: 22:12 */
using System;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Interop.Unmanaged
{
	/// <summary>
	/// A class akin to <see cref="System.Runtime.InteropServices.Marshal"/> that does the reading and writing using <see cref="FarPtr"/>.
	/// </summary>
	public static class FarMarshal
	{
		/// <summary>
		/// Gets the pointer size of the memory context where a far pointer is located.
		/// </summary>
		/// <param name="pointer">The far pointer specifying the memory context.</param>
		/// <returns>The pointer size in bytes.</returns>
		public static int GetTargetContextPointerSize(FarPtr pointer)
		{
			return pointer.memory.PointerSize;
		}
		
		public static byte ReadByte(FarPtr ptr)
		{
			return ReadByte(ptr, 0);
		}
		
		public static byte ReadByte(FarPtr ptr, int ofs)
		{
			return ptr.memory.ReadMemory(ptr.value+ofs, 1)[0];
		}
		
		public static short ReadInt16(FarPtr ptr)
		{
			return ReadInt16(ptr, 0);
		}
		
		public static short ReadInt16(FarPtr ptr, int ofs)
		{
			return BitConverter.ToInt16(ptr.memory.ReadMemory(ptr.value+ofs, 2), 0);
		}
		
		public static int ReadInt32(FarPtr ptr)
		{
			return ReadInt32(ptr, 0);
		}
		
		public static int ReadInt32(FarPtr ptr, int ofs)
		{
			return BitConverter.ToInt32(ptr.memory.ReadMemory(ptr.value+ofs, 4), 0);
		}
		
		public static long ReadInt64(FarPtr ptr)
		{
			return ReadInt64(ptr, 0);
		}
		
		public static long ReadInt64(FarPtr ptr, int ofs)
		{
			return BitConverter.ToInt64(ptr.memory.ReadMemory(ptr.value+ofs, 8), 0);
		}
		
		public static float ReadSingle(FarPtr ptr)
		{
			return ReadSingle(ptr, 0);
		}
		
		public static float ReadSingle(FarPtr ptr, int ofs)
		{
			return BitConverter.ToSingle(ptr.memory.ReadMemory(ptr.value+ofs, 4), 0);
		}
		
		public static double ReadDouble(FarPtr ptr)
		{
			return ReadDouble(ptr, 0);
		}
		
		public static double ReadDouble(FarPtr ptr, int ofs)
		{
			return BitConverter.ToDouble(ptr.memory.ReadMemory(ptr.value+ofs, 8), 0);
		}
		
		public static FarPtr ReadFarPtr(FarPtr ptr)
		{
			return ReadFarPtr(ptr, 0);
		}
		
		public static FarPtr ReadFarPtr(FarPtr ptr, int ofs)
		{
			byte[] data = ptr.memory.ReadMemory(ptr.value, ptr.memory.PointerSize);
			if(data.Length > 8) throw new OverflowException("Target memory pointer size is not representable.");
			Array.Resize(ref data, 8);
			return new FarPtr(ptr.memory, BitConverter.ToInt64(data, 0));
		}
		
		public static byte[] Read(FarPtr ptr, int size)
		{
			return Read(ptr, 0, size);
		}
		
		public static byte[] Read(FarPtr ptr, int ofs, int size)
		{
			return ptr.memory.ReadMemory(ptr.value+ofs, size);
		}
		
		public static void WriteByte(FarPtr ptr, byte val)
		{
			WriteByte(ptr, 0, val);
		}
		
		public static void WriteByte(FarPtr ptr, int ofs, byte val)
		{
			ptr.memory.WriteMemory(ptr.value+ofs, new[]{val});
		}
		
		public static void WriteInt16(FarPtr ptr, short val)
		{
			WriteInt16(ptr, 0, val);
		}
		
		public static void WriteInt16(FarPtr ptr, int ofs, short val)
		{
			ptr.memory.WriteMemory(ptr.value+ofs, BitConverter.GetBytes(val));
		}
		
		public static void WriteInt32(FarPtr ptr, int val)
		{
			WriteInt32(ptr, 0, val);
		}
		
		public static void WriteInt32(FarPtr ptr, int ofs, int val)
		{
			ptr.memory.WriteMemory(ptr.value+ofs, BitConverter.GetBytes(val));
		}
		
		public static void WriteInt64(FarPtr ptr, long val)
		{
			WriteInt64(ptr, 0, val);
		}
		
		public static void WriteInt64(FarPtr ptr, int ofs, long val)
		{
			ptr.memory.WriteMemory(ptr.value+ofs, BitConverter.GetBytes(val));
		}
		
		public static void WriteSingle(FarPtr ptr, float val)
		{
			WriteSingle(ptr, 0, val);
		}
		
		public static void WriteSingle(FarPtr ptr, int ofs, float val)
		{
			ptr.memory.WriteMemory(ptr.value+ofs, BitConverter.GetBytes(val));
		}
		
		public static void WriteDouble(FarPtr ptr, double val)
		{
			WriteDouble(ptr, 0, val);
		}
		
		public static void WriteDouble(FarPtr ptr, int ofs, double val)
		{
			ptr.memory.WriteMemory(ptr.value+ofs, BitConverter.GetBytes(val));
		}
		
		public static void WriteFarPtr(FarPtr ptr, FarPtr val)
		{
			WriteFarPtr(ptr, 0, val);
		}
		
		public static void Write(FarPtr ptr, byte[] val)
		{
			Write(ptr, 0, val);
		}
		
		public static void Write(FarPtr ptr, int ofs, byte[] val)
		{
			ptr.memory.WriteMemory(ptr.value+ofs, val);
		}
		
		public static void WriteFarPtr(FarPtr ptr, int ofs, FarPtr val)
		{
			ptr.CheckSameMemory(ref val);
			byte[] data = BitConverter.GetBytes(val.value);
			int ptrsize = ptr.memory.PointerSize;
			if(!BitConverter.IsLittleEndian)
			{
				Array.Reverse(data);
				Array.Resize(ref data, ptrsize);
				Array.Reverse(data);
			}else{
				Array.Resize(ref data, ptrsize);
			}
			ptr.memory.WriteMemory(ptr.value+ofs, data);
		}
		
		public static void Unlock(FarPtr ptr, int size)
		{
			ptr.memory.Unlock(ptr.value, size);
		}
		
		public static void Lock(FarPtr ptr, int size)
		{
			ptr.memory.Lock(ptr.value, size);
		}
	}
}
