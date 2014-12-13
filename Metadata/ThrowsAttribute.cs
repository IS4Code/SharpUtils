using System;
using System.Runtime.CompilerServices;

namespace IllidanS4.SharpUtils.Metadata
{
	/// <summary>
	/// Method which has this attribute could throw an exception.
	/// </summary>
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Delegate)]
	public sealed class ThrowsAttribute : Attribute
	{
		public Type ExceptionType{
			get; private set;
		}
		
		public Type InnerObjectType{
			get; private set;
		}
		
		public ThrowsAttribute(Type exceptionType)
		{
			if(!TypeOf<Exception>.TypeID.IsAssignableFrom(exceptionType))
			{
				//throw new ArgumentException("This type doesn't inherit System.Exception class.", "t");
				ExceptionType = TypeOf<RuntimeWrappedException>.TypeID;
				InnerObjectType = exceptionType;
			}else{
				ExceptionType = InnerObjectType = exceptionType;
			}
		}
		
		public ThrowsAttribute(string exceptionTypeName) : this(Type.GetType(exceptionTypeName, true, true))
		{
			
		}
	}
}
