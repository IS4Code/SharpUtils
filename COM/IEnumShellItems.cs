/* Date: 10.9.2017, Time: 17:33 */
using System;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.COM
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("70629033-E363-4A28-A567-0DB78006E6D7")]
    [CLSCompliant(false)]
	public interface IEnumShellItems
	{
        void Next(int celt, out IShellItem rgelt, out int pceltFetched);
        void Skip(int celt);
        void Reset();
        IEnumShellItems Clone();
	}
}