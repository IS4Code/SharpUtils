/* Date: 3.9.2017, Time: 4:03 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
		readonly Uri _uri;
		readonly IFileSystem fileSystem;
		
		/// <summary>
		/// The URI of this resource.
		/// </summary>
		public virtual Uri Uri{get{return _uri;}}
		
		internal IFileSystem FileSystem{get{return fileSystem;}}
		
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
			
			this._uri = uri;
			fileSystem = GetFileSystem(uri.Scheme);
		}
		
		protected ResourceInfo(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
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
				return Uri.Scheme;
			}
		}
		
		/// <summary>
		/// Obtains the host server containing the resource. May be empty.
		/// </summary>
		public string Host{
			get{
				return Uri.Host;
			}
		}
		
		/// <summary>
		/// Obtains the absolute path in the specified file system.
		/// </summary>
		public string AbsolutePath{
			get{
				return Uri.AbsolutePath;
			}
		}
		
		/// <summary>
		/// Obtains the absolute URI of the resource, as a URI-encoded string.
		/// </summary>
		public string AbsoluteUri{
			get{
				return Uri.AbsoluteUri;
			}
		}
		
		/// <summary>
		/// Obtains the attribute flags of this resource.
		/// </summary>
		public virtual FileAttributes Attributes{
			get{
				return fileSystem.GetAttributes(Uri);
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
		public virtual DateTime CreationTimeUtc{
			get{
				return fileSystem.GetCreationTime(Uri);
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
		public virtual DateTime LastAccessTimeUtc{
			get{
				return fileSystem.GetLastAccessTime(Uri);
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
		public virtual DateTime LastWriteTimeUtc{
			get{
				return fileSystem.GetLastWriteTime(Uri);
			}
		}
		
		/// <summary>
		/// Obtains the length of the resource's data.
		/// </summary>
		public virtual long Length{
			get{
				return fileSystem.GetLength(Uri);
			}
		}
		
		/// <summary>
		/// Creates a new stream to the resource.
		/// </summary>
		/// <param name="mode">The opening mode of the stream.</param>
		/// <param name="access">The access flags of the stream.</param>
		/// <returns>The newly created stream to the resource.</returns>
		public virtual Stream GetStream(FileMode mode, FileAccess access)
		{
			return fileSystem.GetStream(Uri, mode, access);
		}
		
		/// <summary>
		/// If this resource represents a link in the destination
		/// file system, obtains the target of the link.
		/// Otherwise returns the resource info representing the canonical
		/// path in the file system.
		/// </summary>
		public virtual ResourceInfo Target{
			get{
				Uri target = fileSystem.GetTarget(Uri);
				if(target == null) return null;
				return new ResourceInfo(target);
			}
		}
		
		/// <summary>
		/// Obtains the content type of the resource, in MIME format.
		/// </summary>
		public virtual string ContentType{
			get{
				return fileSystem.GetContentType(Uri);
			}
		}
		
		/// <summary>
		/// Gets the path of the resource in its files system.
		/// </summary>
		public virtual string LocalPath{
			get{
				return fileSystem.GetLocalPath(Uri);
			}
		}
		
		/// <summary>
		/// Gets the path of the resource, suitable for display.
		/// </summary>
		public virtual string DisplayPath{
			get{
				return fileSystem.GetDisplayPath(Uri);
			}
		}
		
		public virtual List<ResourceInfo> GetResources()
		{
			return fileSystem.GetResources(Uri).Select(u => new ResourceInfo(u)).ToList();
		}
		
		/// <summary>
		/// Constructs a resource info from the parent URI.
		/// </summary>
		/// <remarks>
		/// This may not represent an actual parent of the resource.
		/// </remarks>
		public virtual ResourceInfo Parent{
			get{
				if(Uri.AbsolutePath.EndsWith("/"))
				{
					return new ResourceInfo(new Uri(Uri, ".."));
				}else{
					return new ResourceInfo(new Uri(Uri, "."));
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
		
		public Task<ResourceHandle> Rename(string newName)
		{
			return fileSystem.PerformOperationAsync(Uri, ResourceOperation.Move, newName);
		}
		
		public Task<ResourceHandle> Move(Uri newUri)
		{
			return fileSystem.PerformOperationAsync(Uri, ResourceOperation.Move, newUri);
		}
		
		public Task<ResourceHandle> Copy(Uri newUri)
		{
			return fileSystem.PerformOperationAsync(Uri, ResourceOperation.Copy, newUri);
		}
		
		public Task<ResourceHandle> Delete()
		{
			return fileSystem.PerformOperationAsync(Uri, ResourceOperation.Delete, null);
		}
		
		public Task<ResourceHandle> ChangeAttributes(FileAttributes newAttributes)
		{
			return fileSystem.PerformOperationAsync(Uri, ResourceOperation.ChangeAttributes, newAttributes);
		}
		
		public Task<ResourceHandle> Create(FileAttributes attributes)
		{
			return fileSystem.PerformOperationAsync(Uri, ResourceOperation.Create, attributes);
		}
	}
}
