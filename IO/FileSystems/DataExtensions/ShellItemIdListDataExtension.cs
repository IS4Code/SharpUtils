/* Date: 18.9.2017, Time: 13:56 */
using System;
using System.IO;

namespace IllidanS4.SharpUtils.IO.FileSystems.DataExtensions
{
	using DataUri = DataFileSystem.DataUri;
	
	/// <summary>
	/// Resolves links represented by the "application/x-ms-itemidlist" MIME type.
	/// </summary>
	public class ShellItemIdListDataExtension : DataExtension
	{
		public ShellItemIdListDataExtension() : base("application/x-ms-itemidlist")
		{
			
		}
		
		protected override FileAttributes GetAttributesInternal(DataUri dataUri)
		{
			throw new NotImplementedException();
		}
		
		protected override DateTime GetCreationTimeInternal(DataUri dataUri)
		{
			throw new NotImplementedException();
		}
		
		protected override DateTime GetLastAccessTimeInternal(DataUri dataUri)
		{
			throw new NotImplementedException();
		}
		
		protected override DateTime GetLastWriteTimeInternal(DataUri dataUri)
		{
			throw new NotImplementedException();
		}
		
		protected override Uri GetTargetInternal(DataUri dataUri)
		{
			return ShellFileSystem.Instance.LoadShellHandleUri(dataUri.Data);
		}
		
		protected override ResourceInfo GetTargetResourceInternal(DataUri dataUri)
		{
			return ShellFileSystem.Instance.LoadShellHandle(dataUri.Data);
		}
	}
}
