/* Date: 3.9.2017, Time: 23:10 */
using System;
using System.IO;
using System.Net;

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
			return uri;
		}
		
		public string GetContentType(Uri uri)
		{
			var resp = GetResponse(uri, "HEAD");
			return resp.ContentType;
		}
		
		private WebResponse GetResponse(Uri uri, string method)
		{
			var request = WebRequest.Create(uri);
			request.Method = "HEAD";
			
			var http = request as HttpWebRequest;
			if(http != null)
			{
				http.AllowAutoRedirect = false;
			}
			
			return request.GetResponse();
		}
	}
}
