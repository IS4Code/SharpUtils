using System;
using System.Globalization;
using System.Reflection;

namespace IllidanS4.SharpUtils.Reflection
{
	partial class LanguageType
	{
		private class AnonymousTypeTypeDescription : LanguageType
		{
			public override string LocalizedName{
				get{ return Resources.GetString("AnonymousType", CultureInfo.CurrentCulture); }
			}
			
			public override string Name{
				get{ return Resources.GetString("AnonymousType", CultureInfo.InvariantCulture); }
			}
			
			public override MemberInfo[] GetMember(string name, BindingFlags bindingAttr)
			{
				return typeof(object).GetMember(name, bindingAttr);
			}
			
			public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
			{
				return typeof(object).GetMember(name, type, bindingAttr);
			}
			
			public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
			{
				return typeof(object).GetMembers(bindingAttr);
			}
			
			protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, System.Reflection.Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
			{
				return typeof(object).GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
			}
			
			public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
			{
				return typeof(object).GetMethods(bindingAttr);
			}
		}
	}
}
