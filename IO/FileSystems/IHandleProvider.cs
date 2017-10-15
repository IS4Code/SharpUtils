/* Date: 11.9.2017, Time: 10:28 */
using System;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	/// <summary>
	/// Represents a file system that can provide a handle to its resources.
	/// A handle is used to access a file without parsing its URI.
	/// </summary>
	public interface IHandleProvider : IFileSystem
	{
		ResourceHandle ObtainHandle(Uri uri, ResourceFlags flags);
	}
}
