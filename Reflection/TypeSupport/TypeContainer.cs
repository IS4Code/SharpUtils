/* Date: 29.3.2015, Time: 14:24 */
using System;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection.TypeSupport
{
	public class TypeContainer : TypeConstruct
	{
		public TypeContainer(Type baseType) : base(baseType)
		{
			
		}
		
		public override CorElementType CorElementType{
			get{
				return UnderlyingSystemType.GetCorElementType();
			}
		}
		
		protected override void AddSignature(SignatureHelper signature)
		{
			signature.AddArgumentSignature(UnderlyingSystemType);
		}
	}
}
