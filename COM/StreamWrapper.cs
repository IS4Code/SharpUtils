/* Date: ‎21.12.2012, ‏‎Time: 20:28 */
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace IllidanS4.SharpUtils.Com
{
	public class StreamWrapper : IStream
	{
		private readonly Stream baseStream;
		
		public StreamWrapper(Stream baseStream)
		{
			this.baseStream = baseStream;
		}
		
	    public void Read([Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] byte[] pv, int cb, IntPtr pcbRead)
	    {
	    	int read = baseStream.Read(pv, 0, cb);
	    	if(pcbRead != IntPtr.Zero) Marshal.WriteInt32(pcbRead, read);
	    }
	    
	    public void Write([MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] byte[] pv, int cb, IntPtr pcbWritten)
	    {
	    	baseStream.Write(pv, 0, cb);
	    	if(pcbWritten != IntPtr.Zero) Marshal.WriteInt32(pcbWritten, cb);
	    }
	    
	    public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
	    {
	    	long pos = baseStream.Seek(dlibMove, (SeekOrigin)dwOrigin);
	    	if(plibNewPosition != IntPtr.Zero) Marshal.WriteInt64(plibNewPosition, pos);
	    }
	    
	    public void SetSize(long libNewSize)
	    {
	    	baseStream.SetLength(libNewSize);
	    }
	    
	    public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
	    {
	    	IStreamWrapper wrapper = new IStreamWrapper(pstm);
	    	baseStream.CopyTo(wrapper, (int)cb);
	    }
	    
	    public void Commit(int grfCommitFlags)
	    {
	    	baseStream.Flush();
	    }
	    
	    public void Revert()
	    {
	    	throw new NotImplementedException();
	    }
	    
	    public void LockRegion(long libOffset, long cb, int dwLockType)
	    {
	    	throw new NotImplementedException();
	    }
	    
	    public void UnlockRegion(long libOffset, long cb, int dwLockType)
	    {
	    	throw new NotImplementedException();
	    }
	    
	    public void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
	    {
	    	throw new NotImplementedException();
	    }
	    
	    public void Clone(out IStream ppstm)
	    {
	    	throw new NotImplementedException();
	    }
	}
}
