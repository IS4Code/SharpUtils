using System;

namespace IllidanS4.SharpUtils.Patterns
{
	[Pattern]
	public abstract class PStaticComparer<[StaticType]T1, T2> : IStaticComparer
	{
		protected abstract bool op_Equality(T1 t1, T2 t2);
		protected abstract bool op_GreaterThan(T1 t1, T2 t2);
		protected abstract bool op_LessThan(T1 t1, T2 t2);
		
		public int Compare(T1 t1, T2 t2)
		{
			if(op_Equality(t1, t2))
			{
				return 0;
			}else if(op_GreaterThan(t1, t2))
			{
				return 1;
			}else if(op_LessThan(t1, t2))
			{
				return -1;
			}
			throw new InvalidOperationException();
		}
		
		public int Compare(object t1, object t2)
		{
			return Compare((T1)t1, (T2)t2);
		}
	}
}
