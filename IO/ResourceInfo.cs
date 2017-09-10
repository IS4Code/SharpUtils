/* Date: 3.9.2017, Time: 4:03 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using IllidanS4.SharpUtils.IO.FileSystems;

namespace IllidanS4.SharpUtils.IO
{
	/// <summary>
	/// Holds information about a resource in the universal file system, represented by its URI.
	/// </summary>
	/// <remarks>
	/// A resource is represented by its URI in a specified file system, which is attached to
	/// each instance of this type.
	/// </remarks>
	[Serializable]
	public partial class ResourceInfo
	{
		readonly Uri uri;
		readonly IFileSystem fileSystem;
		
		/// <summary>
		/// The URI of this resource.
		/// </summary>
		public Uri Uri{get{return uri;}}
		
		/// <summary>
		/// Constructs a resource info from a string represeting either a system path, or a valid URI.
		/// </summary>
		/// <param name="path">A valid Windows file system path or a URI.</param>
		public ResourceInfo(string pathOrUri) : this(Win32FileSystem.Instance.UriOrPath(pathOrUri))
		{
			
		}
		
		/// <summary>
		/// Constructs a resource info from its URI.
		/// </summary>
		/// <param name="uri">The valid URI representing the resource.</param>
		/// <remarks>
		/// Supported URI schemes include "file", "http", "https", "ftp", "data", and "shell".
		/// </remarks>
		public ResourceInfo(Uri uri)
		{
			if(uri == null) throw new ArgumentNullException("uri");
			
			this.uri = uri;
			fileSystem = GetFileSystem(uri.Scheme);
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
				case "pipe":
					return PipeFileSystem.Instance;
				case "blob":
					return BlobFileSystem.Instance;
				default:
					throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// Obtains the scheme or the namespace of the resource.
		/// </summary>
		public string Scheme{
			get{
				return uri.Scheme;
			}
		}
		
		/// <summary>
		/// Obtains the host server containing the resource. May be empty.
		/// </summary>
		public string Host{
			get{
				return uri.Host;
			}
		}
		
		/// <summary>
		/// Obtains the absolute path in the specified file system.
		/// </summary>
		public string AbsolutePath{
			get{
				return uri.AbsolutePath;
			}
		}
		
		/// <summary>
		/// Obtains the absolute URI of the resource, as a URI-encoded string.
		/// </summary>
		public string AbsoluteUri{
			get{
				return uri.AbsoluteUri;
			}
		}
		
		/// <summary>
		/// Obtains the attribute flags of this resource.
		/// </summary>
		public FileAttributes Attributes{
			get{
				return fileSystem.GetAttributes(uri);
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
				return fileSystem.GetCreationTime(uri);
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
				return fileSystem.GetLastAccessTime(uri);
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
				return fileSystem.GetLastWriteTime(uri);
			}
		}
		
		/// <summary>
		/// Obtains the length of the resource's data.
		/// </summary>
		public long Length{
			get{
				return fileSystem.GetLength(uri);
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
			return fileSystem.GetStream(uri, mode, access);
		}
		
		/// <summary>
		/// If this resource represents a link in the destination
		/// file system, obtains the target of the link.
		/// Otherwise returns the resource info representing the canonical
		/// path in the file system.
		/// </summary>
		public ResourceInfo Target{
			get{
				return new ResourceInfo(fileSystem.GetTarget(uri));
			}
		}
		
		/// <summary>
		/// Obtains the content type of the resource, in MIME format.
		/// </summary>
		public string ContentType{
			get{
				return fileSystem.GetContentType(uri);
			}
		}
		
		public List<ResourceInfo> GetResources()
		{
			return fileSystem.GetResources(uri).Select(u => new ResourceInfo(u)).ToList();
		}
		
		/// <summary>
		/// Constructs a resource info from the parent URI.
		/// </summary>
		/// <remarks>
		/// This may not represent an actual parent of the resource.
		/// </remarks>
		public ResourceInfo Parent{
			get{
				if(uri.AbsolutePath.EndsWith("/"))
				{
					return new ResourceInfo(new Uri(uri, ".."));
				}else{
					return new ResourceInfo(new Uri(uri, "."));
				}
			}
		}
		
		/// <summary>
		/// Returns the string representation of this resource.
		/// </summary>
		/// <returns>The string representation of this resource's URI.</returns>
		public override string ToString()
		{
			return HttpUtility.UrlDecode(AbsoluteUri);
		}
	}
}
