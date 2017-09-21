/* Date: 11.9.2017, Time: 2:09 */
using System;
using System.Collections.Generic;
using System.IO;
using IllidanS4.SharpUtils.IO.FileSystems;

namespace IllidanS4.SharpUtils.IO
{
	/// <summary>
	/// This class is implemented by individual file systems and represents
	/// a system-specific handle to a resource. The purpose of this class is
	/// to avoid parsing the URI each time to access the properties.
	/// </summary>
	/// <remarks>
	/// As the handle may contain unmanaged resources, call Dispose
	/// when you are finished with an instance of this class.
	/// </remarks>
	public abstract class ResourceHandle : ResourceInfo, IDisposable, IEquatable<ResourceHandle>
	{
		public ResourceHandle(IHandleProvider fileSystem) : base(fileSystem)
		{
			
		}
		
		public static ResourceHandle Create(ResourceInfo info)
		{
			var fs = info.FileSystem as IHandleProvider;
			if(fs != null)
			{
				return fs.ObtainHandle(info.Uri);
			}
			throw new ArgumentException("This resource's file system doesn't support handles.");
		}
		
		public abstract override Uri Uri{
			get;
		} 
		
		public abstract override FileAttributes Attributes{
			get;
		}
		
		public abstract override DateTime CreationTimeUtc{
			get;
		}
		
		public abstract override DateTime LastAccessTimeUtc{
			get;
		}
		
		public abstract override DateTime LastWriteTimeUtc{
			get;
		}
		
		public abstract override long Length{
			get;
		}
		
		public abstract override Stream GetStream(FileMode mode, FileAccess access);
		
		public abstract override ResourceInfo Target{
			get;
		}
		
		public abstract override string ContentType{
			get;
		}
		
		public abstract override string LocalPath{
			get;
		}
		
		public abstract override string DisplayPath{
			get;
		}
		
		public abstract override List<ResourceInfo> GetResources();
		
		public abstract override ResourceInfo Parent{
			get;
		}
		
		protected abstract void Dispose(bool disposing);
		
		public abstract bool Equals(ResourceHandle other);
		
		public sealed override bool Equals(object obj)
		{
			var handle = obj as ResourceHandle;
			if(handle != null) return Equals(handle);
			return false;
		}
		
		public abstract override int GetHashCode();
		
		public static bool operator ==(ResourceHandle lhs, ResourceHandle rhs)
		{
			if(ReferenceEquals(lhs, rhs))
				return true;
			if(ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(ResourceHandle lhs, ResourceHandle rhs)
		{
			return !(lhs == rhs);
		}
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~ResourceHandle()
		{
			Dispose(false);
		}
	}
}
