using System;
using System.Reflection;

namespace IllidanS4.SharpUtils.Reflection.CSharp
{
	partial class LanguageType
	{
		private partial class DynamicTypeDescription : LanguageType
		{
			private const BindingFlags mflags = BindingFlags.Public | BindingFlags.Instance;
			
			public override string Name{
				get{ return "dynamic"; }
			}
			
			public DynamicTypeDescription() : base(TypeOf<object>.TypeID)
			{
				
			}
			
			public override bool IsAssignableFrom(Type c)
			{
				return true;
			}
			
			protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, System.Reflection.Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
			{
				if((bindingAttr & mflags) == mflags)return new DynamicMethodInfo(name, bindingAttr, this);
				else return null;
			}
			
			public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
			{
				return new MethodInfo[0];
			}
			
			protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
			{
				if((bindingAttr & mflags) == mflags)return new DynamicPropertyInfo(name, bindingAttr, this);
				else return null;
			}
		}
	}
}
