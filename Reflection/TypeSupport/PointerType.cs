/* Date: 3.12.2014, Time: 16:06 */
using System;
using System.Reflection;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection.TypeSupport
{
	/// <summary>
	/// Represents a derived pointer type.
	/// </summary>
	public class PointerType : TypeAppendConstruct
	{
		public override CorElementType CorElementType{
			get{
				return CorElementType.Ptr;
			}
		}
		
		public PointerType(Type elementType) : base(elementType, UnderlyingPointerType(elementType))
		{
			
		}
		
		protected override string Append(string name)
		{
			return name+"*";
		}
		
		private static Type UnderlyingPointerType(Type elementType)
		{
			if(elementType is FunctionPointerType)
			{
				return Types.Generated.FnPtrPointer;
			}else{
				return elementType.UnderlyingSystemType.MakePointerType();
			}
		}
		
		protected override void AddSignature(SignatureHelper signature)
		{
			signature.AddElementType(CorElementType.Ptr);
			signature.AddArgumentSignature(ElementType);
		}
	}
}
