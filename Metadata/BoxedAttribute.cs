/* Date: 27.12.2014, Time: 13:47 */
using System;

namespace IllidanS4.SharpUtils.Metadata
{
	/// <summary>
	/// Specifies a boxed value type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class BoxedAttribute : Attribute
	{
		public Type BoxedType{
			get; private set;
		}
		
		public BoxedAttribute(Type valueType)
		{
			BoxedType = valueType;
		}
	}
}
