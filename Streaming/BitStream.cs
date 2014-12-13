/* Date: 21.7.2014, Time: 11:53 */
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.Streaming
{
	public class BitStream : IObjectReader<bool>, IObjectWriter<bool>
	{
		public Stream InnerStream{get; private set;}
		public bool LittleEndian{get; private set;}
		
		public BitStream(Stream stream) : this(stream, BitConverter.IsLittleEndian)
		{
			
		}
		
		public BitStream(Stream stream, bool littleEndian)
		{
			InnerStream = stream;
			LittleEndian = littleEndian;
		}
		
		public IEnumerator<bool> GetEnumerator()
		{
			foreach(byte _b in InnerStream.ToIEnumerable())
			{
				byte b = _b;
				for(int i = 0; i < 8; i++)
				{
					if(LittleEndian)
					{
						yield return (b&1)==1;
						b >>= 1;
					}else{
						yield return (b&128)==1;
						b <<= 1;
					}
				}
			}
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		
		public bool Read()
		{
			foreach(bool b in this)
			{
				return b;
			}
			throw new EndOfStreamException();
		}
		
		public void Write(IEnumerable<bool> bits)
		{
			foreach(bool bit in bits)
			{
				Write(bit);
			}
		}
		
		int pos;
		byte buffer;
		public void Write(bool bit)
		{
			if(LittleEndian)
			{
				buffer |= (byte)(bit?1<<pos:0);
			}else{
				buffer |= (byte)(bit?128>>pos:0);
			}
			pos += 1;
			if(pos == 8)
			{
				InnerStream.WriteByte(buffer);
				pos = 0;
				buffer = 0;
			}
		}
	}
}
