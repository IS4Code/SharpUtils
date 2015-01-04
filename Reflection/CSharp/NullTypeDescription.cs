using System;
using System.Globalization;

namespace IllidanS4.SharpUtils.Reflection.CSharp
{
	partial class LanguageType
	{
		private class NullTypeDescription : LanguageType
		{
			public override string LocalizedName{
				get{ return Resources.GetString("NULL", CultureInfo.CurrentCulture); }
			}
			
			public override string Name{
				get{ return Resources.GetString("NULL", CultureInfo.InvariantCulture); }
			}
			
			protected override bool IsPrimitiveImpl()
			{
				return true;
			}
		}
	}
}
