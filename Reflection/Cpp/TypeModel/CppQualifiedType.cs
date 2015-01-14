/* Date: 2.12.2014, Time: 21:15 */
using System;

namespace IllidanS4.SharpUtils.Reflection.Cpp
{
	/// <summary>
	/// Type with type qualifiers.
	/// </summary>
	public abstract class CppQualifiedType : CppDerivedType
	{
		public string ModifierName{get; private set;}
		public Type Modifier{get; private set;}
		
		protected CppQualifiedType(CppType type, string name, Type modifier) : base(type, name)
		{
			ModifierName = name;
			Modifier = modifier;
		}
		
		public override Type ManagedType{
			get{
				return new ModifiedType(ElementType.ManagedType, new CustomTypeModifier(Modifier));
			}
		}
	}
}
