/* Date: 5.9.2017, Time: 1:34 */
using System;
using System.Collections.Generic;
using System.IO;

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
			foreach(var pair in mountPoints)
			{
				var rel = pair.Key.MakeRelativeUri(uri);
				if(pair.Key == uri || (!rel.IsAbsoluteUri && !rel.OriginalString.StartsWith("../")))
				{
					return pair.Value;
				}
			}
			return null;
		}
		
		protected abstract FileAttributes GetAttributesInternal(Uri uri);
		public FileAttributes GetAttributes(Uri uri)
		{
			IFileSystem sub = GetSubSystem(uri);
			if(sub != null) return sub.GetAttributes(uri);
			
			return GetAttributesInternal(uri);
		}
		
		protected abstract DateTime GetCreationTimeInternal(Uri uri);
		public DateTime GetCreationTime(Uri uri)
		{
			IFileSystem sub = GetSubSystem(uri);
			if(sub != null) return sub.GetCreationTime(uri);
			
			return GetCreationTimeInternal(uri);
		}
		
		protected abstract DateTime GetLastAccessTimeInternal(Uri uri);
		public DateTime GetLastAccessTime(Uri uri)
		{
			IFileSystem sub = GetSubSystem(uri);
			if(sub != null) return sub.GetLastAccessTime(uri);
			
			return GetLastAccessTimeInternal(uri);
		}
		
		protected abstract DateTime GetLastWriteTimeInternal(Uri uri);
		public DateTime GetLastWriteTime(Uri uri)
		{
			IFileSystem sub = GetSubSystem(uri);
			if(sub != null) return sub.GetLastWriteTime(uri);
			
			return GetLastWriteTimeInternal(uri);
		}
		
		protected abstract long GetLengthInternal(Uri uri);
		public long GetLength(Uri uri)
		{
			IFileSystem sub = GetSubSystem(uri);
			if(sub != null) return sub.GetLength(uri);
			
			return GetLengthInternal(uri);
		}
		
		protected abstract Stream GetStreamInternal(Uri uri, FileMode mode, FileAccess access);
		public Stream GetStream(Uri uri, FileMode mode, FileAccess access)
		{
			IFileSystem sub = GetSubSystem(uri);
			if(sub != null) return sub.GetStream(uri, mode, access);
			
			return GetStreamInternal(uri, mode, access);
		}
		
		protected abstract Uri GetTargetInternal(Uri uri);
		public Uri GetTarget(Uri uri)
		{
			IFileSystem sub = GetSubSystem(uri);
			if(sub != null) return sub.GetTarget(uri);
			
			return GetTargetInternal(uri);
		}
		
		protected abstract string GetContentTypeInternal(Uri uri);
		public string GetContentType(Uri uri)
		{
			IFileSystem sub = GetSubSystem(uri);
			if(sub != null) return sub.GetContentType(uri);
			
			return GetContentTypeInternal(uri);
		}
	}
}
