using System;
using IllidanS4.SharpUtils.Metadata;

namespace IllidanS4.SharpUtils.Templates
{
	[Template]
	public abstract class TParseable<[StaticType]T> : IParseable
	{
		public abstract T Parse(string s);
		
		object IParseable.Parse(string s)
		{
			return Parse(s);
		}
	}
}
