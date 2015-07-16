using System;
namespace IllidanS4.SharpUtils
{
	/// <summary>
	/// This class can be used as an temporary storage for ByRef parameters.
	/// </summary>
	public static class Tmp<T>
	{
		/// <example>
		/// <code><![CDATA[
		/// SomeMethod(out Tmp<object>.Value);
		/// ]]></code>
		/// </example>
		[ThreadStatic]
		public static T Value;
	}
}