/* Date: 29.7.2015, Time: 18:55 */
using System;

namespace IllidanS4.SharpUtils
{
	/// <summary>
	/// Descriptive exception per Microsoft's standards.
	/// </summary>
	/// <remarks>
	/// Inspired by https://twitter.com/jonathantimar/status/626269345908285441/photo/1
	/// </remarks>
	public sealed class SomeException : Exception
	{
		public SomeException() : base("Something happened")
		{
			
		}
	}
}
