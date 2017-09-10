/* Date: 4.9.2017, Time: 20:09 */
using System;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.COM
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214F2-0000-0000-C000-000000000046")]
    [CLSCompliant(false)]
    public interface IEnumIDList
    {
        void Next(int celt, out IntPtr rgelt, out int pceltFetched);
        void Skip(int celt);
        void Reset();
        IEnumIDList Clone();
    }
}
