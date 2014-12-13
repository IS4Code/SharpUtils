using System;

namespace IllidanS4.SharpUtils
{
	/// <summary>
	/// Provides a cached typeof operator.
	/// </summary>
	public static class TypeOf<T>
	{
		/// <summary>
		/// Returns the type instance.
		/// </summary>
		public static readonly Type TypeID = typeof(T);
	}
}
