/* Date: 6.9.2017, Time: 16:22 */
using System;
using System.IO;

namespace IllidanS4.SharpUtils.IO.FileSystems.DataExtensions
{
	using DataUri = DataFileSystem.DataUri;
	
	/// <summary>
	/// Resolves links represented by the "application/x-ms-shortcut" MIME type.
	/// </summary>
	public class ShellLinkDataExtension : DataExtension
	{
		public ShellLinkDataExtension() : base("application/x-ms-shortcut")
		{
			
		}
		
		protected override T GetPropertyInternal<T>(DataUri dataUri, ResourceProperty property)
		{
			switch(property)
			{
				case ResourceProperty.TargetUri:
					return To<T>.Cast(ShellFileSystem.Instance.LoadLinkTargetUri(dataUri.Data));
				case ResourceProperty.TargetInfo:
					return To<T>.Cast<ResourceInfo>(ShellFileSystem.Instance.LoadLinkTargetResource(dataUri.Data));
				default:
					throw new NotSupportedException();
			}
		}
	}
}
