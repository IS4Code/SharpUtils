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
		
		private IDataExtension ParseUri(Uri uri, out DataUri dataUri)
		{
			dataUri = new DataUri(uri, false, false);
			var ext = GetExtension(dataUri.ContentType);
			if(ext != null)
			{
				dataUri = new DataUri(uri, true, true);
				return ext;
			}
			return null;
		}
		
		public FileAttributes GetAttributes(Uri uri)
		{
			DataUri dataUri;
			var extension = ParseUri(uri, out dataUri);
			if(extension != null) return extension.GetAttributes(dataUri);
			
			return FileAttributes.ReadOnly;
		}
		
		public DateTime GetCreationTime(Uri uri)
		{
			DataUri dataUri;
			var extension = ParseUri(uri, out dataUri);
			if(extension != null) return extension.GetCreationTime(dataUri);
			
			throw new NotImplementedException();
		}
		
		public DateTime GetLastAccessTime(Uri uri)
		{
			DataUri dataUri;
			var extension = ParseUri(uri, out dataUri);
			if(extension != null) return extension.GetLastAccessTime(dataUri);
			
			throw new NotImplementedException();
		}
		
		public DateTime GetLastWriteTime(Uri uri)
		{
			DataUri dataUri;
			var extension = ParseUri(uri, out dataUri);
			if(extension != null) return extension.GetLastWriteTime(dataUri);
			
			throw new NotImplementedException();
		}
		
		public long GetLength(Uri uri)
		{
			return new DataUri(uri, false, true).Data.LongLength;
		}
		
		public Stream GetStream(Uri uri, FileMode mode, FileAccess access)
		{
			return new MemoryStream(new DataUri(uri, false, true).Data, false);
		}
		
		public Uri GetTarget(Uri uri)
		{
			DataUri dataUri;
			var extension = ParseUri(uri, out dataUri);
			if(extension != null) return extension.GetTarget(dataUri);
			
			return uri;
		}
		
		public string GetContentType(Uri uri)
		{
			return new DataUri(uri, false, false).ContentType;
		}
		
		public struct DataUri
		{
			public string ContentType{get; private set;}
			public byte[] Data{get; private set;}
			public Dictionary<string, string> Parameters{get; private set;}
			
			static readonly Regex dataUri = new Regex(@"^(?<type>[a-z\-]+\/[a-z\-]+)?(?:;(?<parameters>.+?=.+?))*(?<base64>;base64)?,", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			public DataUri(Uri uri, bool parseParameters, bool parseData) : this()
			{
				string path = uri.AbsolutePath;
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
