/* Date: 5.9.2017, Time: 1:33 */
using System;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	public interface IMountHost : IFileSystem
	{
		void Mount(Uri baseUrl, IFileSystem subSystem);
		void Unmount(Uri baseUrl);
		IFileSystem GetSubSystem(Uri url);
	}
}
