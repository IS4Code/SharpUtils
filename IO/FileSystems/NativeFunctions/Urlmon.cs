/* Date: 13.9.2017, Time: 18:55 */
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	partial class Win32FileSystem
	{
		static class Urlmon
		{
		    [DllImport("urlmon.dll", CharSet=CharSet.Unicode, ExactSpelling=true, PreserveSig=false)]
		    static extern void FindMimeFromData(IBindCtx pBC, string pwzUrl, IntPtr pBuffer, int cbSize, string pwzMimeProposed, int dwMimeFlags, out string ppwzMimeOut, int dwReserved);
		    
			[DebuggerStepThrough]
		    public static string FindMimeFromData(IBindCtx pBC, string pwzUrl, IntPtr pBuffer, int cbSize, string pwzMimeProposed, int dwMimeFlags)
		    {
		    	string mime;
		    	FindMimeFromData(pBC, pwzUrl, pBuffer, cbSize, pwzMimeProposed, dwMimeFlags, out mime, 0);
		    	return mime;
		    }
		}
	}
}
