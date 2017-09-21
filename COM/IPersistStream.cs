/* Date: 6.9.2017, Time: 16:44 */
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace IllidanS4.SharpUtils.COM
{
	[ComImport]
	[Guid("00000109-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [CLSCompliant(false)]
	public interface IPersistStream
	{
		Guid GetClassID();
		
		[PreserveSig]
		HRESULT IsDirty();
		
		void Load(IStream pStm);
		void Save(IStream pStm, bool fClearDirty);
		uint GetSizeMax();
	}
}
