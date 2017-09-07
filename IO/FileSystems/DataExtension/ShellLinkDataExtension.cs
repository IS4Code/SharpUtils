/* Date: 6.9.2017, Time: 16:22 */
using System;
using System.Diagnostics;
using System.IO;

namespace IllidanS4.SharpUtils.IO.FileSystems.DataExtension
{
	using DataUri = DataFileSystem.DataUri;
	
	/// <summary>
	/// Resolves links represented by the "application/x-ms-shortcut" MIME type.
	/// </summary>
	public class ShellLinkDataExtension : IDataExtension
	{
		private const string linkType = "application/x-ms-shortcut";
		
		public bool SupportsType(string contentType)
		{
			return contentType.Equals(linkType, StringComparison.OrdinalIgnoreCase);
		}
		
		[DebuggerStepThrough]
		private void CheckSupported(DataUri dataUri)
		{
			if(!SupportsType(dataUri.ContentType)) throw new ArgumentException("This content type is not supported.", "dataUri");
		}
		
		private Exception NotSupported()
		{
			return new NotSupportedException();
		}
		
		public FileAttributes GetAttributes(DataUri dataUri)
		{
			CheckSupported(dataUri);
			throw NotSupported();
		}
		
		public DateTime GetCreationTime(DataUri dataUri)
		{
			CheckSupported(dataUri);
			throw NotSupported();
		}
		
		public DateTime GetLastAccessTime(DataUri dataUri)
		{
			CheckSupported(dataUri);
			throw NotSupported();
		}
		
		public DateTime GetLastWriteTime(DataUri dataUri)
		{
			CheckSupported(dataUri);
			throw NotSupported();
		}
		
		public Uri GetTarget(DataUri dataUri)
		{
			CheckSupported(dataUri);
			return ShellFileSystem.Instance.LoadLinkTargetUri(dataUri.Data);
		}
	}
}
