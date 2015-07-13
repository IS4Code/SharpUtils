using System;
using System.Collections;

namespace IllidanS4.SharpUtils.Templates
{
	[Template]
	public abstract class TStaticEqualityComparer<[StaticType]T1, T2> : IStaticEqualityComparer
	{
		protected abstract bool op_Equality(T1 t1, T2 t2);
		
		public bool Equals(T1 t1, T2 t2)
		{
			return op_Equality(t1, t2);
		}
		
		public new bool Equals(object t1, object t2)
		{
			return Equals((T1)t1, (T2)t2);
		}
		
		int IEqualityComparer.GetHashCode(object o)
		{
			return o.GetHashCode();
		}
	}
}
