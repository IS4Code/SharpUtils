using System;
using System.Globalization;

namespace IllidanS4.SharpUtils.Reflection.CSharp
{
	partial class LanguageType
	{
		private class TypeTypeDescription : LanguageType
		{
			public override string LocalizedName{
				get{ return DynamicResources.GetString("SK_CLASS", CultureInfo.CurrentCulture); }
			}
			
			public override string Name{
				get{ return DynamicResources.GetString("SK_CLASS", CultureInfo.InvariantCulture); }
			}
		}
	}
}
