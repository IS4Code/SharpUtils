using System;
using System.Reflection;

namespace IllidanS4.SharpUtils.Reflection
{
	public static class ModifiedTypeExtensions
	{
		public static ModifiedType GetModifiedReturnType(this MethodInfo mi)
		{
			return new ModifiedType(mi.ReturnType, GetTypeModifiers(mi.ReturnParameter));
		}
		
		public static ModifiedType GetModifiedType(this MemberInfo mi)
		{
			PropertyInfo pi = mi as PropertyInfo;
			if(pi != null) return GetModifiedType(pi);
			FieldInfo fi = mi as FieldInfo;
			if(fi != null) return GetModifiedType(fi);
			MethodInfo mti = mi as MethodInfo;
			if(mti != null) return GetModifiedReturnType(mti);
			return null;
		}
		
		public static TypeModifierCollection GetTypeModifiers(this MemberInfo mi)
		{
			PropertyInfo pi = mi as PropertyInfo;
			if(pi != null) return GetTypeModifiers(pi);
			FieldInfo fi = mi as FieldInfo;
			if(fi != null) return GetTypeModifiers(fi);
			MethodInfo mti = mi as MethodInfo;
			if(mti != null) return GetReturnTypeModifiers(mti);
			return null;
		}
		
		public static TypeModifierCollection GetTypeModifiers(this ParameterInfo pi)
		{
			return new TypeModifierCollection(pi.GetRequiredCustomModifiers(), pi.GetOptionalCustomModifiers());
		}
		
		public static TypeModifierCollection GetTypeModifiers(this FieldInfo fi)
		{
			return new TypeModifierCollection(fi.GetRequiredCustomModifiers(), fi.GetOptionalCustomModifiers());
		}
		
		public static TypeModifierCollection GetTypeModifiers(this PropertyInfo pi)
		{
			return new TypeModifierCollection(pi.GetRequiredCustomModifiers(), pi.GetOptionalCustomModifiers());
		}
		
		public static TypeModifierCollection GetReturnTypeModifiers(this MethodInfo mi)
		{
			return GetTypeModifiers(mi.ReturnParameter);
		}
		
		public static ModifiedType GetModifiedType(this ParameterInfo pi)
		{
			return new ModifiedType(pi.ParameterType, GetTypeModifiers(pi));
		}
		
		public static ModifiedType GetModifiedType(this FieldInfo fi)
		{
			return new ModifiedType(fi.FieldType, GetTypeModifiers(fi));
		}
		
		public static ModifiedType GetModifiedType(this PropertyInfo pi)
		{
			return new ModifiedType(pi.PropertyType, GetTypeModifiers(pi));
		}
	}
}
