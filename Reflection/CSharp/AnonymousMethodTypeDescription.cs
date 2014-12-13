using System;
using System.Globalization;

namespace IllidanS4.SharpUtils.Reflection
{
	partial class LanguageType
	{
		private class AnonymousMethodTypeDescription : LanguageType
		{
			public override string LocalizedName{
				get{ return Resources.GetString("AnonMethod", CultureInfo.CurrentCulture); }
			}
			
			public override string Name{
				get{ return Resources.GetString("AnonMethod", CultureInfo.InvariantCulture); }
			}
		}
	}
}
