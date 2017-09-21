/* Date: 20.9.2017, Time: 23:44 */
using System;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Com
{
	[ComImport]
	[Guid("F917BC8A-1BBA-4478-A245-1BDE03EB9431")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IOperationsProgressDialog
	{
        void StartProgressDialog(IntPtr hwndOwner, OPPROGDLGF flags);
        void StopProgressDialog();
        void SetOperation(SPACTION action);
        void SetMode(PDMODE mode);
        void UpdateProgress(long ullPointsCurrent, long ullPointsTotal, long ullSizeCurrent, long ullSizeTotal, long ullItemsCurrent, long ullItemsTotal);
        void UpdateLocations(IShellItem psiSource, IShellItem psiTarget, IShellItem psiItem);
        void ResetTimer();
        void PauseTimer();
        void ResumeTimer();
        void GetMilliseconds(out long pullElapsed, out long pullRemaining);
        PDOPSTATUS GetOperationStatus();
	}
	
	[Flags]
	public enum OPPROGDLGF
	{
		OPPROGDLG_DEFAULT = 0,
		OPPROGDLG_ENABLEPAUSE = 0x80,
		OPPROGDLG_ALLOWUNDO = 0x100,
		OPPROGDLG_DONTDISPLAYSOURCEPATH = 0x200,
		OPPROGDLG_DONTDISPLAYDESTPATH = 0x400,
		OPPROGDLG_NOMULTIDAYESTIMATES = 0x800,
		OPPROGDLG_DONTDISPLAYLOCATIONS = 0x1000,
	}
	
	public enum SPACTION
	{
		SPACTION_NONE = 0,
		SPACTION_MOVING = ( SPACTION_NONE + 1 ) ,
		SPACTION_COPYING = ( SPACTION_MOVING + 1 ) ,
		SPACTION_RECYCLING = ( SPACTION_COPYING + 1 ) ,
		SPACTION_APPLYINGATTRIBS = ( SPACTION_RECYCLING + 1 ) ,
		SPACTION_DOWNLOADING = ( SPACTION_APPLYINGATTRIBS + 1 ) ,
		SPACTION_SEARCHING_INTERNET = ( SPACTION_DOWNLOADING + 1 ) ,
		SPACTION_CALCULATING = ( SPACTION_SEARCHING_INTERNET + 1 ) ,
		SPACTION_UPLOADING = ( SPACTION_CALCULATING + 1 ) ,
		SPACTION_SEARCHING_FILES = ( SPACTION_UPLOADING + 1 ) ,
		SPACTION_DELETING = ( SPACTION_SEARCHING_FILES + 1 ) ,
		SPACTION_RENAMING = ( SPACTION_DELETING + 1 ) ,
		SPACTION_FORMATTING = ( SPACTION_RENAMING + 1 ) ,
		SPACTION_COPY_MOVING = ( SPACTION_FORMATTING + 1 ),
	}
	
	public enum PDMODE
	{
		PDM_DEFAULT = 0,
		PDM_RUN = 0x1,
		PDM_PREFLIGHT = 0x2,
		PDM_UNDOING = 0x4,
		PDM_ERRORSBLOCKING = 0x8,
		PDM_INDETERMINATE = 0x10,
	}
	
	public enum PDOPSTATUS
	{
		PDOPS_RUNNING = 1,
		PDOPS_PAUSED = 2,
		PDOPS_CANCELLED = 3,
		PDOPS_STOPPED = 4,
		PDOPS_ERRORS = 5,
	}
}
