/* Date: 5.9.2017, Time: 0:19 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using IllidanS4.SharpUtils.IO.FileSystems.DataExtension;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	/// <summary>
	/// This file system handles URI using the "data" scheme.
	/// </summary>
	public class DataFileSystem : IExtensionHost
	{
		public static readonly DataFileSystem Instance = new DataFileSystem();
		
		public DataFileSystem()
		{
			Register(new ShellLinkDataExtension());
		}
		
		private readonly List<IDataExtension> extensions = new List<IDataExtension>();
		
		public void Register(IDataExtension extension)
		{
			extensions.Add(extension);
		}
		
		public void Unregister(IDataExtension extension)
		{
			extensions.Remove(extension);
		}
		
		public IDataExtension GetExtension(string contentType)
		{
			foreach(var extension in extensions)
			{
				if(extension.SupportsType(contentType)) return extension;
			}
			return null;
		}
		
		private IDataExtension ParseUrl(Uri url, out DataUri uri)
		{
			uri = new DataUri(url, false, false);
			var ext = GetExtension(uri.ContentType);
			if(ext != null)
			{
				uri = new DataUri(url, true, true);
				return ext;
			}
			return null;
		}
		
		public FileAttributes GetAttributes(Uri url)
		{
			DataUri uri;
			var extension = ParseUrl(url, out uri);
			if(extension != null) return extension.GetAttributes(uri);
			
			return FileAttributes.ReadOnly;
		}
		
		public DateTime GetCreationTime(Uri url)
		{
			DataUri uri;
			var extension = ParseUrl(url, out uri);
			if(extension != null) return extension.GetCreationTime(uri);
			
			throw new NotImplementedException();
		}
		
		public DateTime GetLastAccessTime(Uri url)
		{
			DataUri uri;
			var extension = ParseUrl(url, out uri);
			if(extension != null) return extension.GetLastAccessTime(uri);
			
			throw new NotImplementedException();
		}
		
		public DateTime GetLastWriteTime(Uri url)
		{
			DataUri uri;
			var extension = ParseUrl(url, out uri);
			if(extension != null) return extension.GetLastWriteTime(uri);
			
			throw new NotImplementedException();
		}
		
		public long GetLength(Uri url)
		{
			return new DataUri(url, false, true).Data.LongLength;
		}
		
		public Stream GetStream(Uri url, FileMode mode, FileAccess access)
		{
			return new MemoryStream(new DataUri(url, false, true).Data, false);
		}
		
		public Uri GetTarget(Uri url)
		{
			DataUri uri;
			var extension = ParseUrl(url, out uri);
			if(extension != null) return extension.GetTarget(uri);
			
			return url;
		}
		
		public string GetContentType(Uri url)
		{
			return new DataUri(url, false, false).ContentType;
		}
		
		public struct DataUri
		{
			public string ContentType{get; private set;}
			public byte[] Data{get; private set;}
			public Dictionary<string, string> Parameters{get; private set;}
			
			static readonly Regex dataUri = new Regex(@"^(?<type>[a-z\-]+\/[a-z\-]+)?(?:;(?<parameters>.+?=.+?))*(?<base64>;base64)?,", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			public DataUri(Uri url, bool parseParameters, bool parseData) : this()
			{
				string path = url.AbsolutePath;
				var match = dataUri.Match(path);
				
				ContentType = match.Groups["type"].Value;
				if(String.IsNullOrEmpty(ContentType)) ContentType = "text/plain";
				
				if(parseParameters)
				{
					Parameters = new Dictionary<string, string>();
					
					foreach(Capture capture in match.Groups["parameters"].Captures)
					{
						var split = capture.Value.Split(new[]{'='}, 2);
						Parameters[split[0]] = HttpUtility.UrlDecode(split[1]);
					}
				}
				
				if(parseData)
				{
					bool base64 = match.Groups["base64"].Success;
					
					string data = path.Substring(match.Index+match.Length);
					if(base64)
					{
						Data = Convert.FromBase64String(HttpUtility.UrlDecode(data));
					}else{
						Data = HttpUtility.UrlDecodeToBytes(data);
					}
				}
			}
		}
	}
}
