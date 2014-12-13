using System;

namespace IllidanS4.SharpUtils.Metadata
{
	/// <summary>
	/// Use on members which have `unsafe` keyword. It is not needed if member type is a pointer type or method parameter has a pointer type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Constructor)]
	public sealed class UnsafeAttribute : Attribute
	{
		public UnsafeAttribute()
		{
			
		}
	}
}
