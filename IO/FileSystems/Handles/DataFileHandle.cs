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
					if(ext != null) return ext.GetProperty<FileAttributes>(data, ResourceProperty.FileAttributes);
					
					throw new NotImplementedException();
				}
			}
			
			public override ResourceInfo Target{
				get{
					var ext = fs.GetExtension(ContentType);
					if(ext != null) return ext.GetTargetResource(data);
					
					return null;
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
					if(ext != null) return ext.GetProperty<DateTime>(data, ResourceProperty.CreationTimeUtc);
					
					throw new NotImplementedException();
				}
			}
			
			public override DateTime LastAccessTimeUtc{
				get{
					var ext = fs.GetExtension(ContentType);
					if(ext != null) return ext.GetProperty<DateTime>(data, ResourceProperty.LastAccessTimeUtc);
					
					throw new NotImplementedException();
				}
			}
			
			public override DateTime LastWriteTimeUtc{
				get{
					var ext = fs.GetExtension(ContentType);
					if(ext != null) return ext.GetProperty<DateTime>(data, ResourceProperty.LastWriteTimeUtc);
					
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
			
			public override string LocalPath{
				get{
					var ext = fs.GetExtension(ContentType);
					if(ext != null) return ext.GetProperty<string>(data, ResourceProperty.LocalPath);
					
					throw new NotImplementedException();
				}
			}
			
			public override string DisplayPath{
				get{
					var ext = fs.GetExtension(ContentType);
					if(ext != null) return ext.GetProperty<string>(data, ResourceProperty.DisplayPath);
					
					throw new NotImplementedException();
				}
			}
			
			public override int GetHashCode()
			{
				return data.Uri.GetHashCode();
			}
			
			public override bool Equals(ResourceHandle other)
			{
				var handle = (DataFileHandle)other;
				if(handle != null) return data.Uri.Equals(handle.data.Uri);
				return false;
			}
		}
	}
}
