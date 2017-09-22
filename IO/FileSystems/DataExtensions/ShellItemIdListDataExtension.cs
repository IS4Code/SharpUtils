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
		
		protected override T GetPropertyInternal<T>(DataUri dataUri, ResourceProperty property)
		{
			switch(property)
			{
				case ResourceProperty.TargetUri:
					return To<T>.Cast(ShellFileSystem.Instance.LoadShellHandleUri(dataUri.Data));
				default:
					throw new NotImplementedException();
			}
		}
		
		protected override ResourceInfo GetTargetResourceInternal(DataUri dataUri)
		{
			return ShellFileSystem.Instance.LoadShellHandle(dataUri.Data);
		}
	}
}
