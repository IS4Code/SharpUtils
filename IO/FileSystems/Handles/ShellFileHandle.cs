/* Date: 11.9.2017, Time: 10:33 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
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
			
			public ShellFileHandle(IntPtr pidl, ShellFileSystem fs) : this(pidl, fs, false)
			{
				
			}
			
			private ShellFileHandle(IntPtr pidl, ShellFileSystem fs, bool own) : base(fs)
			{
				this.pidl = own ? pidl : Shell32.ILClone(pidl);
				this.fs = fs;
			}
			
			public ShellFileHandle(byte[] idl, ShellFileSystem fs) : this(LoadIdList(idl), fs, true)
			{
				
			}
			
			private static IntPtr LoadIdList(byte[] idl)
			{
				using(var buffer = new MemoryStream(idl))
				{
					return Shell32.ILLoadFromStreamEx(new StreamWrapper(buffer));
				}
			}
			
			public byte[] SaveIdList()
			{
				using(var buffer = new MemoryStream())
				{
					Shell32.ILSaveToStream(new StreamWrapper(buffer), pidl);
					return buffer.ToArray();
				}
			}
			
			private IShellItem GetItem()
			{
				return Shell32.SHCreateItemFromIDList<IShellItem>(pidl);
			}
			
			public override Uri Uri{
				get{
					return fs.GetShellUri(pidl, false);
				}
			}
			
			public override ResourceInfo Target{
				get{
					var item = GetItem();
					try{
						var targ = fs.GetTargetItem(item);
						
						string uri = targ as string;
						if(uri != null)
						{
							return new ResourceInfo(new Uri(uri));
						}
						var target = (IShellItem)targ;
						try{
							IntPtr pidl = Shell32.SHGetIDListFromObject(target);
							return new ShellFileHandle(pidl, fs, true);
						}finally{
							//TODO: this might release twice the same object
							Marshal.FinalReleaseComObject(target);
						}
					}finally{
						Marshal.FinalReleaseComObject(item);
					}
				}
			}
			
			public override ResourceInfo Parent{
				get{
					IntPtr pidl = Shell32.ILClone(this.pidl);
					Shell32.ILRemoveLastID(pidl);
					return new ShellFileHandle(pidl, fs, true);
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
				var list = new List<ResourceInfo>();
				
				var psf = Shell32.SHBindToObject<IShellFolder>(null, pidl, null);
				try{
					IEnumIDList peidl = psf.EnumObjects(fs.OwnerHwnd, SHCONTF.SHCONTF_FOLDERS | SHCONTF.SHCONTF_NONFOLDERS);
					
					if(peidl == null) return list;
					try{
						while(true)
						{
							IntPtr pidl2;
							int num;
							peidl.Next(1, out pidl2, out num);
							if(num == 0) break;
							try{
								IntPtr pidl3 = Shell32.ILCombine(pidl, pidl2);
								list.Add(new ShellFileHandle(pidl3, fs, true));
							}finally{
								Marshal.FreeCoTaskMem(pidl2);
							}
						}
					}finally{
						Marshal.FinalReleaseComObject(peidl);
					}
				}finally{
					Marshal.FinalReleaseComObject(psf);
				}
				
				return list;
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
