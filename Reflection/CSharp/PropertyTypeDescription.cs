using System;
using System.Globalization;

namespace IllidanS4.SharpUtils.Reflection
{
	partial class LanguageType
	{
		private class PropertyTypeDescription : LanguageType
		{
			public override string LocalizedName{
				get{ return Resources.GetString("SK_PROPERTY", CultureInfo.CurrentCulture); }
			}
			
			public override string Name{
				get{ return Resources.GetString("SK_PROPERTY", CultureInfo.InvariantCulture); }
			}
		}
	}
}
