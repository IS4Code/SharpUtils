/* Date: ‎21.12.2012, ‏‎Time: 19:47 */
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace IllidanS4.SharpUtils.COM
{
	public class StreamToIStreamMarshaler : ICustomMarshaler
	{
		private static readonly StreamToIStreamMarshaler instance = new StreamToIStreamMarshaler();
		
		public StreamToIStreamMarshaler()
		{
			
		}
		
		public static ICustomMarshaler GetInstance(string pstrCookie)
		{
			return instance;
		}
		
		public object MarshalNativeToManaged(IntPtr pNativeData)
		{
			return new IStreamWrapper(pNativeData);
		}
		
		public IntPtr MarshalManagedToNative(object ManagedObj)
		{
			if(!(ManagedObj is Stream)) throw new ArgumentException("Object must be of type System.Stream.", "ManagedObj");
			return Marshal.GetIUnknownForObject(new StreamWrapper((Stream)ManagedObj));
		}
		
		public void CleanUpNativeData(IntPtr pNativeData)
		{
			
		}
		
		public void CleanUpManagedData(object ManagedObj)
		{
			
		}
		
		public int GetNativeDataSize()
		{
			return -1;
		}
	}
}
