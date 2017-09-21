/* Date: 6.9.2017, Time: 16:52 */
using System;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Com
{
	[ComImport]
	[Guid("0000010C-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IPersist
	{
		Guid GetClassID();
	}
}
