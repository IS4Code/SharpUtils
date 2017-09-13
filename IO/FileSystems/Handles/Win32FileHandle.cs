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
			
			public override Uri Uri{
				get{
					return uri;
				}
			}
			
			public override FileAttributes Attributes{
				get{
					var info = Kernel32.GetFileInformationByHandle(handle);
					return (FileAttributes)info.dwFileAttributes;
				}
			}
			
			public override ResourceInfo Target{
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
			
			public override DateTime CreationTimeUtc{
				get{
					var info = Kernel32.GetFileInformationByHandle(handle);
					return Win32FileSystem.GetDateTime(info.ftCreationTime);
				}
			}
			
			public override DateTime LastAccessTimeUtc{
				get{
					var info = Kernel32.GetFileInformationByHandle(handle);
					return Win32FileSystem.GetDateTime(info.ftLastAccessTime);
				}
			}
			
			public override DateTime LastWriteTimeUtc{
				get{
					var info = Kernel32.GetFileInformationByHandle(handle);
					return Win32FileSystem.GetDateTime(info.ftLastWriteTime);
				}
			}
			
			public override long Length{
				get{
					var info = Kernel32.GetFileInformationByHandle(handle);
					unchecked{
						return ((long)((ulong)(uint)info.nFileSizeHigh << 32) | (long)(uint)info.nFileSizeLow);
					}
				}
			}
			
			public override Stream GetStream(FileMode mode, FileAccess access)
			{
				throw new NotImplementedException();
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
			
			public override string ContentType{
				get{
					throw new NotImplementedException();
				}
			}
		}
	}
}
