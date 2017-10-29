/* Date: 5.9.2017, Time: 1:34 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	/// <summary>
	/// This file system implementation allows the environment to mount custom
	/// file systems to specific directories.
	/// </summary>
	public abstract class MountFileSystem : IMountHost
	{
		private readonly Dictionary<Uri, IFileSystem> mountPoints = new Dictionary<Uri, IFileSystem>();
		
		public void Mount(Uri baseUri, IFileSystem subSystem)
		{
			mountPoints.Add(baseUri, subSystem);
		}
		
		public void Unmount(Uri baseUri)
		{
			mountPoints.Remove(baseUri);
		}
		
		public IFileSystem GetSubSystem(Uri uri)
		{
			if(uri == null) return null;
			foreach(var pair in mountPoints)
			{
				var rel1 = pair.Key.MakeRelativeUri(uri);
				var rel2 = uri.MakeRelativeUri(pair.Key);
				if(pair.Key == uri || (!rel1.IsAbsoluteUri && !rel2.IsAbsoluteUri && !rel1.OriginalString.StartsWith("../") && rel2.OriginalString.StartsWith("../")))
				{
					return pair.Value;
				}
			}
			return null;
		}
		
		protected abstract T GetPropertyInternal<T>(Uri uri, ResourceProperty property);
		public T GetProperty<T>(Uri uri, ResourceProperty property)
		{
			IFileSystem sub = GetSubSystem(uri);
			if(sub != null) return sub.GetProperty<T>(uri, property);
			
			return GetPropertyInternal<T>(uri, property);
		}
		
		protected abstract void SetPropertyInternal<T>(Uri uri, ResourceProperty property, T value);
		public void SetProperty<T>(Uri uri, ResourceProperty property, T value)
		{
			IFileSystem sub = GetSubSystem(uri);
			if(sub != null)
			{
				sub.SetProperty<T>(uri, property, value);
			}else{
				SetPropertyInternal<T>(uri, property, value);
			}
		}
		
		protected abstract Stream GetStreamInternal(Uri uri, FileMode mode, FileAccess access);
		public Stream GetStream(Uri uri, FileMode mode, FileAccess access)
		{
			IFileSystem sub = GetSubSystem(uri);
			if(sub != null) return sub.GetStream(uri, mode, access);
			
			return GetStreamInternal(uri, mode, access);
		}
		
		protected abstract List<Uri> GetResourcesInternal(Uri uri);
		public List<Uri> GetResources(Uri uri)
		{
			IFileSystem sub = GetSubSystem(uri);
			if(sub != null) return sub.GetResources(uri);
			
			return GetResourcesInternal(uri);
		}
		
		protected abstract ResourceHandle PerformOperationInternal(Uri uri, ResourceOperation operation, object arg);
		public ResourceHandle PerformOperation(Uri uri, ResourceOperation operation, object arg)
		{
			IFileSystem sub = GetSubSystem(uri);
			if(sub != null) return sub.PerformOperation(uri, operation, arg);
			
			return PerformOperationInternal(uri, operation, arg);
		}
		
		protected abstract Task<ResourceHandle> PerformOperationAsyncInternal(Uri uri, ResourceOperation operation, object arg, CancellationToken cancellationToken);
		public Task<ResourceHandle> PerformOperationAsync(Uri uri, ResourceOperation operation, object arg, CancellationToken cancellationToken)
		{
			IFileSystem sub = GetSubSystem(uri);
			if(sub != null) return sub.PerformOperationAsync(uri, operation, arg, cancellationToken);
			
			return PerformOperationAsyncInternal(uri, operation, arg, cancellationToken);
		}
		
		protected abstract Process ExecuteInternal(Uri uri);
		public Process Execute(Uri uri)
		{
			IFileSystem sub = GetSubSystem(uri);
			if(sub != null) return sub.Execute(uri);
			
			return ExecuteInternal(uri);
		}
	}
}
