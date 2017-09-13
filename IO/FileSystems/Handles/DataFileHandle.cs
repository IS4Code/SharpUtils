/* Date: 13.9.2017, Time: 13:39 */
using System;
using System.Collections.Generic;
using System.IO;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	partial class DataFileSystem
	{
		class DataFileHandle : ResourceHandle
		{
			DataUri data;
			DataFileSystem fs;
			
			public DataFileHandle(DataUri data, DataFileSystem fs) : base(fs)
			{
				this.data = data;
				this.fs = fs;
			}
			
			public override Uri Uri{
				get{
					return data.Uri;
				}
			}
			
			public override FileAttributes Attributes{
				get{
					var ext = fs.GetExtension(ContentType);
					if(ext != null) return ext.GetAttributes(data);
					
					throw new NotImplementedException();
				}
			}
			
			public override ResourceInfo Target{
				get{
					var ext = fs.GetExtension(ContentType);
					if(ext != null) return ext.GetTargetResource(data);
					
					return this;
				}
			}
			
			public override ResourceInfo Parent{
				get{
					throw new NotImplementedException();
				}
			}
			
			public override DateTime CreationTimeUtc{
				get{
					var ext = fs.GetExtension(ContentType);
					if(ext != null) return ext.GetCreationTime(data);
					
					throw new NotImplementedException();
				}
			}
			
			public override DateTime LastAccessTimeUtc{
				get{
					var ext = fs.GetExtension(ContentType);
					if(ext != null) return ext.GetLastAccessTime(data);
					
					throw new NotImplementedException();
				}
			}
			
			public override DateTime LastWriteTimeUtc{
				get{
					var ext = fs.GetExtension(ContentType);
					if(ext != null) return ext.GetLastWriteTime(data);
					
					throw new NotImplementedException();
				}
			}
			
			public override long Length{
				get{
					return data.Data.LongLength;
				}
			}
			
			public override Stream GetStream(FileMode mode, FileAccess access)
			{
				return new MemoryStream(data.Data, false);
			}
			
			public override List<ResourceInfo> GetResources()
			{
				throw new NotImplementedException();
			}
			
			protected override void Dispose(bool disposing)
			{
				
			}
			
			public override string ContentType{
				get{
					return data.ContentType;
				}
			}
		}
	}
}
