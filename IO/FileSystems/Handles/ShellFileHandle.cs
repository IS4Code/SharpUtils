/* Date: 11.9.2017, Time: 10:33 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using IllidanS4.SharpUtils.COM;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	partial class ShellFileSystem
	{
		/// <summary>
		/// This class stores a handle to an item in the shell file system,
		/// using a pointer to its ITEMLIST (PIDL). The structure is copied
		/// on construction and is owned solely by the instance of this class.
		/// </summary>
		class ShellFileHandle : ResourceHandle
		{
			IntPtr pidl;
			ShellFileSystem fs;
			
			public ShellFileHandle(IntPtr pidl, ShellFileSystem fs) : base(fs)
			{
				this.pidl = Shell32.ILClone(pidl);
				this.fs = fs;
			}
			
			private IShellItem GetItem()
			{
				return Shell32.SHCreateItemFromIDList<IShellItem>(pidl);
			}
			
			public override Uri Uri{
				get{
					var item = GetItem();
					try{
						return fs.GetItemUri(item);
					}finally{
						Marshal.FinalReleaseComObject(item);
					}
				}
			}
			
			public override ResourceInfo Target{
				get{
					throw new NotImplementedException();
				}
			}
			
			public override ResourceInfo Parent{
				get{
					var item = (IShellItem2)GetItem();
					try{
						var parent = item.GetParent();
						try{
							IntPtr pidl = Shell32.SHGetIDListFromObject(parent);
							try{
								return new ShellFileHandle(pidl, fs);
							}finally{
								Marshal.FreeCoTaskMem(pidl);
							}
						}finally{
							Marshal.FinalReleaseComObject(parent);
						}
					}finally{
						Marshal.FinalReleaseComObject(item);
					}
				}
			}
			
			public override DateTime CreationTimeUtc{
				get{
					var item = (IShellItem2)GetItem();
					try{
						FILETIME ft = item.GetFileTime(Shell32.PKEY_DateCreated);
						return Win32FileSystem.GetDateTime(ft);
					}finally{
						Marshal.FinalReleaseComObject(item);
					}
				}
			}
			
			public override DateTime LastAccessTimeUtc{
				get{
					var item = (IShellItem2)GetItem();
					try{
						FILETIME ft = item.GetFileTime(Shell32.PKEY_DateAccessed);
						return Win32FileSystem.GetDateTime(ft);
					}finally{
						Marshal.FinalReleaseComObject(item);
					}
				}
			}
			
			public override DateTime LastWriteTimeUtc{
				get{
					var item = (IShellItem2)GetItem();
					try{
						FILETIME ft = item.GetFileTime(Shell32.PKEY_DateModified);
						return Win32FileSystem.GetDateTime(ft);
					}finally{
						Marshal.FinalReleaseComObject(item);
					}
				}
			}
			
			public override long Length{
				get{
					var item = (IShellItem2)GetItem();
					try{
						return (long)item.GetUInt64(Shell32.PKEY_Size);
					}finally{
						Marshal.FinalReleaseComObject(item);
					}
				}
			}
			
			public override Stream GetStream(FileMode mode, FileAccess access)
			{
				var item = GetItem();
				try{
					var stream = item.BindToHandler<IStream>(null, Shell32.BHID_Stream);
					return new IStreamWrapper(stream);
				}finally{
					Marshal.FinalReleaseComObject(item);
				}
			}
			
			public override List<ResourceInfo> GetResources()
			{
				throw new NotImplementedException();
			}
			
			protected override void Dispose(bool disposing)
			{
				if(pidl != IntPtr.Zero)
				{
					Marshal.FreeCoTaskMem(pidl);
					pidl = IntPtr.Zero;
				}
			}
			
			public override string ContentType{
				get{
					throw new NotImplementedException();
				}
			}
			
			public override FileAttributes Attributes{
				get{
					throw new NotImplementedException();
				}
			}
		}
	}
}
