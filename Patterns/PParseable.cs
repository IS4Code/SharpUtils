using System;
using IllidanS4.SharpUtils.Metadata;

namespace IllidanS4.SharpUtils.Patterns
{
	[Pattern]
	public abstract class PParseable<[StaticType]T> : IParseable
	{
		public abstract T Parse(string s);
		
		object IParseable.Parse(string s)
		{
			return Parse(s);
		}
	}
}
