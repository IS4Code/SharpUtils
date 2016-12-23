/* Date: 23.3.2016, Time: 22:27 */
using System;

namespace IllidanS4.SharpUtils.Serialization
{
	/// <summary>
	/// Represents an object capable of cloning an instance with a user-defined method.
	/// </summary>
	public interface ICloner<T>
	{
		T Clone(T instance);
	}
}
