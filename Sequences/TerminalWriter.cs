/* Date: 29.11.2014, Time: 10:53 */
using System;
using System.IO;
using System.Text;

namespace IllidanS4.SharpUtils.Sequences
{
	public class TerminalWriter : StreamWriter
	{
		public TerminalWriter(Stream stream) : this(new TerminalStream(stream))
		{
			if(!stream.CanSeek)
			{
				throw new ArgumentException("Stream must support seeking.", "stream");
			}
		}
		
		private TerminalWriter(TerminalStream terminal) : base(terminal)
		{
			terminal.terminal = this;
		}
		
		private class TerminalStream : Stream
		{
			public TerminalWriter terminal;
			private readonly Stream inner;
			private readonly StringBuilder line;
			private long linestart;
			private long lineend;
			
			public TerminalStream(Stream inner)
			{
				this.inner = inner;
				linestart = inner.Position;
				lineend = linestart;
				line = new StringBuilder();
			}
			
			public override void Write(byte[] buffer, int offset, int count)
			{
				var enc = terminal.Encoding;
				char[] chars = enc.GetChars(buffer, offset, count);
				for(int i = 0; i < chars.Length; i++)
				{
					char ch = chars[i];
					switch(ch)
					{
						case '\r':
							Write(line.ToString());
							lineend = inner.Position;
							inner.Seek(linestart, SeekOrigin.Begin);
							line.Clear();
							break;
						case '\n':
							Write(line.ToString());
							if(lineend > inner.Position)
								inner.Seek(lineend, SeekOrigin.Begin);
							Write(Environment.NewLine);
							linestart = lineend = inner.Position;
							line.Clear();
							break;
						default:
							line.Append(ch);
							break;
					}
				}
				Write(line.ToString());
			}
			
			private void Write(string text)
			{
				byte[] bytes = terminal.Encoding.GetBytes(text);
				inner.Write(bytes, 0, bytes.Length);
			}
			
			public override void SetLength(long value)
			{
				inner.SetLength(value);
			}
			
			public override long Seek(long offset, SeekOrigin origin)
			{
				return inner.Seek(offset, origin);
			}
			
			public override int Read(byte[] buffer, int offset, int count)
			{
				throw new NotImplementedException();
			}
			
			public override long Position{
				get{
					return inner.Position;
				}
				set{
					inner.Position = value;
				}
			}
			
			public override long Length{
				get{
					return inner.Length;
				}
			}
			
			public override void Flush()
			{
				inner.Flush();
			}
			
			public override System.Threading.Tasks.Task FlushAsync(System.Threading.CancellationToken cancellationToken)
			{
				return inner.FlushAsync(cancellationToken);
			}
			
			public override bool CanWrite{
				get{
					return inner.CanWrite;
				}
			}
			
			public override bool CanSeek{
				get{
					return inner.CanSeek;
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
