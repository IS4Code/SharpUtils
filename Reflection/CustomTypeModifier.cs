using System;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using IllidanS4.SharpUtils.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection
{
	public sealed class CustomTypeModifier : TypeModifier, IEquatable<CustomTypeModifier>
	{
		public static readonly Type IsBoxed = typeof(IsBoxed);
		public static readonly Type IsByValue = typeof(IsByValue);
		public static readonly Type IsConst = typeof(IsConst);
		public static readonly Type IsCopyConstructed = typeof(IsCopyConstructed);
		public static readonly Type IsExplicitlyDereferenced = typeof(IsExplicitlyDereferenced);
		public static readonly Type IsImplicitlyDereferenced = typeof(IsImplicitlyDereferenced);
		public static readonly Type IsJitIntrinsic = typeof(IsJitIntrinsic);
		public static readonly Type IsLong = typeof(IsLong);
		public static readonly Type IsPinned = typeof(IsPinned);
		public static readonly Type IsSignUnspecifiedByte = typeof(IsSignUnspecifiedByte);
		public static readonly Type IsUdtReturn = typeof(IsUdtReturn);
		public static readonly Type IsVolatile = typeof(IsVolatile);
		
		public static readonly Type CompilerMarshalOverride = typeof(CompilerMarshalOverride);
		
		public static readonly Type CallConvCdecl = typeof(CallConvCdecl);
		public static readonly Type CallConvFastcall = typeof(CallConvFastcall);
		public static readonly Type CallConvStdcall = typeof(CallConvStdcall);
		public static readonly Type CallConvThiscall = typeof(CallConvThiscall);
		
		public Type ModifierType{get; private set;}
		public bool IsRequired{get; private set;}
		public bool IsOptional{get{return !IsRequired;}}
		
		public CustomTypeModifier(Type classRef) : this(classRef, false)
		{
			
		}
		
		public CustomTypeModifier(Type classRef, bool required) : base(required?CorElementType.CModReqd:CorElementType.CModOpt)
		{
			if(classRef == null) throw new ArgumentNullException("classRef");
			ModifierType = classRef;
			IsRequired = required;
		}
		
		public override bool Equals(object obj)
		{
			CustomTypeModifier other = obj as CustomTypeModifier;
			if(other == null)
				return false;
			return Equals(other);
		}
		
		public override bool Equals(TypeModifier other)
		{
			return Equals((object)other);
		}
		
		public bool Equals(CustomTypeModifier other)
		{
			return this.ModifierType == other.ModifierType && this.IsRequired == other.IsRequired;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (ModifierType != null)
					hashCode += 1000000007 * ModifierType.GetHashCode();
				hashCode += 1000000009 * IsRequired.GetHashCode();
			}
			return hashCode;
		}
		
		public static bool operator ==(CustomTypeModifier lhs, CustomTypeModifier rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(CustomTypeModifier lhs, CustomTypeModifier rhs)
		{
			return !(lhs == rhs);
		}
		
		public override string ToString()
		{
			return (IsRequired?"modreq":"modopt")+"("+ModifierType+")";
		}
		
		protected override void AddSignature(SignatureHelper signature)
		{
			signature.AddElementType(ElementType);
			signature.AddArgumentSignature(ModifierType);
		}
	}
}
