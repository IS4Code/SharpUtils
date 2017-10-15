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
		
		public static ResourceHandle Create(ResourceInfo info, ResourceFlags flags)
		{
			var fs = info.FileSystem as IHandleProvider;
			if(fs != null)
			{
				return fs.ObtainHandle(info.Uri, flags);
			}
			throw new ArgumentException("This resource's file system doesn't support handles.");
		}
		
		public abstract override Uri Uri{
			get;
		} 
		
		protected new abstract FileAttributes Attributes{
			get;
		}
		
		protected new abstract DateTime CreationTimeUtc{
			get;
		}
		
		protected new abstract DateTime LastAccessTimeUtc{
			get;
		}
		
		protected new abstract DateTime LastWriteTimeUtc{
			get;
		}
		
		protected new abstract long Length{
			get;
		}
		
		public abstract override Stream GetStream(FileMode mode, FileAccess access);
		
		public sealed override ResourceInfo Target{
			get{
				return GetProperty<ResourceInfo>(ResourceProperty.TargetInfo);
			}
		}
		
		protected abstract Uri TargetUri{
			get;
		}
		
		protected abstract ResourceInfo TargetInfo{
			get;
		}
		
		protected new abstract string ContentType{
			get;
		}
		
		protected new abstract string LocalPath{
			get;
		}
		
		protected new abstract string DisplayPath{
			get;
		}
		
		protected virtual TValue GetPropertyInternal<TValue>(ResourceProperty property)
		{
			throw new NotSupportedException();
		}
		
		public sealed override TValue GetProperty<TValue>(ResourceProperty property)
		{
			switch(property)
			{
				case ResourceProperty.FileAttributes:
					return To<TValue>.Cast(Attributes);
				case ResourceProperty.CreationTimeUtc:
					return To<TValue>.Cast(CreationTimeUtc);
				case ResourceProperty.LastAccessTimeUtc:
					return To<TValue>.Cast(LastAccessTimeUtc);
				case ResourceProperty.LastWriteTimeUtc:
					return To<TValue>.Cast(LastWriteTimeUtc);
				case ResourceProperty.LongLength:
					return To<TValue>.Cast(Length);
				case ResourceProperty.ContentType:
					return To<TValue>.Cast(ContentType);
				case ResourceProperty.LocalPath:
					return To<TValue>.Cast(LocalPath);
				case ResourceProperty.DisplayPath:
					return To<TValue>.Cast(DisplayPath);
				case ResourceProperty.TargetInfo:
					return To<TValue>.Cast(TargetInfo);
				default:
					return GetPropertyInternal<TValue>(property);
			}
		}
		
		public sealed override TValue GetCustomProperty<TValue, TProperty>(TProperty property)
		{
			var provider = this as IPropertyProviderResource<TProperty>;
			if(provider != null)
			{
				return provider.GetProperty<TValue>(property);
			}else{
				throw new NotSupportedException();
			}
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
