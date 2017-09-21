/* Date: 21.9.2017, Time: 11:10 */
using System;

namespace IllidanS4.SharpUtils.Com
{
	public enum HRESULT
	{
		E = unchecked((int)0x80004004),
		S_OK = 0x00000000,
		E_FAIL = E | 0x00004005,
	}
}
