using System;
using System.Collections;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.Templates
{
	[Template]
	public abstract class TStaticSingleTypeComparer<[StaticType]T> : TStaticComparer<T,T>, IComparer<T>, IComparer
	{
		
	}
}
