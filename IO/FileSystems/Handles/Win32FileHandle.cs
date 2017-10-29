/* Date: 13.9.2017, Time: 14:16 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	partial class Win32FileSystem
	{
		class Win32FileHandle : ResourceHandle, IPropertyProviderResource<Win32FileProperty>
		{
			IntPtr handle;
			Win32FileSystem fs;
			
			public Win32FileHandle(IntPtr handle, Win32FileSystem fs) : this(handle, fs, false)
			{
				IntPtr proc = Kernel32.GetCurrentProcess();
				
				this.handle = Kernel32.DuplicateHandle(proc, handle, proc, 0, true, 2);
				this.fs = fs;
			}
			
			private Win32FileHandle(IntPtr handle, Win32FileSystem fs, bool own) : base(fs)
			{
				if(own)
				{
					this.handle = handle;
				}else{
					IntPtr proc = Kernel32.GetCurrentProcess();
					this.handle = Kernel32.DuplicateHandle(proc, handle, proc, 0, true, 2);
				}
				this.fs = fs;
			}
			
			private IntPtr CloneHandle()
			{
				IntPtr proc = Kernel32.GetCurrentProcess();
				return Kernel32.DuplicateHandle(proc, handle, proc, 0, true, 2);
			}
			
			public override Uri Uri{
				get{
					return fs.FileUriFromPath(LocalPath);
				}
			}
			
			protected override FileAttributes Attributes{
				get{
					try{
						var info = Kernel32.GetFileInformationByHandle(handle);
						return (FileAttributes)info.dwFileAttributes;
					}catch(Win32Exception)
					{
						return FileAttributes.Device;
					}
				}
			}
			
			protected override Uri TargetUri{
				get{
					int flags;
					string path = GetSubstituteName(out flags);
					if(path == null) return null;
					if(flags == 0)
					{
						return fs.FileUriFromPath(path);
					}else{
						return new Uri(Uri, path);
					}
				}
			}
			
			private string GetSubstituteName(out int flags)
			{
				if((Attributes & FileAttributes.ReparsePoint) == 0)
				{
					flags = 0;
					return null;
				}
				unsafe{
					byte* buffer = stackalloc byte[4096];
					int size = Kernel32.DeviceIoControl(handle, 0x900A8, IntPtr.Zero, 0, (IntPtr)buffer, 4096);
					var data = (Kernel32.REPARSE_DATA_BUFFER*)buffer;
					
					char* pbuffer;
					int length;
					switch(data->ReparseTag)
					{
						case -1610612724:
							var symlink = &data->SymbolicLink;
							pbuffer = &symlink->PathBuffer + symlink->SubstituteNameOffset/2;
							length = symlink->SubstituteNameLength/2;
							flags = symlink->Flags;
							break;
						case -1610612733:
							var junction = &data->MountPoint;
							pbuffer = &junction->PathBuffer + junction->SubstituteNameOffset/2;
							length = junction->SubstituteNameLength/2;
							flags = 0;
							break;
						default:
							flags = 0;
							return null;
					}
					return new String(pbuffer, 0, length);
				}
			}
			
			private string GetPrintName()
			{
				if((Attributes & FileAttributes.ReparsePoint) == 0) return null;
				unsafe{
					byte* buffer = stackalloc byte[4096];
					int size = Kernel32.DeviceIoControl(handle, 0x900A8, IntPtr.Zero, 0, (IntPtr)buffer, 4096);
					var data = (Kernel32.REPARSE_DATA_BUFFER*)buffer;
					
					char* pbuffer;
					int length;
					switch(data->ReparseTag)
					{
						case -1610612724:
							var symlink = &data->SymbolicLink;
							pbuffer = &symlink->PathBuffer + symlink->PrintNameOffset/2;
							length = symlink->PrintNameLength/2;
							break;
						case -1610612733:
							var junction = &data->MountPoint;
							pbuffer = &junction->PathBuffer + junction->PrintNameOffset/2;
							length = junction->PrintNameLength/2;
							break;
						default:
							return null;
					}
					return new String(pbuffer, 0, length);
				}
			}
			
			protected override ResourceInfo TargetInfo{
				get{
					return new ResourceInfo(TargetUri);
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
				return new DeviceStream(Kernel32.ReOpenFile(handle, access, DefaultFileShare, DefaultFileFlags), access);
			}
			
			public override Process Execute()
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
			
			protected override string ContentType{
				get{
					throw new NotImplementedException();
				}
			}
			
			protected override string LocalPath{
				get{
					try{
						return Kernel32.GetFinalPathNameByHandle(handle, 0);
					}catch(Win32Exception)
					{
						Ntdll.OBJECT_NAME_INFORMATION nameInfo;
						Ntdll.NtQueryObject(handle, out nameInfo);
						return @"\\?\GlobalRoot"+nameInfo.Buffer;
					}
				}
			}
			
			protected override string DisplayPath{
				get{
					try{
						return Kernel32.GetFinalPathNameByHandle(handle, 4);
					}catch(Win32Exception)
					{
						Ntdll.OBJECT_NAME_INFORMATION nameInfo;
						Ntdll.NtQueryObject(handle, out nameInfo);
						return nameInfo.Buffer;
					}
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
			
			public T GetProperty<T>(Win32FileProperty property)
			{
				switch(property)
				{
					case Win32FileProperty.DosPath:
					case Win32FileProperty.GuidPath:
					case Win32FileProperty.BarePath:
					case Win32FileProperty.DevicePath:
						int flag = property-Win32FileProperty.DosPath;
						return To<T>.Cast(Kernel32.GetFinalPathNameByHandle(handle, flag));
					case Win32FileProperty.LinkPrintName:
						return To<T>.Cast(GetPrintName());
					case Win32FileProperty.LinkSubstituteName:
						int flags;
						return To<T>.Cast(GetSubstituteName(out flags));
					default:
						throw new NotImplementedException();
				}
			}
			
			public void SetProperty<T>(Win32FileProperty property, T value)
			{
				throw new NotImplementedException();
			}
		}
	}
}
