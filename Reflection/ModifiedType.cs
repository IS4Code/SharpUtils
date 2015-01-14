/*
 * Created by SharpDevelop.
 * User: Illidan
 * Date: 21.12.2012
 * Time: 22:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using IllidanS4.SharpUtils.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.TypeSupport;

namespace IllidanS4.SharpUtils.Reflection
{
	/// <summary>
	/// Represents a type with custom modifiers.
	/// </summary>
	public class ModifiedType : TypeAppendConstruct
	{
		public TypeModifierCollection Modifiers{get; private set;}
		public override CorElementType CorElementType{
			get{
				return ElementType.GetCorElementType();
			}
		}
		
		private ModifiedType(Type baseType) : base(baseType, baseType.UnderlyingSystemType)
		{
			if(baseType == null) throw new ArgumentNullException("baseType");
		}
		
		public ModifiedType(Type baseType, params TypeModifier[] modifiers) : this(baseType, (IList<TypeModifier>)modifiers)
		{
			
		}
		
		public ModifiedType(Type baseType, IList<TypeModifier> modifiers) : this(baseType)
		{
			if(modifiers == null)
			{
				Modifiers = TypeModifierCollection.Empty;
			}else{
				Modifiers = new TypeModifierCollection(modifiers.ToArray());
			}
		}
		
		public ModifiedType(Type baseType, TypeModifierCollection modifiers) : this(baseType)
		{
			Modifiers = modifiers;
		}
		
		protected override string Append(string name)
		{
			return name+" "+String.Join(" ", Modifiers.Select(m => m.ToString()));
		}
		
		public override bool Equals(Type o)
		{
			if(!UnderlyingSystemType.Equals(o))return false;
			ModifiedType modt = o as ModifiedType;
			if(modt == null)
				return Modifiers.Count == 0;
			
			foreach(CustomTypeModifier modifier in Modifiers)
			{
				if(!modt.Modifiers.HasModifier(modifier))return false;
			}
			return true;
		}
		
		public override bool Equals(object o)
		{
			return Equals(o as Type);
		}
		
		public static bool operator ==(ModifiedType a, ModifiedType b)
		{
			if(Object.ReferenceEquals(a, b)) return true;
			else if(Object.ReferenceEquals(a, null)) return false;
			else return a.Equals(b);
		}
		
		public static bool operator !=(ModifiedType a, ModifiedType b)
		{
			return !(a == b);
		}
		
		public override int GetHashCode()
		{
			int hashCode = UnderlyingSystemType.GetHashCode();
			
			unchecked{
				foreach(CustomTypeModifier mod in Modifiers)
				{
					hashCode *= 17;
					if(mod != null) hashCode += mod.GetHashCode();
				}
			}
			return hashCode;
		}

		protected override void AddSignature(SignatureHelper signature)
		{
			signature.AddElement(Modifiers);
			signature.AddArgumentSignature(ElementType);
		}
	}
}
