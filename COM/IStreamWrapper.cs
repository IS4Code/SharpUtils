/* Date: ‎21.12.2012, ‏‎Time: 20:03 */
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.COM
{
	/// <summary>
	/// A type wrapping COM IStream.
	/// </summary>
	public class IStreamWrapper : Stream
	{
		private readonly IStream baseStream;
		
		public IStreamWrapper(IntPtr baseStreamPIUnk)
		{
			this.baseStream = (IStream)Marshal.GetObjectForIUnknown(baseStreamPIUnk);
		}
		
		public IStreamWrapper(System.Runtime.InteropServices.ComTypes.IStream baseStream)
		{
			this.baseStream = (IStream)baseStream;
		}
		
		public override void Write(byte[] buffer, int offset, int count)
		{
			byte[] buf = new byte[count];
			Array.Copy(buffer, offset, buf, 0, count);
			baseStream.Write(buf, count, IntPtr.Zero);
		}
		
		public override int Read(byte[] buffer, int offset, int count)
		{
			byte[] buf = new byte[count];
			int read;
			baseStream.Read(buf, count, out read);
			buf.CopyTo(buffer, offset);
			return read;
		}
		
		public override void SetLength(long value)
		{
			baseStream.SetSize(value);
		}
		
		public override long Seek(long offset, SeekOrigin origin)
		{
			long position;
			baseStream.Seek(offset, (int)origin, out position);
			return position;
		}
		
		public override void Flush()
		{
			baseStream.Commit(0);
		}
		
		public override long Position{
			get{
				return Seek(0, SeekOrigin.Current);
			}
			set{
				Seek(value, SeekOrigin.Begin);
			}
		}
		
		public override long Length{
			get{
				STATSTG stat;
				baseStream.Stat(out stat, 1);
				return stat.cbSize;
			}
		}
		
		public override bool CanWrite{
			get{
				return true;
			}
		}
		
		public override bool CanSeek{
			get{
				return true;
			}
		}
		
		public override bool CanRead{
			get{
				return true;
			}
		}
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	
		[ComImport, Guid("0000000c-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IStream
		{
		    void Read([Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] byte[] pv, int cb, out int pcbRead);
		    void Write([MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] byte[] pv, int cb, IntPtr pcbWritten);
		    void Seek(long dlibMove, int dwOrigin, out long plibNewPosition);
		    void SetSize(long libNewSize);
		    void CopyTo(IStream pstm, long cb, out int pcbRead, out int pcbWritten);
		    void Commit(int grfCommitFlags);
		    void Revert();
		    void LockRegion(long libOffset, long cb, int dwLockType);
		    void UnlockRegion(long libOffset, long cb, int dwLockType);
		    void Stat(out STATSTG pstatstg, int grfStatFlag);
		    void Clone(out IStream ppstm);
		}
		
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
		private struct STATSTG
		{
		    public string pwcsName;
		    public int type;
		    public long cbSize;
		    public FILETIME mtime;
		    public FILETIME ctime;
		    public FILETIME atime;
		    public int grfMode;
		    public int grfLocksSupported;
		    public Guid clsid;
		    public int grfStateBits;
		    public int reserved;
		}
		
		[StructLayout(LayoutKind.Sequential)]
		private struct FILETIME
		{
		    public int dwLowDateTime;
		    public int dwHighDateTime;
		}
	}
}
