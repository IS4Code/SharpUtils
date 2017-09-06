/* Date: 3.9.2017, Time: 4:03 */
using System;
using System.IO;
using IllidanS4.SharpUtils.IO.FileSystems;

namespace IllidanS4.SharpUtils.IO
{
	[Serializable]
	public partial class ResourceInfo
	{
		readonly Uri url;
		readonly IFileSystem fileSystem;
		
		public Uri Url{get{return url;}}
		
		public ResourceInfo(string path) : this(Win32FileSystem.UrlOrPath(path))
		{
			
		}
		
		public ResourceInfo(Uri url)
		{
			if(url == null) throw new ArgumentNullException("url");
			
			this.url = url;
			fileSystem = GetFileSystem(url.Scheme);
		}
		
		static IFileSystem GetFileSystem(string scheme)
		{
			switch(scheme)
			{
				case "file":
					return Win32FileSystem.Instance;
				case "http":
				case "https":
				case "ftp":
					return WebFileSystem.Instance;
				case "data":
					return DataFileSystem.Instance;
				case "shell":
					return ShellFileSystem.Instance;
				default:
					throw new NotImplementedException();
			}
		}
		
		public string Scheme{
			get{
				return url.Scheme;
			}
		}
		
		public string Host{
			get{
				return url.Host;
			}
		}
		
		public string AbsolutePath{
			get{
				return url.AbsolutePath;
			}
		}
		
		public string AbsoluteUri{
			get{
				return url.AbsoluteUri;
			}
		}
		
		public FileAttributes Attributes{
			get{
				return fileSystem.GetAttributes(url);
			}
		}
		
		public DateTime CreationTime{
			get{
				return CreationTimeUtc.ToLocalTime();
			}
		}
		public DateTime CreationTimeUtc{
			get{
				return fileSystem.GetCreationTime(url);
			}
		}
		public DateTime LastAccessTime{
			get{
				return LastAccessTimeUtc.ToLocalTime();
			}
		}
		public DateTime LastAccessTimeUtc{
			get{
				return fileSystem.GetLastAccessTime(url);
			}
		}
		public DateTime LastWriteTime{
			get{
				return LastWriteTimeUtc.ToLocalTime();
			}
		}
		public DateTime LastWriteTimeUtc{
			get{
				return fileSystem.GetLastWriteTime(url);
			}
		}
		
		public long Length{
			get{
				return fileSystem.GetLength(url);
			}
		}
		
		public Stream GetStream(FileMode mode, FileAccess access)
		{
			return fileSystem.GetStream(url, mode, access);
		}
		
		public ResourceInfo Target{
			get{
				return new ResourceInfo(fileSystem.GetTarget(url));
			}
		}
		
		public string ContentType{
			get{
				return fileSystem.GetContentType(url);
			}
		}
		
		public ResourceInfo Parent{
			get{
				if(url.AbsolutePath.EndsWith("/"))
				{
					return new ResourceInfo(new Uri(url, ".."));
				}else{
					return new ResourceInfo(new Uri(url, "."));
				}
			}
		}
		
		public override string ToString()
		{
			return url.AbsoluteUri;
		}
	}
}
