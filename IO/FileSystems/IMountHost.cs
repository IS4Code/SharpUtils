/* Date: 5.9.2017, Time: 1:33 */
using System;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	/// <summary>
	/// Represents a file system whose directories may be used as mount points
	/// for another file systems.
	/// </summary>
	public interface IMountHost : IFileSystem
	{
		void Mount(Uri baseUrl, IFileSystem subSystem);
		void Unmount(Uri baseUrl);
		IFileSystem GetSubSystem(Uri url);
	}
}
