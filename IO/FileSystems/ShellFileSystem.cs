/* Date: 5.9.2017, Time: 1:58 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using IllidanS4.SharpUtils.Com;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	/// <summary>
	/// This file system contains locations accessible using the Win32 shell model.
	/// The resources in this system may be real files, but also virtual shell items.
	/// </summary>
	public partial class ShellFileSystem : IHandleProvider
	{
		public static readonly ShellFileSystem Instance = new ShellFileSystem(new Uri("shell:"));
		
		private IntPtr OwnerHwnd{
			get{
				return Process.GetCurrentProcess().MainWindowHandle;
			}
		}
		
		private readonly Uri baseUri;
		
		private const SHCONTF EnumConst = SHCONTF.SHCONTF_INCLUDEHIDDEN | SHCONTF.SHCONTF_SHAREABLE | SHCONTF.SHCONTF_STORAGE | SHCONTF.SHCONTF_INCLUDESUPERHIDDEN | SHCONTF.SHCONTF_FOLDERS | SHCONTF.SHCONTF_NONFOLDERS;
		
		public ShellFileSystem(Uri baseUri)
		{
			this.baseUri = baseUri;
		}
		
		#region Public API
		public string GetShellPath(Uri uri)
		{
			var rel = baseUri.MakeRelativeUri(uri);
			if(rel.IsAbsoluteUri) throw new ArgumentException("URI is not within this subsystem.", "uri");
			
			string path = rel.OriginalString.Replace('/', Path.DirectorySeparatorChar);
			
			if(path.StartsWith(@".\")) path = path.Substring(2);
			
			return "shell:"+HttpUtility.UrlDecode(path);
		}
		
		public Uri GetShellUri(string relUri)
		{
			return new Uri(baseUri, relUri);
		}
		
		[CLSCompliant(false)]
		public IShellLink LoadLink(byte[] linkData)
		{
			var data = (IPersistStream)Shell32.CreateShellLink();
			using(var buffer = new MemoryStream(linkData))
			{
				var stream = new StreamWrapper(buffer);
				data.Load(stream);
			}
			var link = (IShellLink)data;
			link.Resolve(OwnerHwnd, SLR_FLAGS.SLR_UPDATE);
			return link;
		}
		
		[CLSCompliant(false)]
		public IShellLink LoadLink(string linkFile)
		{
			var data = (IPersistFile)Shell32.CreateShellLink();
			data.Load(linkFile, 0);
			var link = (IShellLink)data;
			link.Resolve(OwnerHwnd, SLR_FLAGS.SLR_UPDATE);
			return link;
		}
		
		[CLSCompliant(false)]
		public Uri GetLinkTargetUri(IShellLink link)
		{
			var target = GetLinkTarget(link);
			try{
				IntPtr pidl = Shell32.SHGetIDListFromObject(target);
				return GetShellUri(pidl, true);
			}finally{
				Marshal.FinalReleaseComObject(target);
			}
		}
		
		[CLSCompliant(false)]
		public ResourceHandle GetLinkTargetResource(IShellLink link)
		{
			var target = GetLinkTarget(link);
			try{
				IntPtr pidl = Shell32.SHGetIDListFromObject(target);
				try{
					return new ShellFileHandle(pidl, this);
				}finally{
					Marshal.FreeCoTaskMem(pidl);
				}
			}finally{
				Marshal.FinalReleaseComObject(target);
			}
		}
		
		public Uri LoadLinkTargetUri(byte[] linkData)
		{
			var link = LoadLink(linkData);
			try{
				return GetLinkTargetUri(link);
			}finally{
				Marshal.FinalReleaseComObject(link);
			}
		}
		
		public Uri LoadLinkTargetUri(string linkFile)
		{
			var link = LoadLink(linkFile);
			try{
				return GetLinkTargetUri(link);
			}finally{
				Marshal.FinalReleaseComObject(link);
			}
		}
		
		public ResourceHandle LoadLinkTargetResource(byte[] linkData)
		{
			var link = LoadLink(linkData);
			try{
				return GetLinkTargetResource(link);
			}finally{
				Marshal.FinalReleaseComObject(link);
			}
		}
		
		public ResourceHandle LoadLinkTargetResource(string linkFile)
		{
			var link = LoadLink(linkFile);
			try{
				return GetLinkTargetResource(link);
			}finally{
				Marshal.FinalReleaseComObject(link);
			}
		}
		
		public Uri LoadShellHandleUri(byte[] itemList)
		{
			using(var handle = new ShellFileHandle(itemList, this))
			{
				return handle.Uri;
			}
		}
		
		public ResourceHandle LoadShellHandle(byte[] itemList)
		{
			return new ShellFileHandle(itemList, this);
		}
		
		public byte[] SaveShellHandle(ResourceHandle handle)
		{
			return ((ShellFileHandle)handle).SaveIdList();
		}
		
		[CLSCompliant(false)]
		public IShellItem GetLinkTarget(IShellLink link)
		{
			link.Resolve(OwnerHwnd, SLR_FLAGS.SLR_UPDATE);
			IntPtr pidl = link.GetIDList();
			try{
				return Shell32.SHCreateItemFromIDList<IShellItem>(pidl);
			}finally{
				Marshal.FreeCoTaskMem(pidl);
			}
		}
		#endregion
		
		static readonly Regex pathNameRegex = new Regex(@"^(shell:.*?\\?)([^\\]*)$", RegexOptions.Compiled);
		private IShellItem GetItem(Uri uri)
		{
			var rel = baseUri.MakeRelativeUri(uri);
			if(rel.IsAbsoluteUri) throw new ArgumentException("URI is not within this subsystem.", "uri");
			if(String.IsNullOrEmpty(rel.OriginalString)) return GetDesktop();
			var segments = rel.OriginalString.Split('/').Where(s => s != "." && s != "").Select(s => HttpUtility.UrlDecode(s)).ToList();
			
			if(segments.Count == 0) return GetDesktop();
			
			IShellItem item = null;
			foreach(var name in segments)
			{
				if(item == null)
				{
					try{
						item = Shell32.SHCreateItemFromParsingName<IShellItem>("shell:"+name, null);
						continue;
					}catch(IOException)
					{
						try{
							item = Shell32.SHCreateItemFromParsingName<IShellItem>(name, null);
							continue;
						}catch(IOException)
						{
							item = GetDesktop();
						}
					}
				}
				var psf = item.BindToHandler<IShellFolder>(null, Shell32.BHID_SFObject);
				uint tmp;
				IntPtr pidl;
				psf.ParseDisplayName(OwnerHwnd, null, name, out tmp, out pidl, 0);
				try{
					item = Shell32.SHCreateItemWithParent<IShellItem>(IntPtr.Zero, psf, pidl);
				}finally{
					Marshal.FreeCoTaskMem(pidl);
				}
			}
			
			return item;
		}
		
		private IShellItem GetDesktop()
		{
			return Shell32.SHGetKnownFolderItem<IShellItem>(Shell32.FOLDERID_Desktop, 0, IntPtr.Zero);
		}
		
		private Uri GetShellUri(IntPtr pidl, bool free)
		{
			Stack<string> list;
			IntPtr pidlrel = free ? pidl : Shell32.ILClone(pidl);
			try{
				list = new Stack<string>();
				
				while(true)
				{
					string name = Shell32.SHGetNameFromIDList(pidlrel, SIGDN.SIGDN_PARENTRELATIVEPARSING);
					if(!Shell32.ILRemoveLastID(pidlrel)) break; //TODO check if really desktop
					list.Push(name);
				}
			}finally{
				Marshal.FreeCoTaskMem(pidlrel);
			}
			
			string path = String.Join("/", list.Select(n => HttpUtility.UrlEncode(n)));
			
			return GetShellUri(path);
		}
		
		private Uri ConstructDataLink(IntPtr pidl)
		{
			using(var buffer = new MemoryStream())
			{
				Shell32.ILSaveToStream(new StreamWrapper(buffer), pidl);
				byte[] data = buffer.ToArray();
				var uri = new DataFileSystem.DataUri("application/x-ms-itemidlist", data, true);
				return uri.Uri;
			}
		}
		
		#region Implementation
		public ResourceHandle ObtainHandle(Uri uri)
		{
			var item = GetItem(uri);
			try{
				IntPtr pidl = Shell32.SHGetIDListFromObject(item);
				try{
					return new ShellFileHandle(pidl, this);
				}finally{
					Marshal.FreeCoTaskMem(pidl);
				}
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public FileAttributes GetAttributes(Uri uri)
		{
			var item = (IShellItem2)GetItem(uri);
			try{
				return (FileAttributes)item.GetUInt32(Shell32.PKEY_FileAttributes);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public DateTime GetCreationTime(Uri uri)
		{
			var item = (IShellItem2)GetItem(uri);
			try{
				FILETIME ft = item.GetFileTime(Shell32.PKEY_DateCreated);
				return Win32FileSystem.GetDateTime(ft);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public DateTime GetLastAccessTime(Uri uri)
		{
			var item = (IShellItem2)GetItem(uri);
			try{
				FILETIME ft = item.GetFileTime(Shell32.PKEY_DateAccessed);
				return Win32FileSystem.GetDateTime(ft);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public DateTime GetLastWriteTime(Uri uri)
		{
			var item = (IShellItem2)GetItem(uri);
			try{
				FILETIME ft = item.GetFileTime(Shell32.PKEY_DateModified);
				return Win32FileSystem.GetDateTime(ft);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public long GetLength(Uri uri)
		{
			var item = (IShellItem2)GetItem(uri);
			try{
				return (long)item.GetUInt64(Shell32.PKEY_Size);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public Stream GetStream(Uri uri, FileMode mode, FileAccess access)
		{
			var item = GetItem(uri);
			try{
				var stream = item.BindToHandler<IStream>(null, Shell32.BHID_Stream);
				return new IStreamWrapper(stream);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public Uri GetTarget(Uri uri)
		{
			var item = GetItem(uri);
			try{
				object targ = GetTargetItem(item);
				if(targ == null) return null;
				string url = targ as string;
				if(url != null)
				{
					return new Uri(url);
				}
				var target = (IShellItem)targ;
				var pidl = Shell32.SHGetIDListFromObject(target);
				return GetShellUri(pidl, true);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public string GetContentType(Uri uri)
		{
			throw new NotImplementedException();
		}
		
		public string GetLocalPath(Uri uri)
		{
			var item = GetItem(uri);
			try{
				return item.GetDisplayName(SIGDN.SIGDN_DESKTOPABSOLUTEPARSING);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public string GetDisplayPath(Uri uri)
		{
			var item = GetItem(uri);
			try{
				return item.GetDisplayName(SIGDN.SIGDN_DESKTOPABSOLUTEEDITING);
			}finally{
				Marshal.FinalReleaseComObject(item);
			}
		}
		
		public List<Uri> GetResources(Uri uri)
		{
			var list = new List<Uri>();
			
			var item = GetItem(uri);
			
			IntPtr pidl = Shell32.SHGetIDListFromObject(item);
			try{
				var psf = Shell32.SHBindToObject<IShellFolder>(null, pidl, null);
				try{
					IEnumIDList peidl = psf.EnumObjects(OwnerHwnd, EnumConst);
					
					if(peidl == null) return list;
					try{
						while(true)
						{
							IntPtr pidl2;
							int num;
							peidl.Next(1, out pidl2, out num);
							if(num == 0) break;
							try{
								IntPtr pidl3 = Shell32.ILCombine(pidl, pidl2);
								list.Add(GetShellUri(pidl3, true));
							}finally{
								Marshal.FreeCoTaskMem(pidl2);
							}
						}
					}finally{
						Marshal.FinalReleaseComObject(peidl);
					}
				}finally{
					Marshal.FinalReleaseComObject(psf);
				}
			}finally{
				Marshal.FreeCoTaskMem(pidl);
			}
			
			return list;
		}
		
		public Task<ResourceHandle> PerformOperationAsync(Uri uri, ResourceOperation operation, object arg)
		{
			IShellItem source = null, target = null;
			string name = null;
			FileAttributes attributes = 0;
			IPropertyChangeArray properties = null;
			
			switch(operation)
			{
				case ResourceOperation.Create:
					attributes = (FileAttributes)arg;
					name = HttpUtility.UrlDecode(uri.Segments[uri.Segments.Length-1]);
					uri = new Uri(uri, ".");
					source = GetItem(uri);
					break;
				case ResourceOperation.Delete:
					source = GetItem(uri);
					break;
				case ResourceOperation.Move:
					source = GetItem(uri);
					name = arg as string;
					if(name == null)
					{
						uri = (Uri)arg;
						name = HttpUtility.UrlDecode(uri.Segments[uri.Segments.Length-1]);
						uri = new Uri(uri, ".");
						target = GetItem(uri);
					}
					break;
				case ResourceOperation.Copy:
					source = GetItem(uri);
					uri = (Uri)arg;
					name = HttpUtility.UrlDecode(uri.Segments[uri.Segments.Length-1]);
					uri = new Uri(uri, ".");
					target = GetItem(uri);
					break;
				case ResourceOperation.ChangeAttributes:
					source = GetItem(uri);
					attributes = (FileAttributes)arg;
					var propvar = Propsys.VariantToPropVariant((uint)attributes);
					properties = Propsys.PSCreatePropertyChangeArray<IPropertyChangeArray>(new[]{Shell32.PKEY_FileAttributes}, new[]{Propsys.PKA_FLAGS.PKA_SET}, new[]{propvar});
					break;
			}
			
			FileOperationProgressSink sink;
			var op = CreateOperation(operation, source, target, name, attributes, properties, out sink);
			return Task.Run((Func<Task<ResourceHandle>>)(async ()=>await FinishOperation(op, sink)));
		}
		#endregion
		
		private async Task<ResourceHandle> FinishOperation(IFileOperation operation, FileOperationProgressSink sink)
		{
			operation.PerformOperations();
			await sink.WhenCompleted();
			var item = sink.CreatedItems.LastOrDefault();
			if(item != null)
			{
				return new ShellFileHandle(item, this);
			}else{
				return null;
			}
		}
		
		private IFileOperation CreateOperation(ResourceOperation operation, IShellItem source, IShellItem target, string name, FileAttributes attributes, IPropertyChangeArray properties, out FileOperationProgressSink sink)
		{
			var op = Shell32.CreateFileOperation();
			sink = new FileOperationProgressSink();
			op.Advise(sink);
			op.SetOwnerWindow(OwnerHwnd);
			op.SetOperationFlags(0x0400 | 0x0004 | 0x0200 | 0x00100000);
			switch(operation)
			{
				case ResourceOperation.Create:
					op.NewItem(source, attributes, name, null, null);
					break;
				case ResourceOperation.Delete:
					op.DeleteItem(source, null);
					break;
				case ResourceOperation.Move:
					if(target == null)
					{
						op.RenameItem(source, name, null);
					}else{
						op.MoveItem(source, target, name, null);
					}
					break;
				case ResourceOperation.Copy:
					op.CopyItem(source, target, name, null);
					break;
				case ResourceOperation.ChangeAttributes:
					op.SetProperties(properties);
					op.ApplyPropertiesToItem(source);
					break;
			}
			return op;
		}
		
		internal object GetTargetItem(IShellItem shellItem)
		{
			var item = (IShellItem2)shellItem;
			try{
				string linkuri = item.GetString(Shell32.PKEY_Link_TargetUrl);
				return linkuri;
			}catch(COMException e)
			{
				if(unchecked((uint)e.HResult) != 0x80070490) throw;
			}
			
			var attr = item.GetAttributes(SFGAOF.SFGAO_LINK | SFGAOF.SFGAO_FILESYSTEM);
			
			if((attr & SFGAOF.SFGAO_LINK) != 0)
			{
				return item.BindToHandler<IShellItem>(null, Shell32.BHID_LinkTargetItem);
			}else{
				try{
					var link = item.BindToHandler<IShellLink>(null, Shell32.BHID_SFUIObject);
					return GetLinkTarget(link);
				}catch(NotImplementedException)
				{
					
				}catch(InvalidCastException)
				{
					
				}
			}
			return null;
		}
		
		private Uri GetItemUri(IShellItem item)
		{
			IntPtr pidl = Shell32.SHGetIDListFromObject(item);
			return GetShellUri(pidl, true);
		}
		
		private class FileOperationProgressSink : IFileOperationProgressSink
		{
			readonly TaskCompletionSource<object> task;
			
			public List<IShellItem> CreatedItems{get; private set;}
			
			public FileOperationProgressSink()
			{
				task = new TaskCompletionSource<object>();
				CreatedItems = new List<IShellItem>();
			}
			
			public Task WhenCompleted()
			{
				return task.Task;
			}
			
			void IFileOperationProgressSink.StartOperations()
			{
				
			}
			
			void IFileOperationProgressSink.FinishOperations(HRESULT hrResult)
			{
				var exception = Marshal.GetExceptionForHR((int)hrResult);
				if(exception != null)
				{
					task.TrySetException(exception);
				}else{
					task.TrySetResult(null);
				}
			}
			
			void IFileOperationProgressSink.PreRenameItem(int dwFlags, IShellItem psiItem, string pszNewName)
			{
				
			}
			
			void IFileOperationProgressSink.PostRenameItem(int dwFlags, IShellItem psiItem, string pszNewName, HRESULT hrRename, IShellItem psiNewlyCreated)
			{
				CreatedItems.Add(psiNewlyCreated);
			}
			
			void IFileOperationProgressSink.PreMoveItem(int dwFlags, IShellItem psiItem, IShellItem psiDestinationFolder, string pszNewName)
			{
				
			}
			
			void IFileOperationProgressSink.PostMoveItem(int dwFlags, IShellItem psiItem, IShellItem psiDestinationFolder, string pszNewName, HRESULT hrMove, IShellItem psiNewlyCreated)
			{
				CreatedItems.Add(psiNewlyCreated);
			}
			
			void IFileOperationProgressSink.PreCopyItem(int dwFlags, IShellItem psiItem, IShellItem psiDestinationFolder, string pszNewName)
			{
				
			}
			
			void IFileOperationProgressSink.PostCopyItem(int dwFlags, IShellItem psiItem, IShellItem psiDestinationFolder, string pszNewName, HRESULT hrCopy, IShellItem psiNewlyCreated)
			{
				CreatedItems.Add(psiNewlyCreated);
			}
			
			void IFileOperationProgressSink.PreDeleteItem(int dwFlags, IShellItem psiItem)
			{
				
			}
			
			void IFileOperationProgressSink.PostDeleteItem(int dwFlags, IShellItem psiItem, HRESULT hrDelete, IShellItem psiNewlyCreated)
			{
				CreatedItems.Add(psiNewlyCreated);
			}
			
			void IFileOperationProgressSink.PreNewItem(int dwFlags, IShellItem psiDestinationFolder, string pszNewName)
			{
				
			}
			
			void IFileOperationProgressSink.PostNewItem(int dwFlags, IShellItem psiDestinationFolder, string pszNewName, string pszTemplateName, int dwFileAttributes, HRESULT hrNew, IShellItem psiNewItem)
			{
				CreatedItems.Add(psiNewItem);
			}
			
			void IFileOperationProgressSink.UpdateProgress(int iWorkTotal, int iWorkSoFar)
			{
				
			}
			
			void IFileOperationProgressSink.ResetTimer()
			{
				
			}
			
			void IFileOperationProgressSink.PauseTimer()
			{
				
			}
			
			void IFileOperationProgressSink.ResumeTimer()
			{
				
			}
		}
	}
}
