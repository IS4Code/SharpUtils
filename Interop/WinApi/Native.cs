/* Date: 21.9.2017, Time: 16:50 */
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Interop.WinApi
{
	/// <summary>
	/// Provides tools for WinApi interop.
	/// </summary>
	public static class Native
	{
		/// <summary>
		/// Generates a new exception object from the last Win32 error.
		/// </summary>
		public static Exception GetExceptionForLastWin32Error()
		{
			return GetExceptionForWin32Error(Marshal.GetLastWin32Error());
		}
		
		/// <summary>
		/// Generates a new exception object from a Win32 error code.
		/// </summary>
		public static Exception GetExceptionForWin32Error(int error)
		{
			int hresult = GetHRForWin32Error(error);
			var comException = Marshal.GetExceptionForHR(hresult);
			if(comException is COMException)
			{
				return new Win32Exception(error);
			}else{
				return comException;
			}
		}
		
		/// <summary>
		/// Throws an exception for a Win32 error code.
		/// </summary>
		[DebuggerStepThrough]
		public static void ThrowExceptionForWin32Error(int error)
		{
			var exception = GetExceptionForWin32Error(error);
			if(exception != null) throw exception;
		}
		
		/// <summary>
		/// Throws an exception for the last Win32 error.
		/// </summary>
		[DebuggerStepThrough]
		public static void ThrowExceptionForLastWin32Error()
		{
			var exception = GetExceptionForLastWin32Error();
			if(exception != null) throw exception;
		}
		
		/// <summary>
		/// Converts a Win32 error code to a HRESULT.
		/// </summary>
		public static int GetHRForWin32Error(int error)
		{
			unchecked{
				if (((long)error & (long)((ulong)-2147483648)) == (long)((ulong)-2147483648))
				{
					return error;
				}
				return (error & 65535) | -2147024896;
			}
		}
	}
}
