/* Date: 29.11.2014, Time: 10:36 */
using System;
using System.IO;
using System.Text;

namespace IllidanS4.SharpUtils.Streaming
{
	public abstract class TextToStreamWrapper : Stream
	{
		private readonly IDisposable text;
		protected readonly Encoding encoding;
		
		internal TextToStreamWrapper(IDisposable text, Encoding encoding)
		{
			this.text = text;
			this.encoding = encoding;
		}
		
		public static TextToStreamWrapper Create(TextWriter writer)
		{
			return new WriteWrapper(writer);
		}
		
		public static TextToStreamWrapper Create(TextReader reader, Encoding encoding)
		{
			return new ReadWrapper(reader, encoding);
		}
		
		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}
		
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}
		
		public override long Position{
			get{
				throw new NotImplementedException();
			}
			set{
				throw new NotImplementedException();
			}
		}
		
		public override long Length{
			get{
				throw new NotImplementedException();
			}
		}
		
		public sealed override bool CanSeek{
			get{
				return false;
			}
		}
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			
			if(disposing)
			{
				text.Dispose();
			}
		}
		
		private class ReadWrapper : TextToStreamWrapper
		{
			private readonly TextReader reader;
			
			public ReadWrapper(TextReader reader, Encoding encoding) : base(reader, encoding)
			{
				this.reader = reader;
			}
			
			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotImplementedException();
			}
			
			public override int ReadByte()
			{
				return reader.Read();
			}
			
			public override int Read(byte[] buffer, int offset, int count)
			{
				char[] cbuffer = new char[count];
				int c = reader.Read(cbuffer, 0, count);
				return encoding.GetBytes(cbuffer, 0, c, buffer, offset);
			}
			
			public override async System.Threading.Tasks.Task<int> ReadAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
			{
				char[] cbuffer = new char[count];
				int c = await reader.ReadAsync(cbuffer, 0, count);
				return encoding.GetBytes(cbuffer, 0, c, buffer, offset);
			}
			
			public override void Flush()
			{
				
			}
			
			public override bool CanWrite{
				get{
					return false;
				}
			}
			
			public override bool CanRead{
				get{
					return true;
				}
			}
		}
		
		private class WriteWrapper : TextToStreamWrapper
		{
			private readonly TextWriter writer;
			
			public WriteWrapper(TextWriter writer) : base(writer, writer.Encoding)
			{
				this.writer = writer;
			}
			
			public override void Write(byte[] buffer, int offset, int count)
			{
				writer.Write(encoding.GetString(buffer, offset, count));
			}
			
			public override System.Threading.Tasks.Task WriteAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
			{
				return writer.WriteAsync(encoding.GetString(buffer, offset, count));
			}
			
			public override int Read(byte[] buffer, int offset, int count)
			{
				throw new NotImplementedException();
			}
			
			public override void Flush()
			{
				writer.Flush();
			}
			
			public override System.Threading.Tasks.Task FlushAsync(System.Threading.CancellationToken cancellationToken)
			{
				return writer.FlushAsync();
			}
			
			public override bool CanWrite{
				get{
					return true;
				}
			}
			
			public override bool CanRead{
				get{
					return false;
				}
			}
		}
	}
}
