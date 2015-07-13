using System;
using System.Collections;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.Patterns
{
	[Pattern]
	public abstract class PStaticSingleTypeComparer<[StaticType]T> : PStaticComparer<T,T>, IComparer<T>, IComparer
	{
		
	}
}
