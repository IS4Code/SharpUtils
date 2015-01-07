/* Date: 31.12.2014, Time: 13:07 */
using System;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.TypeSupport;

namespace IllidanS4.SharpUtils.Reflection
{
	/// <summary>
	/// Used to distinguish between basic element type and typeref in signatures.
	/// </summary>
	public class TypeRefElementType : TypeConstruct
	{
		private readonly CorElementType elType;
		public override CorElementType CorElementType{
			get{
				return elType;
			}
		}
		
		public TypeRefElementType(Type typeRef) : base(typeRef)
		{
			if(typeRef.IsValueType)
			{
				elType = CorElementType.ValueType;
			}else{
				elType = CorElementType.Class;
			}
		}
		
		protected override void AddSignature(SignatureHelper signature)
		{
			signature.AddTypeRef(this);
		}
	}
}
