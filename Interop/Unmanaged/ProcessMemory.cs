/* Date: 8.1.2017, Time: 21:51 */
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Interop.Unmanaged
{
	public sealed class ProcessMemory : MemoryContext
	{
		readonly IntPtr processHandle;
		
		public ProcessMemory(Process process)
		{
			processHandle = process.Handle;
		}
		
		private static class NtDll
		{
			[DllImport("ntdll.dll")]
			public static extern int RtlNtStatusToDosError(int Status);
			
			[DllImport("ntdll.dll", SetLastError=true)]
			public static extern int NtWow64ReadVirtualMemory64(IntPtr hProcess, long lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out UIntPtr lpNumberOfBytesRead);
			
			[DllImport("ntdll.dll", SetLastError=true)]
			public static extern int NtWow64WriteVirtualMemory64(IntPtr hProcess, long lpBaseAddress, byte[] lpBuffer, int nSize, out UIntPtr lpNumberOfBytesWritten);
		}
		
		private static class Kernel32
		{
			[DllImport("kernel32.dll", SetLastError=true)]
			public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out UIntPtr lpNumberOfBytesRead);
			
			[DllImport("kernel32.dll", SetLastError=true)]
			public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out UIntPtr lpNumberOfBytesWritten);
			
			[DllImport("kernel32.dll", SetLastError=true)]
			public static extern bool IsWow64Process(IntPtr processHandle, out bool wow64Process);
			
			[DllImport("kernel32.dll")]
			public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
		}
		
		private static bool IsWow64()
		{
			return Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess;
		}
		
		private static bool IsWow64Process(IntPtr processHandle)
		{
			bool ret;
			if(!Kernel32.IsWow64Process(processHandle, out ret)) throw new Win32Exception();
			return ret;
		}
		
		public override byte[] ReadMemory(long address, int size)
		{
			byte[] buffer = new byte[size];
			UIntPtr numBytes;
			int error = 0;
			if(IsWow64() && !IsWow64Process(processHandle))
			{
				int status = NtDll.NtWow64ReadVirtualMemory64(processHandle, (long)address, buffer, size, out numBytes);
				error = NtDll.RtlNtStatusToDosError(status);
			}else{
				bool success = Kernel32.ReadProcessMemory(processHandle, checked((IntPtr)address), buffer, size, out numBytes);
				if(!success)
				{
					error = Marshal.GetLastWin32Error();
				}
			}
			if(error != 0 && error != 299 || (error == 299 && size > 0 && numBytes == UIntPtr.Zero)) throw new Win32Exception(error);
			if(numBytes != (UIntPtr)size) Array.Resize(ref buffer, (int)numBytes);
			return buffer;
		}
		
		public override int WriteMemory(long address, byte[] data)
		{
			UIntPtr numBytes;
			int error = 0;
			if(IsWow64() && !IsWow64Process(processHandle))
			{
				int status = NtDll.NtWow64WriteVirtualMemory64(processHandle, address, data, data.Length, out numBytes);
				error = NtDll.RtlNtStatusToDosError(status);
			}else{
				bool success = Kernel32.WriteProcessMemory(processHandle, checked((IntPtr)address), data, data.Length, out numBytes);
				if(!success)
				{
					error = Marshal.GetLastWin32Error();
				}
			}
			if(error != 0 && error != 299 || (error == 299 && data.Length > 0 && numBytes == UIntPtr.Zero)) throw new Win32Exception(error);
			return (int)numBytes;
		}
		
		public override int PointerSize{
			get{
				if(Environment.Is64BitOperatingSystem)
				{
					bool isWow64;
					if(!Kernel32.IsWow64Process(processHandle, out isWow64))
						throw new Win32Exception();
					return isWow64 ? 4 : 8;
				}else{
					return 4;
				}
			}
		}
		
		public override void Unlock(long address, int size)
		{
			uint tmp;
			bool res = Kernel32.VirtualProtectEx(processHandle, (IntPtr)address, (UIntPtr)size, 0x40, out tmp);
			if(!res) throw new Win32Exception();
		}
		
		public override void Lock(long address, int size)
		{
			uint tmp;
			bool res = Kernel32.VirtualProtectEx(processHandle, (IntPtr)address, (UIntPtr)size, 0x20, out tmp);
			if(!res) throw new Win32Exception();
		}
	}
}
