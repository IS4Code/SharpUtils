/* Date: 5.9.2017, Time: 0:19 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using IllidanS4.SharpUtils.IO.FileSystems.DataExtensions;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	/// <summary>
	/// This file system handles URI using the "data" scheme.
	/// </summary>
	public partial class DataFileSystem : IExtensionHost, IHandleProvider
	{
		public static readonly DataFileSystem Instance = new DataFileSystem();
		
		public DataFileSystem()
		{
			Register(new ShellLinkDataExtension());
			Register(new ShellItemIdListDataExtension());
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
		
		public ResourceHandle ObtainHandle(Uri uri)
		{
			return new DataFileHandle(new DataUri(uri, true, true), this);
		}
		
		public T GetProperty<T>(Uri uri, ResourceProperty property)
		{
			DataUri dataUri;
			var extension = ParseUri(uri, out dataUri);
			if(extension != null)
			{
				try{
					return extension.GetProperty<T>(dataUri, property);
				}catch(NotSupportedException)
				{
					
				}catch(NotImplementedException)
				{
					
				}
			}
			
			switch(property)
			{
				case ResourceProperty.FileAttributes:
					return To<T>.Cast(FileAttributes.ReadOnly);
				/*case ResourceProperty.CreationTimeUtc:
					break;
				case ResourceProperty.LastAccessTimeUtc:
					break;
				case ResourceProperty.LastWriteTimeUtc:
					break;*/
				case ResourceProperty.LongLength:
					return To<T>.Cast(dataUri.Data.LongLength);
				case ResourceProperty.TargetUri:
					return To<T>.Cast((Uri)null);
				case ResourceProperty.ContentType:
					return To<T>.Cast(dataUri.ContentType);
				/*case ResourceProperty.LocalPath:
					break;
				case ResourceProperty.DisplayPath:
					break;*/
				default:
					throw new NotImplementedException();
			}
		}
		
		public Stream GetStream(Uri uri, FileMode mode, FileAccess access)
		{
			return new MemoryStream(new DataUri(uri, false, true).Data, false);
		}
		
		public List<Uri> GetResources(Uri uri)
		{
			throw new NotImplementedException();
		}
		
		public ResourceHandle PerformOperation(Uri uri, ResourceOperation operation, object arg)
		{
			throw new NotImplementedException();
		}
		
		public Task<ResourceHandle> PerformOperationAsync(Uri uri, ResourceOperation operation, object arg, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
		
		public struct DataUri
		{
			public Uri Uri{get; private set;}
			public string ContentType{get; private set;}
			public byte[] Data{get; private set;}
			public Dictionary<string, string> Parameters{get; private set;}
			
			static readonly Regex dataUri = new Regex(@"^(?<type>[a-z\-]+\/[a-z\-]+)?(?:;(?<parameters>.+?=.+?))*(?<base64>;base64)?,", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			public DataUri(Uri uri, bool parseParameters, bool parseData) : this()
			{
				Uri = uri;
				
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
			
			public DataUri(string contentType, byte[] data, bool base64) : this()
			{
				ContentType = contentType;
				Data = data;
				
				string dataString;
				if(base64)
				{
					dataString = Convert.ToBase64String(data);
					
					Uri = new Uri("data:"+contentType+";base64,"+dataString);
				}else{
					dataString = HttpUtility.UrlEncode(data);
					Uri = new Uri("data:"+contentType+","+dataString);
				}
			}
		}
	}
}
