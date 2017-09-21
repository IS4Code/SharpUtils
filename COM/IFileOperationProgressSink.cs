/* Date: 20.9.2017, Time: 23:22 */
using System;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Com
{
	[ComImport]
	[Guid("04B0F1A7-9490-44BC-96E1-4296A31252E2")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IFileOperationProgressSink
	{
		void StartOperations();
		void FinishOperations(HRESULT hrResult);
		void PreRenameItem(int dwFlags, IShellItem psiItem, [MarshalAs(UnmanagedType.LPWStr)]string pszNewName);
		void PostRenameItem(int dwFlags, IShellItem psiItem, [MarshalAs(UnmanagedType.LPWStr)]string pszNewName, HRESULT hrRename, IShellItem psiNewlyCreated);
		void PreMoveItem(int dwFlags, IShellItem psiItem, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)]string pszNewName);
		void PostMoveItem(int dwFlags, IShellItem psiItem, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)]string pszNewName, HRESULT hrMove, IShellItem psiNewlyCreated);
		void PreCopyItem(int dwFlags, IShellItem psiItem, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)]string pszNewName);
		void PostCopyItem(int dwFlags, IShellItem psiItem, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)]string pszNewName, HRESULT hrCopy, IShellItem psiNewlyCreated);
		void PreDeleteItem(int dwFlags, IShellItem psiItem);
		void PostDeleteItem(int dwFlags, IShellItem psiItem, HRESULT hrDelete, IShellItem psiNewlyCreated);
		void PreNewItem(int dwFlags, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)]string pszNewName);
		void PostNewItem(int dwFlags, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)]string pszNewName, [MarshalAs(UnmanagedType.LPWStr)]string pszTemplateName, int dwFileAttributes, HRESULT hrNew, IShellItem psiNewItem);
		void UpdateProgress(int iWorkTotal, int iWorkSoFar);
		void ResetTimer();
		void PauseTimer();
		void ResumeTimer();
	}
}
