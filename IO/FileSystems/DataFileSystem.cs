/* Date: 5.9.2017, Time: 0:19 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	/// <summary>
	/// This file system handles URI using the "data" scheme.
	/// </summary>
	public class DataFileSystem : IFileSystem
	{
		public static readonly DataFileSystem Instance = new DataFileSystem();
		
		public FileAttributes GetAttributes(Uri url)
		{
			return FileAttributes.ReadOnly;
		}
		
		public DateTime GetCreationTime(Uri url)
		{
			throw new NotImplementedException();
		}
		
		public DateTime GetLastAccessTime(Uri url)
		{
			throw new NotImplementedException();
		}
		
		public DateTime GetLastWriteTime(Uri url)
		{
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
