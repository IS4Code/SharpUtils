using System;

namespace IllidanS4.SharpUtils
{
	/// <summary>
	/// A class providing the empty value for a type.
	/// </summary>
	public static class Empty<T>
	{
		/// <summary>
		/// The empty value.
		/// </summary>
		public static readonly T Value;
		
		static Empty()
		{
			Type t = TypeOf<T>.TypeID;
			if(t.IsValueType)return;
			Value = (T)EmptyManager.GetEmpty(t);
		}
	}
}