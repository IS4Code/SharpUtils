using System;
using System.Globalization;

namespace IllidanS4.SharpUtils.Reflection.CSharp
{
	partial class LanguageType
	{
		private class NullTypeDescription : LanguageType
		{
			public override string LocalizedName{
				get{ return DynamicResources.GetString("NULL", CultureInfo.CurrentCulture); }
			}
			
			public override string Name{
				get{ return DynamicResources.GetString("NULL", CultureInfo.InvariantCulture); }
			}
			
			protected override bool IsPrimitiveImpl()
			{
				return true;
			}
		}
	}
}
