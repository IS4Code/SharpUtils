using System;
using System.Globalization;

namespace IllidanS4.SharpUtils.Reflection.CSharp
{
	partial class LanguageType
	{
		private class MethodGroupTypeDescription : LanguageType
		{
			public override string LocalizedName{
				get{ return DynamicResources.GetString("MethodGroup", CultureInfo.CurrentCulture); }
			}
			
			public override string Name{
				get{ return DynamicResources.GetString("MethodGroup", CultureInfo.InvariantCulture); }
			}
		}
	}
}
