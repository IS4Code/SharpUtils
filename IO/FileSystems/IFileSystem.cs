/* Date: 3.9.2017, Time: 4:05 */
using System;
using System.IO;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	public interface IFileSystem
	{
		FileAttributes GetAttributes(Uri url);
		DateTime GetCreationTime(Uri url);
		DateTime GetLastAccessTime(Uri url);
		DateTime GetLastWriteTime(Uri url);
		long GetLength(Uri url);
		Stream GetStream(Uri url, FileMode mode, FileAccess access);
		Uri GetTarget(Uri url);
		string GetContentType(Uri url);
	}
}
