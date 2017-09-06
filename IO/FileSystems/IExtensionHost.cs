/* Date: 6.9.2017, Time: 16:32 */
using System;
using IllidanS4.SharpUtils.IO.FileSystems.DataExtension;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	/// <summary>
	/// Represents a file system which can register extensions
	/// based on a file content type to provide additional
	/// metadata about the files.
	/// </summary>
	public interface IExtensionHost : IFileSystem
	{
		void Register(IDataExtension extension);
		void Unregister(IDataExtension extension);
		IDataExtension GetExtension(string contentType);
	}
}
