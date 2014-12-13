/* Date: 12.11.2014, Time: 15:38 */
using System;
using System.Diagnostics;

namespace IllidanS4.SharpUtils.Reflection
{
	public class AnonymousTypeFactory<T> : IAnonymousTypeFactory
	{
		private static readonly Type DebuggerDisplayAttribute_Type = typeof(DebuggerDisplayAttribute);
		
		private readonly Type anonymousType;
		private readonly T defValue;
		private readonly string display;
		public Type AnonymousType{get{return anonymousType;}}
		public T DefaultValue{get{return defValue;}}
		
		object IAnonymousTypeFactory.DefaultValue{get{return defValue;}}
		
		public AnonymousTypeFactory(T def)
		{
			anonymousType = TypeOf<T>.TypeID;
			defValue = def;
			object[] attrs = anonymousType.GetCustomAttributes(DebuggerDisplayAttribute_Type, false);
			if(attrs.Length >= 1)
			{
				display = ((DebuggerDisplayAttribute)attrs[0]).Value.Replace("\\{", "{");
			}
		}
		
		public T Create(params object[] args)
		{
			return (T)Activator.CreateInstance(anonymousType, args);
		}
		
		object IAnonymousTypeFactory.Create(params object[] args)
		{
			return this.Create(args);
		}
		
		public bool Test<T1>(T1 instance)
		{
			return instance is T;
		}
		
		public override string ToString()
		{
			return display;
		}
	}
	
	public static class AnonymousTypeFactory
	{
		public static AnonymousTypeFactory<T> New<T>(T def)
		{
			return new AnonymousTypeFactory<T>(def);
		}
	}
	
	public interface IAnonymousTypeFactory
	{
		Type AnonymousType{get;}
		object DefaultValue{get;}
		object Create(params object[] args);
		bool Test<T1>(T1 instance);
	}
}
