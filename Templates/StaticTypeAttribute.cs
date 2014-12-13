using System;

namespace IllidanS4.SharpUtils.Templates
{
	[AttributeUsage(AttributeTargets.GenericParameter)]
	public sealed class StaticTypeAttribute : Attribute
	{
		public StaticTypeAttribute()
		{
		}
	}
}
