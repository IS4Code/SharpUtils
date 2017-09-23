/* Date: 13.9.2017, Time: 14:16 */
using System;
using System.Collections.Generic;
using System.IO;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	partial class Win32FileSystem
	{
		class Win32FileHandle : ResourceHandle
		{
			readonly Uri uri;
			IntPtr handle;
			Win32FileSystem fs;
			
			public Win32FileHandle(Uri uri, IntPtr handle, Win32FileSystem fs) : base(fs)
			{
				this.uri = uri;
				IntPtr proc = Kernel32.GetCurrentProcess();
				
				this.handle = Kernel32.DuplicateHandle(proc, handle, proc, 0, true, 2);
				this.fs = fs;
			}
			
			private IntPtr CloneHandle()
			{
				IntPtr proc = Kernel32.GetCurrentProcess();
				return Kernel32.DuplicateHandle(proc, handle, proc, 0, true, 2);
			}
			
			public override Uri Uri{
				get{
					return uri;
				}
			}
			
			protected override FileAttributes Attributes{
				get{
					var info = Kernel32.GetFileInformationByHandle(handle);
					return (FileAttributes)info.dwFileAttributes;
				}
			}
			
			protected override ResourceInfo Target{
				get{
					string path = Kernel32.GetFinalPathNameByHandle(handle, 0);
					return new ResourceInfo(fs.FileUriFromPath(path));
				}
			}
			
			public override ResourceInfo Parent{
				get{
					throw new NotImplementedException();
				}
			}
			
			protected override DateTime CreationTimeUtc{
				get{
					var info = Kernel32.GetFileInformationByHandle(handle);
					return Win32FileSystem.GetDateTime(info.ftCreationTime);
				}
			}
			
			protected override DateTime LastAccessTimeUtc{
				get{
					var info = Kernel32.GetFileInformationByHandle(handle);
					return Win32FileSystem.GetDateTime(info.ftLastAccessTime);
				}
			}
			
			protected override DateTime LastWriteTimeUtc{
				get{
					var info = Kernel32.GetFileInformationByHandle(handle);
					return Win32FileSystem.GetDateTime(info.ftLastWriteTime);
				}
			}
			
			protected override long Length{
				get{
					var info = Kernel32.GetFileInformationByHandle(handle);
					unchecked{
						return ((long)((ulong)(uint)info.nFileSizeHigh << 32) | (long)(uint)info.nFileSizeLow);
					}
				}
			}
			
			public override Stream GetStream(FileMode mode, FileAccess access)
			{
				return new DeviceStream(CloneHandle(), access);
			}
			
			public override List<ResourceInfo> GetResources()
			{
				throw new NotImplementedException();
			}
			
			protected override void Dispose(bool disposing)
			{
				if(handle != IntPtr.Zero)
				{
					Kernel32.CloseHandle(handle);
					handle = IntPtr.Zero;
				}
			}
			
			protected override string ContentType{
				get{
					throw new NotImplementedException();
				}
			}
			
			protected override string LocalPath{
				get{
					return fs.GetProperty<string>(uri, ResourceProperty.LocalPath);
				}
			}
			
			protected override string DisplayPath{
				get{
					return fs.GetProperty<string>(uri, ResourceProperty.DisplayPath);
				}
			}
			
			public override int GetHashCode()
			{
				return handle.GetHashCode();
			}
			
			public override bool Equals(ResourceHandle other)
			{
				var handle = (Win32FileHandle)other;
				if(handle != null) return Kernel32.CompareObjectHandles(this.handle, handle.handle);
				return false;
			}
		}
	}
}
