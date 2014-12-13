using System;
using System.Globalization;
using System.Resources;

namespace IllidanS4.SharpUtils
{
	internal static class Resources
	{
		static ResourceManager manager = new ResourceManager("Microsoft.CSharp.RuntimeBinder.Errors", typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly);
		
		public static string GetString(string key)
		{
			return manager.GetString(key);
		}
		
		public static string GetString(string key, CultureInfo culture)
		{
			return manager.GetString(key, culture);
		}
	}
}
