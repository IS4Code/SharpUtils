using System;

namespace IllidanS4.SharpUtils.Patterns
{
	[AttributeUsage(AttributeTargets.GenericParameter)]
	public sealed class StaticTypeAttribute : Attribute
	{
		public StaticTypeAttribute()
		{
		}
	}
}
