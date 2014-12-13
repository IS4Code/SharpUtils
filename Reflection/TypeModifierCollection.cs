/* Date: 19.11.2014, Time: 14:55 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection
{
	public class TypeModifierCollection : ReadOnlyCollection<TypeModifier>, ISignatureElement
	{
		public static readonly TypeModifierCollection Empty = new TypeModifierCollection(new CustomTypeModifier[0]);
		
		public TypeModifierCollection(IList<TypeModifier> modifiers) : base(modifiers.ToArray())
		{
			
		}
		
		public TypeModifierCollection(Type[] required, Type[] optional) : this(GetModifiers(required, optional))
		{
			
		}
		
		public CallingConvention CallingConvention{
			get{
				if(HasModifier(CustomTypeModifier.CallConvCdecl)) return CallingConvention.Cdecl;
				if(HasModifier(CustomTypeModifier.CallConvFastcall)) return CallingConvention.FastCall;
				if(HasModifier(CustomTypeModifier.CallConvStdcall)) return CallingConvention.StdCall;
				if(HasModifier(CustomTypeModifier.CallConvThiscall)) return CallingConvention.ThisCall;
				return 0;
			}
		}
		
		public bool CompilerMarshalOverride{
			get{
				return HasModifier(CustomTypeModifier.CompilerMarshalOverride);
			}
		}
		
		public bool IsBoxed{
			get{
				return HasModifier(CustomTypeModifier.IsBoxed);
			}
		}
		
		public bool IsByValue{
			get{
				return HasModifier(CustomTypeModifier.IsByValue);
			}
		}
		
		public bool IsConst{
			get{
				return HasModifier(CustomTypeModifier.IsConst);
			}
		}
		
		public bool IsCopyConstructed{
			get{
				return HasModifier(CustomTypeModifier.IsCopyConstructed);
			}
		}
		
		public bool IsExplicitlyDereferenced{
			get{
				return HasModifier(CustomTypeModifier.IsExplicitlyDereferenced);
			}
		}
		
		public bool IsImplicitlyDereferenced{
			get{
				return HasModifier(CustomTypeModifier.IsImplicitlyDereferenced);
			}
		}
		
		public bool IsJitIntrinsic{
			get{
				return HasModifier(CustomTypeModifier.IsJitIntrinsic);
			}
		}
		
		public bool IsLong{
			get{
				return HasModifier(CustomTypeModifier.IsLong);
			}
		}
		
		public bool IsPinned{
			get{
				return HasModifier(CustomTypeModifier.IsPinned);
			}
		}
		
		public bool IsSignUnspecifiedByte{
			get{
				return HasModifier(CustomTypeModifier.IsSignUnspecifiedByte);
			}
		}
		
		public bool IsUdtReturn{
			get{
				return HasModifier(CustomTypeModifier.IsUdtReturn);
			}
		}
		
		public bool IsVolatile{
			get{
				return HasModifier(CustomTypeModifier.IsVolatile);
			}
		}
		
		public Type BoxedType{
			get{
				if(IsBoxed)
				{
					foreach(CustomTypeModifier modifier in this)
					{
						if(modifier.ModifierType.IsValueType) return modifier.ModifierType;
					}
				}
				throw new Exception("No boxed type is defined.");
			}
		}
		
		public IEnumerable<CustomTypeModifier> EnumerateCustomModifiers()
		{
			return this.Select(m => m as CustomTypeModifier).Where(m => m != null);
		}
		
		public IEnumerable<Type> EnumerateOptionalCustomModifiers()
		{
			return EnumerateCustomModifiers().Where(m => m.IsOptional).Select(m => m.ModifierType);
		}
		
		public IEnumerable<Type> EnumerateRequiredCustomModifiers()
		{
			return EnumerateCustomModifiers().Where(m => m.IsRequired).Select(m => m.ModifierType);
		}
		
		public bool HasModifier(Type modifierType)
		{
			return EnumerateCustomModifiers().Any(m => m.ModifierType == modifierType);
		}
		
		public bool HasModifier(Type modifierType, bool required)
		{
			return EnumerateCustomModifiers().Any(m => m.ModifierType == modifierType && m.IsRequired == required);
		}
		
		public bool HasModifier(TypeModifier modifier)
		{
			return this.Any(m => m.Equals(modifier));
		}
		
		public bool? GetModifier(Type modifierType)
		{
			foreach(CustomTypeModifier modifier in this)
			{
				if(modifier.ModifierType == modifierType) return modifier.IsRequired;
			}
			return null;
		}
		
		private static CustomTypeModifier[] GetModifiers(Type[] required, Type[] optional)
		{
			CustomTypeModifier[] modifiers = new CustomTypeModifier[required.Length+optional.Length];
			for(int i = 0; i < required.Length; i++)
			{
				modifiers[i] = new CustomTypeModifier(required[i], true);
			}
			for(int i = 0; i < optional.Length; i++)
			{
				modifiers[i+required.Length] = new CustomTypeModifier(optional[i], false);
			}
			return modifiers;
		}
		
		void ISignatureElement.AddSignature(SignatureHelper signature)
		{
			foreach(var modifier in this)
			{
				signature.AddElement(modifier);
			}
		}
	}
}
