/* Date: 29.10.2017, Time: 21:24 */
using System;
using System.IO;

namespace IllidanS4.SharpUtils.IO
{
	/// <summary>
	/// Represents a resource capable of opening data streams to itself.
	/// </summary>
	public interface IStreamSource
	{
		/// <summary>
		/// Opens a new stream to the resource, with as most permissions as possible.
		/// </summary>
		/// <returns>The newly opened stream.</returns>
		Stream OpenStream();
	}
}
