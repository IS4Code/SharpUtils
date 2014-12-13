using System;

namespace IllidanS4.SharpUtils.Metadata
{
	/// <summary>
	/// Return parameter can have this attribute only if the method always never-returns, therefore it throws an exception, closes the application or perfoms an evil long jump.
	/// </summary>
	[AttributeUsage(AttributeTargets.ReturnValue)]
	public sealed class NeverAttribute : Attribute
	{
		public NeverAttribute()
		{
			
		}
	}
}
