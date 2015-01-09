/* Date: 12.12.2014, Time: 14:37 */
using System;
using System.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection.Emit
{
	/// <summary>
	/// Represents an element of a member signature.
	/// </summary>
	public interface ISignatureElement
	{
		void AddSignature(SignatureHelper signature);
	}
}
