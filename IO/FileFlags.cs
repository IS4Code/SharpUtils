/* Date: 23.9.2017, Time: 16:18 */
using System;

namespace IllidanS4.SharpUtils.IO
{
	[Flags]
	public enum FileFlags
	{
		None = 0,
		BackupSemantics = 0x02000000,
		DeleteOnClose = 0x04000000,
		NoBuffering = 0x20000000,
		OpenNoRecall = 0x00100000,
		OpenReparsePoint = 0x00200000,
		Overlapped = 0x40000000,
		PosixSemantics = 0x0100000,
		RandomAccess = 0x10000000,
		SessionAware = 0x00800000,
		SequentialScan = 0x08000000,
		WriteThrough = unchecked((int)0x80000000),
	}
}
