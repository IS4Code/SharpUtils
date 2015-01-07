/* Date: 4.1.2015, Time: 17:42 */
using System;
using System.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection.Emit
{
	/// <summary>
	/// Represents a signature of a field.
	/// </summary>
	public class FieldSignature : MemberSignature
	{
		public Type FieldType{get; private set;}
		
		public FieldSignature(Type fieldType) : base(MdSigCallingConvention.Field)
		{
			FieldType = fieldType;
		}
		
		protected override void AddSignature(SignatureHelper signature)
		{
			base.AddSignature(signature);
			signature.AddArgumentSignature(FieldType);
		}
	}
}
