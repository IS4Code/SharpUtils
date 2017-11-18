/* Date: 18.11.2017, Time: 2:25 */
using System;
using System.Runtime.Serialization;

namespace IllidanS4.SharpUtils.ObjectModel
{
	/// <summary>
	/// This exception is thrown when a property is not completed during the object's completion time.
	/// </summary>
	public class PropertyIncompleteException : Exception, ISerializable
	{
		public PropertyIncompleteException() : this("The property wasn't completed.")
		{
			
		}

	 	public PropertyIncompleteException(string message) : base(message)
		{
	 		
		}

		public PropertyIncompleteException(string message, Exception innerException) : base(message, innerException)
		{
			
		}

		protected PropertyIncompleteException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			
		}
	}
}