/* Date: 13.9.2017, Time: 18:55 */
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using IllidanS4.SharpUtils.COM;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	partial class Win32FileSystem
	{
		static class Shlwapi
		{
		    [DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
			static extern int UrlCreateFromPath(string pszPath, StringBuilder pszUrl, ref int pcchUrl, int dwFlags);
			
		    [DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
			static extern int PathCreateFromUrl(string pszUrl, StringBuilder pszPath, ref int pcchPath, int dwFlags);
			
			[DebuggerStepThrough]
			public static string UrlCreateFromPath(string pszPath)
			{
				int len = 1;
				var buffer = new StringBuilder(len);
				int result = UrlCreateFromPath(pszPath, buffer, ref len, 0);
				if(len == 1) Marshal.ThrowExceptionForHR(result);
				
				buffer.EnsureCapacity(len);
				result = UrlCreateFromPath(pszPath, buffer, ref len, 0);
				if(result == 1) return null;
				Marshal.ThrowExceptionForHR(result);
				return buffer.ToString();
			}
			
			[DebuggerStepThrough]
			public static string PathCreateFromUrl(string pszUrl)
			{
				int len = 1;
				var buffer = new StringBuilder(len);
				int result = PathCreateFromUrl(pszUrl, buffer, ref len, 0);
				if(len == 1) Marshal.ThrowExceptionForHR(result);
				else if(len == 0) return String.Empty;
				
				buffer.EnsureCapacity(len);
				result = PathCreateFromUrl(pszUrl, buffer, ref len, 0);
				Marshal.ThrowExceptionForHR(result);
				return buffer.ToString();
			}
		}
	}
	
	partial class ShellFileSystem
	{
		static class Shlwapi
		{
			[DllImport("shlwapi.dll", CharSet=CharSet.Auto, PreserveSig=false)]
			public static extern string StrRetToStr(ref STRRET pstr, [Optional]IntPtr pidl);
			
		}
	}
}
