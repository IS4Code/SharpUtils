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
	/// Description of ModifiedType.
	/// </summary>
	public class ModifiedType : TypeAppendConstruct
	{
		public TypeModifierCollection Modifiers{get; private set;}
		
		private ModifiedType(Type baseType) : base(baseType, baseType.UnderlyingSystemType)
		{
			if(baseType == null) throw new ArgumentNullException("baseType");
		}
		
		public ModifiedType(Type baseType, params CustomTypeModifier[] modifiers) : this(baseType, (IList<CustomTypeModifier>)modifiers)
		{
			
		}
		
		public ModifiedType(Type baseType, IList<CustomTypeModifier> modifiers) : this(baseType)
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
		
		/*public override string Name{
			get{
				return base.Name;
			}
		}*/
		
		public static bool operator ==(ModifiedType a, ModifiedType b)
		{
			if(a==null)return b==null;
			if(b==null)return false;
			return a.Equals(b);
		}
		
		public static bool operator !=(ModifiedType a, ModifiedType b)
		{
			if(a==null)return b!=null;
			if(b==null)return true;
			return !a.Equals(b);
		}
		
		public static bool operator ==(Type a, ModifiedType b)
		{
			if(a==null)return b==null;
			if(b==null)return false;
			return b.Equals(a);
		}
		
		
		public static bool operator !=(Type a, ModifiedType b)
		{
			if(a==null)return b!=null;
			if(b==null)return true;
			return !b.Equals(a);
		}
		
		public static bool operator ==(ModifiedType a, Type b)
		{
			if(a==null)return b==null;
			if(b==null)return false;
			return a.Equals(b);
		}
		
		public static bool operator !=(ModifiedType a, Type b)
		{
			if(a==null)return b!=null;
			if(b==null)return true;
			return !a.Equals(b);
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
