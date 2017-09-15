using System;
using System.Globalization;

namespace IllidanS4.SharpUtils.Reflection.CSharp
{
	partial class LanguageType
	{
		private class AnonymousMethodTypeDescription : LanguageType
		{
			public override string LocalizedName{
				get{ return DynamicResources.GetString("AnonMethod", CultureInfo.CurrentCulture); }
			}
			
			public override string Name{
				get{ return DynamicResources.GetString("AnonMethod", CultureInfo.InvariantCulture); }
			}
		}
	}
}
