using System;

namespace IllidanS4.SharpUtils.Templates
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public sealed class TemplateAttribute : Attribute
	{
		public TemplateAttribute()
		{
			
		}
	}
}
