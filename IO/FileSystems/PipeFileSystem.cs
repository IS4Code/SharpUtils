/* Date: 7.9.2017, Time: 13:51 */
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	public class PipeFileSystem : IFileSystem
	{
		public static readonly PipeFileSystem Instance = new PipeFileSystem();
		
		public T GetProperty<T>(Uri uri, ResourceProperty property)
		{
			switch(property)
			{
				case ResourceProperty.FileAttributes:
					return To<T>.Cast(FileAttributes.Normal);
				case ResourceProperty.CreationTimeUtc:
				case ResourceProperty.LastAccessTimeUtc:
				case ResourceProperty.LastWriteTimeUtc:
				case ResourceProperty.LongLength:
				case ResourceProperty.ContentType:
					throw new NotSupportedException();
				case ResourceProperty.TargetUri:
					return To<T>.Cast(GetTarget(uri));
				case ResourceProperty.LocalPath:
					return To<T>.Cast(GetLocalPath(uri));
				case ResourceProperty.DisplayPath:
					return To<T>.Cast(GetDisplayPath(uri));
				default:
					throw new NotImplementedException();
			}
		}
		
		public void SetProperty<T>(Uri uri, ResourceProperty property, T value)
		{
			throw new NotImplementedException();
		}
		
		public Stream GetStream(Uri uri, FileMode mode, FileAccess access)
		{
			string server = uri.Host;
			if(String.IsNullOrEmpty(server)) server = ".";
			string path = HttpUtility.UrlDecode(uri.AbsolutePath);
			PipeDirection direction;
			switch(access)
			{
				case FileAccess.Read:
					direction = PipeDirection.In;
					break;
				case FileAccess.ReadWrite:
					direction = PipeDirection.InOut;
					break;
				case FileAccess.Write:
					direction = PipeDirection.Out;
					break;
				default:
					throw new ArgumentException(null, "mode");
			}
			
			var pipe = new NamedPipeClientStream(server, path, direction, PipeOptions.Asynchronous);
			pipe.Connect();
			return pipe;
		}
		
		private Uri GetTarget(Uri uri)
		{
			return new UriBuilder("file", uri.Host, uri.Port, @"\pipe\"+uri.AbsolutePath).Uri;
		}
		
		private string GetLocalPath(Uri uri)
		{
			string server = uri.Host;
			if(String.IsNullOrEmpty(server)) server = ".";
			string path = HttpUtility.UrlDecode(uri.AbsolutePath);
			return String.Format(@"\\{0}\{1}", server, path);
		}
		
		private string GetDisplayPath(Uri uri)
		{
			return HttpUtility.UrlDecode(uri.AbsolutePath);
		}
		
		List<Uri> IFileSystem.GetResources(Uri uri)
		{
			throw new NotImplementedException();
		}
		
		ResourceHandle IFileSystem.PerformOperation(Uri uri, ResourceOperation operation, object arg)
		{
			throw new NotImplementedException();
		}
		
		Task<ResourceHandle> IFileSystem.PerformOperationAsync(Uri uri, ResourceOperation operation, object arg, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
		
		System.Diagnostics.Process IFileSystem.Execute(Uri uri)
		{
			throw new NotImplementedException();
		}
	}
}
