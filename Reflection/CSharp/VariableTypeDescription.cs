using System;
using System.Globalization;

namespace IllidanS4.SharpUtils.Reflection
{
	partial class LanguageType
	{
		private class VariableTypeDescription : LanguageType
		{
			public override string LocalizedName{
				get{ return Resources.GetString("SK_VARIABLE", CultureInfo.CurrentCulture); }
			}
			
			public override string Name{
				get{ return Resources.GetString("SK_VARIABLE", CultureInfo.InvariantCulture); }
			}
		}
	}
}
