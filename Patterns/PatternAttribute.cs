using System;

namespace IllidanS4.SharpUtils.Patterns
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public sealed class PatternAttribute : Attribute
	{
		public PatternAttribute()
		{
			
		}
	}
}
