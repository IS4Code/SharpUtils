/* Date: 3.9.2017, Time: 23:10 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	/// <summary>
	/// This file systems contains web locations representing resources
	/// obtained using the standard web protocols.
	/// </summary>
	public class WebFileSystem : IFileSystem
	{
		public static readonly WebFileSystem Instance = new WebFileSystem();
		
		public T GetProperty<T>(Uri uri, ResourceProperty property)
		{
			switch(property)
			{
				/*case ResourceProperty.FileAttributes:
					break;
				case ResourceProperty.CreationTime:
					break;
				case ResourceProperty.LastAccessTime:
					break;*/
				case ResourceProperty.LastWriteTimeUtc:
					return To<T>.Cast(GetLastWriteTime(uri));
				case ResourceProperty.LongLength:
					return To<T>.Cast(GetLength(uri));
				case ResourceProperty.TargetUri:
					return To<T>.Cast(GetTarget(uri));
				case ResourceProperty.ContentType:
					return To<T>.Cast(GetContentType(uri));
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
		
		private DateTime GetLastWriteTime(Uri uri)
		{
			var resp = GetResponse(uri, "HEAD");
			var http = resp as HttpWebResponse;
			if(http != null)
			{
				return http.LastModified;
			}
			var ftp = resp as FtpWebResponse;
			if(ftp != null)
			{
				return ftp.LastModified;
			}
			
			throw new NotImplementedException();
		}
		
		private long GetLength(Uri uri)
		{
			return GetResponse(uri, "HEAD").ContentLength;
		}
		
		public Stream GetStream(Uri uri, FileMode mode, FileAccess access)
		{
			return GetResponse(uri, "GET").GetResponseStream();
		}
		
		private Uri GetTarget(Uri uri)
		{
			var resp = GetResponse(uri, "HEAD");
			var http = resp as HttpWebResponse;
			if(http != null)
			{
				int code = (int)http.StatusCode;
				if(300 <= code && code < 400)
				{
					string loc = http.Headers["Location"];
					if(loc != null)
					{
						return new Uri(uri, loc);
					}
				}
			}
			return null;
		}
		
		private string GetContentType(Uri uri)
		{
			var resp = GetResponse(uri, "HEAD");
			return resp.ContentType;
		}
		
		private string GetLocalPath(Uri uri)
		{
			var resp = GetResponse(uri, "HEAD");
			return HttpUtility.UrlDecode(resp.ResponseUri.AbsolutePath);
		}
		
		private string GetDisplayPath(Uri uri)
		{
			return HttpUtility.UrlDecode(uri.AbsolutePath);
		}
		
		public List<Uri> GetResources(Uri uri)
		{
			var request = WebRequest.Create(uri);
			
			var http = request as HttpWebRequest;
			if(http != null)
			{
				throw new NotImplementedException();
			}
			
			var ftp = request as FtpWebRequest;
			if(ftp != null)
			{
				var list = new List<Uri>();
				ftp.Method = WebRequestMethods.Ftp.ListDirectory;
				
				var resp = ftp.GetResponse();
				using(var stream = resp.GetResponseStream())
				{
					var reader = new StreamReader(stream);
					string line;
					while((line = reader.ReadLine()) != null)
					{
						if(line != "." && line != "..")
						{
							list.Add(new Uri(uri, line));
						}
					}
				}
				return list;
			}
			
			throw new NotImplementedException();
		}
		
		private WebResponse GetResponse(Uri uri, string method)
		{
			var request = WebRequest.Create(uri);
			request.Method = method;
			
			var http = request as HttpWebRequest;
			if(http != null)
			{
				http.AllowAutoRedirect = false;
			}
			
			return request.GetResponse();
		}
		
		public ResourceHandle PerformOperation(Uri uri, ResourceOperation operation, object arg)
		{
			var request = CreateOperation(uri, operation, arg);
			var resp = request.GetResponse();
			return null;
		}
		
		public async Task<ResourceHandle> PerformOperationAsync(Uri uri, ResourceOperation operation, object arg, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var request = CreateOperation(uri, operation, arg);
			cancellationToken.ThrowIfCancellationRequested();
			var resp = await request.GetResponseAsync();
			return null;
		}
		
		private WebRequest CreateOperation(Uri uri, ResourceOperation operation, object arg)
		{
			var request = WebRequest.Create(uri);
			
			var ftp = request as FtpWebRequest;
			if(ftp == null)
			{
				throw new NotImplementedException();
			}
			
			string path = arg as string;
			if(path == null)
			{
				var target = arg as Uri;
				if(target != null)
				{
					path = target.AbsolutePath;
				}
			}
			switch(operation)
			{
				case ResourceOperation.Create:
					throw new NotImplementedException();
				case ResourceOperation.Delete:
					request.Method = WebRequestMethods.Ftp.DeleteFile;
					break;
				case ResourceOperation.Move:
					request.Method = WebRequestMethods.Ftp.Rename;
					ftp.RenameTo = path;
					break;
				case ResourceOperation.Copy:
					throw new NotImplementedException();
				default:
					throw new NotImplementedException();
			}
			
			return request;
		}
	}
}
