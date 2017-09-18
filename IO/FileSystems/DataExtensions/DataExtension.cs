/* Date: 18.9.2017, Time: 13:51 */
using System;
using System.Diagnostics;
using System.IO;

namespace IllidanS4.SharpUtils.IO.FileSystems.DataExtensions
{
	using DataUri = DataFileSystem.DataUri;
	
	public abstract class DataExtension : IDataExtension
	{
		public string ContentType{get; private set;}
		
		public DataExtension(string contentType)
		{
			if(contentType == null) throw new ArgumentNullException("contentType");
			ContentType = contentType;
		}
		
		public bool SupportsType(string contentType)
		{
			return ContentType.Equals(contentType, StringComparison.OrdinalIgnoreCase);
		}
		
		[DebuggerStepThrough]
		private void CheckSupported(DataUri dataUri)
		{
			if(!SupportsType(dataUri.ContentType)) throw new ArgumentException("This content type is not supported.", "dataUri");
		}
		
		protected abstract FileAttributes GetAttributesInternal(DataUri dataUri);
		public FileAttributes GetAttributes(DataUri dataUri)
		{
			CheckSupported(dataUri);
			return GetAttributesInternal(dataUri);
		}
		
		protected abstract DateTime GetCreationTimeInternal(DataUri dataUri);
		public DateTime GetCreationTime(DataUri dataUri)
		{
			CheckSupported(dataUri);
			return GetCreationTimeInternal(dataUri);
		}
		
		protected abstract DateTime GetLastAccessTimeInternal(DataUri dataUri);
		public DateTime GetLastAccessTime(DataUri dataUri)
		{
			CheckSupported(dataUri);
			return GetLastAccessTimeInternal(dataUri);
		}
		
		protected abstract DateTime GetLastWriteTimeInternal(DataUri dataUri);
		public DateTime GetLastWriteTime(DataUri dataUri)
		{
			CheckSupported(dataUri);
			return GetLastWriteTimeInternal(dataUri);
		}
		
		protected abstract Uri GetTargetInternal(DataUri dataUri);
		public Uri GetTarget(DataUri dataUri)
		{
			CheckSupported(dataUri);
			return GetTargetInternal(dataUri);
		}
		
		protected abstract ResourceInfo GetTargetResourceInternal(DataUri dataUri);
		public ResourceInfo GetTargetResource(DataUri dataUri)
		{
			CheckSupported(dataUri);
			return GetTargetResourceInternal(dataUri);
		}
	}
}
