using System;
using System.Collections;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.Templates
{
	[Template]
	public abstract class TStaticSingleTypeEqualityComparer<[StaticType]T> : TStaticEqualityComparer<T,T>, IEqualityComparer<T>, IEqualityComparer
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
