/* Date: 23.3.2016, Time: 22:34 */
using System;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Interop;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils.Serialization
{
	public static class Copier
	{
		public static void CopyShallow(object source, object target)
		{
			if(source == null) throw new ArgumentNullException("source");
			if(target == null) throw new ArgumentNullException("target");
			Type t = source.GetType();
			if(target.GetType() != t) throw new ArgumentException("Object types must be equal.");
			
			byte[] data = new byte[UnsafeTools.BaseInstanceSizeOf(t) - IntPtr.Size];
			InteropTools.Pin(
				source, o1 => {
					var ptr = UnsafeTools.GetAddress(o1);
					Marshal.Copy(ptr, data, 0, data.Length);
				}
			);
			
			InteropTools.Pin(
				target, o2 => {
					var ptr = UnsafeTools.GetAddress(o2);
					Marshal.Copy(data, 0, ptr, data.Length);
				}
			);
		}
	}
}
