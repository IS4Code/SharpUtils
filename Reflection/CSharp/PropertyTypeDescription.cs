using System;
using System.Globalization;

namespace IllidanS4.SharpUtils.Reflection.CSharp
{
	partial class LanguageType
	{
		private class PropertyTypeDescription : LanguageType
		{
			public override string LocalizedName{
				get{ return DynamicResources.GetString("SK_PROPERTY", CultureInfo.CurrentCulture); }
			}
			
			public override string Name{
				get{ return DynamicResources.GetString("SK_PROPERTY", CultureInfo.InvariantCulture); }
			}
		}
	}
}
