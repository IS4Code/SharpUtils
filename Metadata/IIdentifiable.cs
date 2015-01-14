/* Date: 19.11.2014, Time: 13:17 */
using System;

namespace IllidanS4.SharpUtils.Metadata
{
	/// <summary>
	/// An interface for types that have an identifier.
	/// </summary>
	public interface IIdentifiable<out TIdentifier> 
	{
		/// <summary>
		/// The identifier of this object.
		/// </summary>
		TIdentifier ID{get;}
	}
}
