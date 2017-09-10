/* Date: 9.9.2017, Time: 2:20 */
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
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
			
			private PipeServer pipe;
			
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
		
		public FileAttributes GetAttributes(Uri uri)
		{
			return FileAttributes.Normal;
		}
		
		public DateTime GetCreationTime(Uri uri)
		{
			var blob = GetBlob(uri.AbsolutePath);
			return blob.CreationTime;
		}
		
		public DateTime GetLastAccessTime(Uri uri)
		{
			var blob = GetBlob(uri.AbsolutePath);
			return blob.LastAccessTime;
		}
		
		public DateTime GetLastWriteTime(Uri uri)
		{
			var blob = GetBlob(uri.AbsolutePath);
			return blob.LastWriteTime;
		}
		
		public long GetLength(Uri uri)
		{
			var blob = GetBlob(uri.AbsolutePath);
			return blob.Length;
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
		
		public Uri GetTarget(Uri uri)
		{
			return uri;
		}
		
		public string GetContentType(Uri uri)
		{
			throw new NotImplementedException();
		}
		
		public List<Uri> GetResources(Uri uri)
		{
			throw new NotImplementedException();
		}
	}
}
