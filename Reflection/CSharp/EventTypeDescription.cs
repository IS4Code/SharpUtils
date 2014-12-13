using System;
using System.Globalization;

namespace IllidanS4.SharpUtils.Reflection
{
	partial class LanguageType
	{
		private class EventTypeDescription : LanguageType
		{
			public override string LocalizedName{
				get{ return Resources.GetString("SK_EVENT", CultureInfo.CurrentCulture); }
			}
			
			public override string Name{
				get{ return Resources.GetString("SK_EVENT", CultureInfo.InvariantCulture); }
			}
		}
	}
}
