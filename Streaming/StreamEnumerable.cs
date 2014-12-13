/* Date: 21.7.2014, Time: 11:37 */
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Streaming
{
	public static class StreamEnumerable
	{
		public static IEnumerable<byte> ToIEnumerable(this Stream input)
		{
			int c;
			while((c = input.ReadByte()) != -1)
			{
				yield return (byte)c;
			}
		}
		
		public static Stream ToStream(this IEnumerable<byte> enumerable)
		{
			return new EnumerableStream(enumerable);
		}
		
		public static void Write(this Stream output, params byte[] bytes)
		{
			Write(output, (IEnumerable<byte>)bytes);
		}
		
		public static void Write(this Stream output, IEnumerable<byte> bytes)
		{
			foreach(var b in bytes)
			{
				output.WriteByte(b);
			}
		}
		
		public static void Write<T>(this Stream output, params T[] structures) where T : struct
		{
			Write<T>(output, (IEnumerable<T>)structures);
		}
		
		public static void Write<T>(this Stream output, IEnumerable<T> structures) where T : struct
		{
			Type t = typeof(T);
			int size = Marshal.SizeOf(t);
			byte[] buffer = new byte[size];
			IntPtr ptr = Marshal.AllocHGlobal(size);
			try{
				foreach(var s in structures)
				{
					Marshal.StructureToPtr(s, ptr, true);
					Marshal.Copy(ptr, buffer, 0, size);
					output.Write(buffer, 0, size);
				}
			}finally{
				Marshal.FreeHGlobal(ptr);
			}
		}
		
		public static void Write(this Stream output, params object[] structures)
		{
			Write(output, (IEnumerable<object>)structures);
		}
		
		public static void Write(this Stream output, IEnumerable<object> structures)
		{
			foreach(var s in structures)
			{
				int size = Marshal.SizeOf(s);
				byte[] buffer = new byte[size];
				IntPtr ptr = Marshal.AllocHGlobal(size);
				try{
					Marshal.StructureToPtr(s, ptr, true);
					Marshal.Copy(ptr, buffer, 0, size);
					output.Write(buffer, 0, size);
				}finally{
					Marshal.FreeHGlobal(ptr);
				}
			}
		}
		
		/*[CLSCompliant(false)]
		public static void Write(this Stream output, __arglist)
		{
			ArgIterator iter = new ArgIterator(__arglist);
			try{
				while(iter.GetRemainingCount() > 0)
				{
					TypedReference tr = iter.GetNextArg();
					Type t = __reftype(tr);
					int size = Marshal.SizeOf(t);
					byte[] buffer = new byte[size];
					IntPtr ptr = Marshal.AllocHGlobal(size);
					try{
						Marshal.StructureToPtr(__refvalue(tr), ptr, true);
						Marshal.Copy(ptr, buffer, 0, size);
						output.Write(buffer, 0, size);
					}finally{
						Marshal.FreeHGlobal(ptr);
					}
				}
			}finally{
				iter.End();
			}
		}*/
		
		/*static class GenericTypedRefMarshalHelper where T : struct
		{
			static readonly Type valht = typeof(ValueHolder<>);
			
			public static IntPtr Marshal(TypedReference tr, out int size)
			{
				Type t = __reftype(tr);
				Type valh = valht.MakeGenericType(t);
				valh.GetField("val", BindingFlags.Public | BindingFlags.Static).SetValueDirect(
			}
			
			static class ValueHolder<T> where T : struct
			{
				public static T val;
				
				public static IntPtr Marshal(out int size)
				{
					
				}
			}
		}*/
		
		public static IEnumerable<T> ToIEnumerable<T>(this Stream input) where T : struct
		{
			if(TypeOf<T>.TypeID == TypeOf<byte>.TypeID) return (IEnumerable<T>)ToIEnumerable(input);
			return EnumerateStructures<T>(input);
		}
		
		private static IEnumerable<T> EnumerateStructures<T>(Stream input) where T : struct
		{
			Type t = TypeOf<T>.TypeID;
			int size = Marshal.SizeOf(t);
			byte[] buffer = new byte[size];
			IntPtr ptr = Marshal.AllocHGlobal(size);
			try{
				while(input.Read(buffer, 0, size) == size)
				{
					Marshal.Copy(buffer, 0, ptr, size);
					yield return Marshal.PtrToStructure<T>(ptr);
				}
				throw new EndOfStreamException();
			}finally{
				Marshal.FreeHGlobal(ptr);
			}
		}
		
		private class EnumerableStream : Stream
		{
			readonly IEnumerator<byte> enumerator;
			bool end;
			
			public EnumerableStream(IEnumerable<byte> enumerable)
			{
				enumerator = enumerable.GetEnumerator();
			}
			
			public override void Close()
			{
				base.Close();
			}
			
			protected override void Dispose(bool disposing)
			{
				if(disposing)
					enumerator.Dispose();
			}
			
			public override bool CanRead{
				get{
					return !end;
				}
			}
			
			public override bool CanSeek{
				get{
					return false;
				}
			}
			
			public override bool CanTimeout{
				get{
					return false;
				}
			}
			
			public override bool CanWrite{
				get{
					return false;
				}
			}
			
			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}
			
			public override int Read(byte[] buffer, int offset, int count)
			{
				for(int i = 0; i < count; i++)
				{
					if(enumerator.MoveNext())
					{
						buffer[offset+i] = enumerator.Current;
					}else{
						end = true;
						return i;
					}
				}
				return count;
			}
			
			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}
			
			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException();
			}
			
			public override void Flush()
			{
				
			}
			
			public override long Position{
				get{
					throw new NotSupportedException();
				}
				set{
					throw new NotSupportedException();
				}
			}
			
			public override long Length{
				get{
					throw new NotSupportedException();
				}
			}
		}
	}
}
