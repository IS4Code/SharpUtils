using System;
using System.Reflection;

namespace IllidanS4.SharpUtils.Reflection.CSharp
{
	partial class LanguageType
	{
		partial class DynamicTypeDescription
		{
			partial class DynamicMethodInfo
			{
				private class DynamicMethodInfoReturnParameter : ParameterInfo
				{
					private readonly DynamicMethodInfo dynamicmethod;
					
					public DynamicMethodInfoReturnParameter(DynamicMethodInfo dynamicmethod)
					{
						this.dynamicmethod = dynamicmethod;
					}
					
					public override Type ParameterType{
						get{
							return typeof(object);
						}
					}
				
					public override bool IsDefined(Type attributeType, bool inherit)
					{
						return false;
					}
					
					public override object[] GetCustomAttributes(bool inherit)
					{
						return new object[0];
					}
					
					public override object[] GetCustomAttributes(Type attributeType, bool inherit)
					{
						return new object[0];
					}
					
					public override ParameterAttributes Attributes{
						get{
							return ParameterAttributes.Out;
						}
					}
					
					public override MemberInfo Member{
						get{
							return dynamicmethod;
						}
					}
				}
			}
		}
	}
}
