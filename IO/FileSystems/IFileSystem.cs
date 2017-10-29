/* Date: 3.9.2017, Time: 4:05 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	/// <summary>
	/// Represents a file system in the universal file system.
	/// </summary>
	public interface IFileSystem : IPropertyProvider<ResourceProperty>
	{
		//T GetProperty<T>(Uri uri, ResourceProperty property);
		Stream GetStream(Uri uri, FileMode mode, FileAccess access);
		List<Uri> GetResources(Uri uri);
		
		Process Execute(Uri uri);
		ResourceHandle PerformOperation(Uri uri, ResourceOperation operation, object arg);
		Task<ResourceHandle> PerformOperationAsync(Uri uri, ResourceOperation operation, object arg, CancellationToken cancellationToken);
	}
}
