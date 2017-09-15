using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Resources;

namespace IllidanS4.SharpUtils
{
	/// <summary>
	/// Various tools for dynamic types.
	/// </summary>
	public static class DynamicResources
	{
		static readonly ResourceManager manager = new ResourceManager("Microsoft.CSharp.RuntimeBinder.Errors", typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly);
		
		/// <summary>
		/// Returns a resource string from the Microsoft.CSharp.RuntimeBinder.Errors resource.
		/// </summary>
		/// <param name="key">The key of the resource.</param>
		/// <returns>The resource text.</returns>
		public static string GetString(string key)
		{
			return manager.GetString(key);
		}
		
		/// <summary>
		/// Returns a resource string from the Microsoft.CSharp.RuntimeBinder.Errors resource.
		/// </summary>
		/// <param name="key">The key of the resource.</param>
		/// <param name="culture">The specific culture of the resource.</param>
		/// <returns>The resource text.</returns>
		public static string GetString(string key, CultureInfo culture)
		{
			return manager.GetString(key, culture);
		}
		
		/// <summary>
		/// An assembly to create dynamic types in.
		/// </summary>
		public static readonly AssemblyBuilder DynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("SharpUtilsDynamic"), AssemblyBuilderAccess.Run);
		/// <summary>
		/// A module to create dynamic types in.
		/// </summary>
		public static readonly ModuleBuilder DynamicModule = DynamicAssembly.DefineDynamicModule("SharpUtilsDynamic.dll");
		private static int TypeId = 1;
		/// <summary>
		/// Defines a new anonymous dynamic type.
		/// </summary>
		/// <param name="attributes">The type attributes.</param>
		/// <returns>The type builder.</returns>
		public static TypeBuilder DefineDynamicType(TypeAttributes attributes)
		{
			return DynamicModule.DefineType("@DynamicType"+(TypeId++), attributes);
		}
	}
}
