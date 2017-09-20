/* Date: 20.9.2017, Time: 23:02 */
using System;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.COM
{
	[ComImport]
	[Guid("947AAB5F-0A5C-4C13-B4D6-4BF7836FC9F8")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IFileOperation
	{
		int Advise(IFileOperationProgressSink pfops);
		void Unadvise(int dwCookie);
		void SetOperationFlags(int dwOperationFlags);
		void SetProgressMessage([MarshalAs(UnmanagedType.LPWStr)]string pszMessage);
		void SetProgressDialog(IOperationsProgressDialog popd);
		void SetProperties(IPropertyChangeArray pproparray);
		void SetOwnerWindow(IntPtr hwndOwner);
		void ApplyPropertiesToItem(IShellItem psiItem);
		void ApplyPropertiesToItems([MarshalAs(UnmanagedType.IUnknown)]object punkItems);
		void RenameItem(IShellItem psiItem, [MarshalAs(UnmanagedType.LPWStr)]string pszNewName, IFileOperationProgressSink pfopsItem);
		void RenameItems([MarshalAs(UnmanagedType.IUnknown)]object pUnkItems, [MarshalAs(UnmanagedType.LPWStr)]string pszNewName);
		void MoveItem(IShellItem psiItem, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)]string pszNewName, IFileOperationProgressSink pfopsItem);
		void MoveItems([MarshalAs(UnmanagedType.IUnknown)]object punkItems, IShellItem psiDestinationFolder);
		void CopyItem(IShellItem psiItem, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)]string pszCopyName, IFileOperationProgressSink pfopsItem);
		void CopyItems([MarshalAs(UnmanagedType.IUnknown)]object punkItems, IShellItem psiDestinationFolder);
		void DeleteItem(IShellItem psiItem, IFileOperationProgressSink pfopsItem);
		void DeleteItems([MarshalAs(UnmanagedType.IUnknown)]object punkItems);
		void NewItem( IShellItem psiDestinationFolder, int dwFileAttributes, [MarshalAs(UnmanagedType.LPWStr)]string pszName, [MarshalAs(UnmanagedType.LPWStr)]string pszTemplateName, IFileOperationProgressSink pfopsItem);
		void PerformOperations();
		bool GetAnyOperationsAborted();
	}
}
