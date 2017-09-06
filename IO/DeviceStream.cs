/* Date: 3.9.2017, Time: 2:33 */
using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.IO
{
	/// <summary>
	/// This class inherits its functionality from <see cref="System.IO.FileStream"/>,
	/// but allows extended and UNC paths.
	/// </summary>
	public class DeviceStream : FileStream
	{
		[DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		static extern IntPtr CreateFile(
			string filename,
			FileAccess access,
			FileShare share,
			IntPtr securityAttributes,
			FileMode creationDisposition,
			FileAttributes flagsAndAttributes,
			IntPtr templateFile
		);
		
		private static SafeFileHandle OpenFile(string filename, FileMode mode, FileAccess access, FileShare share)
		{
			IntPtr handle = CreateFile(filename, access, share, IntPtr.Zero, mode, (FileAttributes)1048576, IntPtr.Zero);
			return new SafeFileHandle(handle, true);
		}
		
		public DeviceStream(string path, FileMode mode) : this(path, mode, (mode == FileMode.Append) ? FileAccess.Write : FileAccess.ReadWrite, FileShare.Read, 4096)
		{
			
		}
		
		public DeviceStream(string path, FileMode mode, FileAccess access) : this(path, mode, access, FileShare.Read, 4096)
		{
			
		}
		
		public DeviceStream(string path, FileMode mode, FileAccess access, FileShare share) : this(path, mode, access, share, 4096)
		{
			
		}
		
		public DeviceStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize) : base(OpenFile(path, mode, access, share), access, bufferSize)
		{
			
		}
	}
}
