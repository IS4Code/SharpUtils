/* Date: 1.5.2015, Time: 11:58 */
using System;

namespace IllidanS4.SharpUtils.Proxies.Dynamic
{
	/// <summary>
	/// Marks a type suitable for marshalling via its address with an <see cref="UnsafeHandle"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class MarshalByAddressAttribute : Attribute
	{
		
	}
}
