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
		
		protected abstract T GetPropertyInternal<T>(DataUri dataUri, ResourceProperty property);
		public T GetProperty<T>(DataUri dataUri, ResourceProperty property)
		{
			CheckSupported(dataUri);
			return GetPropertyInternal<T>(dataUri, property);
		}
		
		protected abstract ResourceInfo GetTargetResourceInternal(DataUri dataUri);
		public ResourceInfo GetTargetResource(DataUri dataUri)
		{
			CheckSupported(dataUri);
			return GetTargetResourceInternal(dataUri);
		}
	}
}
