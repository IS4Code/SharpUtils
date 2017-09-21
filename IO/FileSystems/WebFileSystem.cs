/* Date: 3.9.2017, Time: 23:10 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
		
		public FileAttributes GetAttributes(Uri uri)
		{
			throw new NotImplementedException();
		}
		
		public DateTime GetCreationTime(Uri uri)
		{
			throw new NotImplementedException();
		}
		
		public DateTime GetLastAccessTime(Uri uri)
		{
			throw new NotImplementedException();
		}
		
		public DateTime GetLastWriteTime(Uri uri)
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
		
		public long GetLength(Uri uri)
		{
			return GetResponse(uri, "HEAD").ContentLength;
		}
		
		public Stream GetStream(Uri uri, FileMode mode, FileAccess access)
		{
			return GetResponse(uri, "GET").GetResponseStream();
		}
		
		public Uri GetTarget(Uri uri)
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
		
		public string GetContentType(Uri uri)
		{
			var resp = GetResponse(uri, "HEAD");
			return resp.ContentType;
		}
		
		public string GetLocalPath(Uri uri)
		{
			var resp = GetResponse(uri, "HEAD");
			return HttpUtility.UrlDecode(resp.ResponseUri.AbsolutePath);
		}
		
		public string GetDisplayPath(Uri uri)
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
		
		public Task<ResourceHandle> PerformOperationAsync(Uri uri, ResourceOperation operation, object arg)
		{
			throw new NotImplementedException();
		}
	}
}
