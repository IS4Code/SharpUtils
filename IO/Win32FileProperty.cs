/* Date: 23.9.2017, Time: 20:13 */
using System;

namespace IllidanS4.SharpUtils.IO
{
	public enum Win32FileProperty
	{
		LinkSubstituteName,
		LinkPrintName,
		
		DosPath = 16,
		GuidPath = DosPath+1,
		DevicePath = DosPath+2,
		BarePath = DosPath+4,
	}
}
