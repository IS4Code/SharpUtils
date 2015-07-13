using System;
using System.Collections;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.Patterns
{
	[PatternAttribute]
	public abstract class PStaticSingleTypeEqualityComparer<[StaticType]T> : PStaticEqualityComparer<T,T>, IEqualityComparer<T>, IEqualityComparer
	{
		int IEqualityComparer<T>.GetHashCode(T t)
		{
			return t.GetHashCode();
		}
		
		int IEqualityComparer.GetHashCode(object o)
		{
			return o.GetHashCode();
		}
	}
}
