/* Date: 7.9.2017, Time: 13:51 */
using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Web;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	public class PipeFileSystem : IFileSystem
	{
		public static readonly PipeFileSystem Instance = new PipeFileSystem();
		
		public FileAttributes GetAttributes(Uri uri)
		{
			return FileAttributes.Normal;
		}
		
		public DateTime GetCreationTime(Uri uri)
		{
			throw new NotSupportedException();
		}
		
		public DateTime GetLastAccessTime(Uri uri)
		{
			throw new NotSupportedException();
		}
		
		public DateTime GetLastWriteTime(Uri uri)
		{
			throw new NotSupportedException();
		}
		
		public long GetLength(Uri uri)
		{
			throw new NotSupportedException();
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
		
		public Uri GetTarget(Uri uri)
		{
			return new UriBuilder("file", uri.Host, uri.Port, @"\pipe\"+uri.AbsolutePath).Uri;
		}
		
		public string GetContentType(Uri uri)
		{
			throw new NotSupportedException();
		}
		
		public string GetLocalPath(Uri uri)
		{
			string server = uri.Host;
			if(String.IsNullOrEmpty(server)) server = ".";
			string path = HttpUtility.UrlDecode(uri.AbsolutePath);
			return String.Format(@"\\{0}\{1}", server, path);
		}
		
		public string GetDisplayPath(Uri uri)
		{
			return HttpUtility.UrlDecode(uri.AbsolutePath);
		}
		
		public System.Collections.Generic.List<Uri> GetResources(Uri uri)
		{
			throw new NotImplementedException();
		}
	}
}
