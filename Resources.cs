using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Resources;

namespace IllidanS4.SharpUtils
{
	internal static class Resources
	{
		static readonly ResourceManager manager = new ResourceManager("Microsoft.CSharp.RuntimeBinder.Errors", typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly);
		
		public static string GetString(string key)
		{
			return manager.GetString(key);
		}
		
		public static string GetString(string key, CultureInfo culture)
		{
			return manager.GetString(key, culture);
		}
		
		public static readonly AssemblyBuilder DynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("SharpUtilsDynamic"), AssemblyBuilderAccess.Run);
		public static readonly ModuleBuilder DynamicModule = DynamicAssembly.DefineDynamicModule("SharpUtilsDynamic.dll");
		private static int TypeId = 1;
		public static TypeBuilder DefineDynamicType(TypeAttributes attributes)
		{
			return DynamicModule.DefineType("@DynamicType"+(TypeId++), attributes);
		}
	}
}
