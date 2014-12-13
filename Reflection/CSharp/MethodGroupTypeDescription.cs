using System;
using System.Globalization;

namespace IllidanS4.SharpUtils.Reflection
{
	partial class LanguageType
	{
		private class MethodGroupTypeDescription : LanguageType
		{
			public override string LocalizedName{
				get{ return Resources.GetString("MethodGroup", CultureInfo.CurrentCulture); }
			}
			
			public override string Name{
				get{ return Resources.GetString("MethodGroup", CultureInfo.InvariantCulture); }
			}
		}
	}
}
