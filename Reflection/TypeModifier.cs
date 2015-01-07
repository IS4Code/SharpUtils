/* Date: 13.12.2014, Time: 10:51 */
using System;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection
{
	public class TypeModifier : ISignatureElement, IEquatable<TypeModifier>
	{
		public CorElementType ElementType{get; private set;}
		
		
		
		internal TypeModifier(CorElementType elementType)
		{
			ElementType = elementType;
		}
		
		protected virtual void AddSignature(SignatureHelper signature)
		{
			signature.AddElementType(ElementType);
		}
		
		void ISignatureElement.AddSignature(SignatureHelper signature)
		{
			this.AddSignature(signature);
		}
		
		public static implicit operator TypeModifier(Type modifyingType)
		{
			return new CustomTypeModifier(modifyingType);
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			TypeModifier other = obj as TypeModifier;
			if(other == null)
				return false;
			return Equals(other);
		}
		
		public virtual bool Equals(TypeModifier other)
		{
			return this.ElementType == other.ElementType;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * ElementType.GetHashCode();
			}
			return hashCode;
		}
		
		public static bool operator ==(TypeModifier lhs, TypeModifier rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(TypeModifier lhs, TypeModifier rhs)
		{
			return !(lhs == rhs);
		}
		#endregion

	}
}
