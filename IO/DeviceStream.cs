/* Date: 3.9.2017, Time: 2:33 */
using System.ComponentModel;
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
		private const int DefaultBufferSize = 4096;
		
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
			bool append = (mode == FileMode.Append);
			if(append)
			{
				mode = FileMode.OpenOrCreate;
			}
			IntPtr handle = CreateFile(filename, access, share, IntPtr.Zero, mode, (FileAttributes)1048576, IntPtr.Zero);
			var sfh = new SafeFileHandle(handle, true);
			if(sfh.IsInvalid) throw new Win32Exception();
			return sfh;
		}
		
		public DeviceStream(string path, FileMode mode) : this(path, mode, (mode == FileMode.Append) ? FileAccess.Write : FileAccess.ReadWrite, FileShare.Read, DefaultBufferSize)
		{
			
		}
		
		public DeviceStream(string path, FileMode mode, FileAccess access) : this(path, mode, access, FileShare.Read, DefaultBufferSize)
		{
			
		}
		
		public DeviceStream(string path, FileMode mode, FileAccess access, FileShare share) : this(path, mode, access, share, DefaultBufferSize)
		{
			
		}
		
		public DeviceStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize) : base(OpenFile(path, mode, access, share), access, bufferSize)
		{
			if(mode == FileMode.Append)
			{
				this.Position = this.Length;
			}
		}
		
		public DeviceStream(IntPtr handle, FileAccess access) : this(handle, access, DefaultBufferSize)
		{
			
		}
		
		public DeviceStream(IntPtr handle, FileAccess access, int bufferSize) : base(new SafeFileHandle(handle, true), access, bufferSize)
		{
			
		}
	}
}
