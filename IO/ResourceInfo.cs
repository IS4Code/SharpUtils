/* Date: 3.9.2017, Time: 4:03 */
using System;
using System.IO;
using IllidanS4.SharpUtils.IO.FileSystems;

namespace IllidanS4.SharpUtils.IO
{
	/// <summary>
	/// Holds information about a resource in the universal file system, represented by its URL.
	/// </summary>
	/// <remarks>
	/// A resource is represented by its URL in a specified file system, which is attached to
	/// each instance of this type.
	/// </remarks>
	[Serializable]
	public partial class ResourceInfo
	{
		readonly Uri url;
		readonly IFileSystem fileSystem;
		
		/// <summary>
		/// The URL of this resource.
		/// </summary>
		public Uri Url{get{return url;}}
		
		/// <summary>
		/// Constructs a resource info from a string represeting either a system path, or a valid URL.
		/// </summary>
		/// <param name="path">A valid Windows file system path or a URL.</param>
		public ResourceInfo(string pathOrUrl) : this(Win32FileSystem.UrlOrPath(pathOrUrl))
		{
			
		}
		
		/// <summary>
		/// Constructs a resource info from its URL.
		/// </summary>
		/// <param name="url">The valid URL representing the resource.</param>
		/// <remarks>
		/// Supported URI schemes include "file", "http", "https", "ftp", "data", and "shell".
		/// </remarks>
		public ResourceInfo(Uri url)
		{
			if(url == null) throw new ArgumentNullException("url");
			
			this.url = url;
			fileSystem = GetFileSystem(url.Scheme);
		}
		
		/// <summary>
		/// Obtains the specific file system representing a specific URI scheme or namespace.
		/// </summary>
		/// <param name="scheme">The scheme of the file system.</param>
		/// <returns>The file system representing resources with in a specific namespace.</returns>
		public static IFileSystem GetFileSystem(string scheme)
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
		
		/// <summary>
		/// Obtains the scheme or the namespace of the resource.
		/// </summary>
		public string Scheme{
			get{
				return url.Scheme;
			}
		}
		
		/// <summary>
		/// Obtains the host server containing the resource. May be empty.
		/// </summary>
		public string Host{
			get{
				return url.Host;
			}
		}
		
		/// <summary>
		/// Obtains the absolute path in the specified file system.
		/// </summary>
		public string AbsolutePath{
			get{
				return url.AbsolutePath;
			}
		}
		
		/// <summary>
		/// Obtains the absolute URI of the resource, as a URI-encoded string.
		/// </summary>
		public string AbsoluteUri{
			get{
				return url.AbsoluteUri;
			}
		}
		
		/// <summary>
		/// Obtains the attribute flags of this resource.
		/// </summary>
		public FileAttributes Attributes{
			get{
				return fileSystem.GetAttributes(url);
			}
		}
		
		/// <summary>
		/// Obtains the creation time of the resource.
		/// </summary>
		public DateTime CreationTime{
			get{
				return CreationTimeUtc.ToLocalTime();
			}
		}
		
		/// <summary>
		/// Obtains the creation time of the resource, in coordinated universal time (UTC).
		/// </summary>
		public DateTime CreationTimeUtc{
			get{
				return fileSystem.GetCreationTime(url);
			}
		}
		
		/// <summary>
		/// Obtains the last access time of the resource.
		/// </summary>
		public DateTime LastAccessTime{
			get{
				return LastAccessTimeUtc.ToLocalTime();
			}
		}
		
		/// <summary>
		/// Obtains the last access time of the resource, in coordinated universal time (UTC).
		/// </summary>
		public DateTime LastAccessTimeUtc{
			get{
				return fileSystem.GetLastAccessTime(url);
			}
		}
		
		/// <summary>
		/// Obtains the last write time of the resource.
		/// </summary>
		public DateTime LastWriteTime{
			get{
				return LastWriteTimeUtc.ToLocalTime();
			}
		}
		
		/// <summary>
		/// Obtains the last write time of the resource, in coordinated universal time (UTC).
		/// </summary>
		public DateTime LastWriteTimeUtc{
			get{
				return fileSystem.GetLastWriteTime(url);
			}
		}
		
		/// <summary>
		/// Obtains the length of the resource's data.
		/// </summary>
		public long Length{
			get{
				return fileSystem.GetLength(url);
			}
		}
		
		/// <summary>
		/// Creates a new stream to the resource.
		/// </summary>
		/// <param name="mode">The opening mode of the stream.</param>
		/// <param name="access">The access flags of the stream.</param>
		/// <returns>The newly created stream to the resource.</returns>
		public Stream GetStream(FileMode mode, FileAccess access)
		{
			return fileSystem.GetStream(url, mode, access);
		}
		
		/// <summary>
		/// If this resource represents a link in the destination
		/// file system, obtains the target of the link.
		/// Otherwise returns the resource info representing the canonical
		/// path in the file system.
		/// </summary>
		public ResourceInfo Target{
			get{
				return new ResourceInfo(fileSystem.GetTarget(url));
			}
		}
		
		/// <summary>
		/// Obtains the content type of the resource, in MIME format.
		/// </summary>
		public string ContentType{
			get{
				return fileSystem.GetContentType(url);
			}
		}
		
		/// <summary>
		/// Constructs a resource info from the parent URL.
		/// </summary>
		/// <remarks>
		/// This may not represent an actual parent of the resource.
		/// </remarks>
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
		
		/// <summary>
		/// Returns the string representation of this resource.
		/// </summary>
		/// <returns>The string representation of this resource's URL.</returns>
		public override string ToString()
		{
			return url.ToString();
		}
	}
}
