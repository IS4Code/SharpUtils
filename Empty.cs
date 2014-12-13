using System;

namespace IllidanS4.SharpUtils
{
	public static class Empty<T>
	{
		public static readonly T Value;
		
		static Empty()
		{
			Type t = TypeOf<T>.TypeID;
			if(t.IsValueType)return;
			Value = (T)EmptyManager.GetEmpty(t);
		}
	}
}