/* Date: 5.9.2017, Time: 1:34 */
using System;
using System.Collections.Generic;
using System.IO;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	public abstract class MountFileSystem : IMountHost
	{
		private readonly Dictionary<Uri, IFileSystem> mountPoints = new Dictionary<Uri, IFileSystem>();
		
		public void Mount(Uri baseUrl, IFileSystem subSystem)
		{
			mountPoints.Add(baseUrl, subSystem);
		}
		
		public void Unmount(Uri baseUrl)
		{
			mountPoints.Remove(baseUrl);
		}
		
		public IFileSystem GetSubSystem(Uri url)
		{
			foreach(var pair in mountPoints)
			{
				var rel = pair.Key.MakeRelativeUri(url);
				if(pair.Key == url || (!rel.IsAbsoluteUri && !rel.OriginalString.StartsWith("../")))
				{
					return pair.Value;
				}
			}
			return null;
		}
		
		protected abstract FileAttributes GetAttributesInternal(Uri url);
		public FileAttributes GetAttributes(Uri url)
		{
			IFileSystem sub = GetSubSystem(url);
			if(sub != null) return sub.GetAttributes(url);
			
			return GetAttributesInternal(url);
		}
		
		protected abstract DateTime GetCreationTimeInternal(Uri url);
		public DateTime GetCreationTime(Uri url)
		{
			IFileSystem sub = GetSubSystem(url);
			if(sub != null) return sub.GetCreationTime(url);
			
			return GetCreationTimeInternal(url);
		}
		
		protected abstract DateTime GetLastAccessTimeInternal(Uri url);
		public DateTime GetLastAccessTime(Uri url)
		{
			IFileSystem sub = GetSubSystem(url);
			if(sub != null) return sub.GetLastAccessTime(url);
			
			return GetLastAccessTimeInternal(url);
		}
		
		protected abstract DateTime GetLastWriteTimeInternal(Uri url);
		public DateTime GetLastWriteTime(Uri url)
		{
			IFileSystem sub = GetSubSystem(url);
			if(sub != null) return sub.GetLastWriteTime(url);
			
			return GetLastWriteTimeInternal(url);
		}
		
		protected abstract long GetLengthInternal(Uri url);
		public long GetLength(Uri url)
		{
			IFileSystem sub = GetSubSystem(url);
			if(sub != null) return sub.GetLength(url);
			
			return GetLengthInternal(url);
		}
		
		protected abstract Stream GetStreamInternal(Uri url, FileMode mode, FileAccess access);
		public Stream GetStream(Uri url, FileMode mode, FileAccess access)
		{
			IFileSystem sub = GetSubSystem(url);
			if(sub != null) return sub.GetStream(url, mode, access);
			
			return GetStreamInternal(url, mode, access);
		}
		
		protected abstract Uri GetTargetInternal(Uri url);
		public Uri GetTarget(Uri url)
		{
			IFileSystem sub = GetSubSystem(url);
			if(sub != null) return sub.GetTarget(url);
			
			return GetTargetInternal(url);
		}
		
		protected abstract string GetContentTypeInternal(Uri url);
		public string GetContentType(Uri url)
		{
			IFileSystem sub = GetSubSystem(url);
			if(sub != null) return sub.GetContentType(url);
			
			return GetContentTypeInternal(url);
		}
	}
}
