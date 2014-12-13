/* Date: 26.11.2014, Time: 21:04 */
using System;
using System.Reflection;

namespace IllidanS4.SharpUtils.Reflection.Emit
{
	[AttributeUsage(AttributeTargets.Method)]
	public class MethodTargetAttribute : Attribute
	{
		public string TargetName{get; private set;}
		public Type ReturnType{get; private set;}
		public Type[] ParameterTypes{get; private set;}
		
		public MethodTargetAttribute() : this(typeof(void))
		{
			
		}
		
		public MethodTargetAttribute(Type returnType) : this(returnType, Type.EmptyTypes)
		{
			
		}
		
		public MethodTargetAttribute(Type returnType, params Type[] parameterTypes) : this(null, returnType, parameterTypes)
		{
			
		}
		
		public MethodTargetAttribute(string target, Type returnType, params Type[] parameterTypes)
		{
			TargetName = target;
			ReturnType = returnType;
			ParameterTypes = parameterTypes;
		}
	}
}
