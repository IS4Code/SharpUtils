/* Date: 29.7.2015, Time: 18:55 */
using System;

namespace IllidanS4.SharpUtils
{
	/// <summary>
	/// Descriptive exception per Microsoft's standards.
	/// </summary>
	/// <remarks>
	/// Inspired by http://cdn.windowsreport.com/wp-content/uploads/2016/02/something-happened.jpg
	/// </remarks>
	public sealed class SomeException : Exception
	{
		/// <summary>
		/// Creates a new exception for any occasion.
		/// </summary>
		public SomeException() : base("Something happened")
		{
			
		}
	}
}
