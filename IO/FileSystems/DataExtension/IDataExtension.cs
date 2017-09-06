/* Date: 6.9.2017, Time: 16:19 */
using System;
using System.Collections;
using System.IO;

namespace IllidanS4.SharpUtils.IO.FileSystems.DataExtension
{
	using DataUri = DataFileSystem.DataUri;
	/// <summary>
	/// Provides extended information about a resource based on its content rather than metadata.
	/// </summary>
	public interface IDataExtension
	{
		bool SupportsType(string contentType);
		
		FileAttributes GetAttributes(DataUri dataUri);
		DateTime GetCreationTime(DataUri dataUri);
		DateTime GetLastAccessTime(DataUri dataUri);
		DateTime GetLastWriteTime(DataUri dataUri);
		Uri GetTarget(DataUri dataUri);
	}
}
