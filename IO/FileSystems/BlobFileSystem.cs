/* Date: 9.9.2017, Time: 2:20 */
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	public class BlobFileSystem : IFileSystem
	{
		public static readonly BlobFileSystem Instance = new BlobFileSystem();
		
		Dictionary<string, BlobInfo> blobs = new Dictionary<string, BlobInfo>();
		
		private BlobInfo GetBlob(string name, bool create=false, bool createNew=false)
		{
			BlobInfo blob;
			if(!blobs.TryGetValue(name, out blob))
			{
				if(!create) throw new FileNotFoundException();
				blobs[name] = blob = new BlobInfo(name);
				blob.CreationTime = DateTime.UtcNow;
			}else{
				if(createNew) throw new IOException();
			}
			blob.LastAccessTime = DateTime.UtcNow;
			return blob;
		}
		
		public BlobFileSystem()
		{
			
		}
		
		private class BlobInfo
		{
			public string Name{get; private set;}
			
			public DateTime CreationTime{get; set;}
			public DateTime LastAccessTime{get; set;}
			public DateTime LastWriteTime{get; set;}
			
			private byte[] data;
			private long length;
			private bool locked;
			
			//private PipeServer pipe;
			
			public BlobInfo(string name)
			{
				Name = name;
				data = new byte[0];
				
				/*pipe = new PipeServer();
				pipe.PipeName = @"blob\"+name;
				pipe.PipeDirection = PipeDirection.InOut;
				pipe.PipeOpened += PipeOpened;
				pipe.Start();*/
			}
			
			/*private async void PipeOpened(object sender, NamedPipeEventArgs e)
			{
				var stream = e.Stream;
				byte[] buffer = new byte[4096];
				
				stream.Write(data, 0, data.Length);
				stream.WaitForPipeDrain();
				
				
			}*/
			
			public long Length{
				get{
					return length;
				}
			}
			
			[MethodImpl(MethodImplOptions.Synchronized)]
			public MemoryStream ObtainStream(bool writable)
			{
				if(writable)
				{
					if(locked)
					{
						return null;
					}
					return new BlobStream(this);
				}else{
					return new MemoryStream(data, false);
				}
			}
			
			[MethodImpl(MethodImplOptions.Synchronized)]
			private void UpdateData(byte[] data, long length)
			{
				this.data = data;
				this.length = length;
			}
			
			private class BlobStream : MemoryStream
			{
				private readonly BlobInfo blob;
				
				public override int Capacity{
					get{
						return base.Capacity;
					}
					set{
						int old = base.Capacity;
						base.Capacity = value;
						
						if(old != value)
						{
							blob.UpdateData(this.GetBuffer(), this.Length);
						}
					}
				}
				
				public BlobStream(BlobInfo blob) : base((int)blob.length)
				{
					this.blob = blob;
					blob.locked = true;
					this.Write(blob.data, 0, (int)blob.length);
				}
				
				protected override void Dispose(bool disposing)
				{
					blob.UpdateData(this.ToArray(), this.Length);
					blob.locked = false;
					
					base.Dispose(disposing);
				}
				
				public override void SetLength(long value)
				{
					base.SetLength(value);
					
					blob.UpdateData(this.GetBuffer(), this.Length);
				}
				
				public override void Write(byte[] buffer, int offset, int count)
				{
					base.Write(buffer, offset, count);
					blob.UpdateData(this.GetBuffer(), this.Length);
					blob.LastWriteTime = DateTime.UtcNow;
				}
				
				public override async System.Threading.Tasks.Task WriteAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
				{
					await base.WriteAsync(buffer, offset, count, cancellationToken);
					blob.UpdateData(this.GetBuffer(), this.Length);
					blob.LastWriteTime = DateTime.UtcNow;
				}
				
				public override void WriteByte(byte value)
				{
					base.WriteByte(value);
					blob.UpdateData(this.GetBuffer(), this.Length);
					blob.LastWriteTime = DateTime.UtcNow;
				}
			}
		}
		
		public T GetProperty<T>(Uri uri, ResourceProperty property)
		{
			BlobInfo blob;
			switch(property)
			{
				case ResourceProperty.FileAttributes:
					return To<T>.Cast(FileAttributes.Normal);
				case ResourceProperty.CreationTimeUtc:
					blob = GetBlob(uri.AbsolutePath);
					return To<T>.Cast(blob.CreationTime);
				case ResourceProperty.LastAccessTimeUtc:
					blob = GetBlob(uri.AbsolutePath);
					return To<T>.Cast(blob.LastAccessTime);
				case ResourceProperty.LastWriteTimeUtc:
					blob = GetBlob(uri.AbsolutePath);
					return To<T>.Cast(blob.LastWriteTime);
				case ResourceProperty.LongLength:
					blob = GetBlob(uri.AbsolutePath);
					return To<T>.Cast(blob.Length);
				case ResourceProperty.TargetUri:
					return To<T>.Cast((Uri)null);
				/*case ResourceProperty.ContentType:
					break;
				case ResourceProperty.LocalPath:
					break;
				case ResourceProperty.DisplayPath:
					break;*/
				default:
					throw new NotImplementedException();
			}
		}
		
		public void SetProperty<T>(Uri uri, ResourceProperty property, T value)
		{
			throw new NotImplementedException();
		}
		
		public Stream GetStream(Uri uri, FileMode mode, FileAccess access)
		{
			bool create = false;
			switch(mode)
			{
				case FileMode.Append:
				case FileMode.Create:
				case FileMode.CreateNew:
				case FileMode.OpenOrCreate:
					create = true;
					break;
			}
			var blob = GetBlob(uri.AbsolutePath, create, mode == FileMode.CreateNew);
			if(access == FileAccess.Read)
			{
				return blob.ObtainStream(false);
			}else{
				return blob.ObtainStream(true);
			}
		}
		
		List<Uri> IFileSystem.GetResources(Uri uri)
		{
			throw new NotImplementedException();
		}
		
		ResourceHandle IFileSystem.PerformOperation(Uri uri, ResourceOperation operation, object arg)
		{
			throw new NotImplementedException();
		}
		
		Task<ResourceHandle> IFileSystem.PerformOperationAsync(Uri uri, ResourceOperation operation, object arg, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
		
		System.Diagnostics.Process IFileSystem.Execute(Uri uri)
		{
			throw new NotImplementedException();
		}
	}
}
