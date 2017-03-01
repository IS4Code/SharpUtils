/* Date: 10.2.2017, Time: 19:53 */
using System;
using System.Reflection;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Interop;

namespace IllidanS4.SharpUtils.Reflection.Emit
{
	public class ParameterDefinition
	{
		public string Name{get; private set;}
		public ParameterAttributes Attributes{get; private set;}
		public object DefaultValue{get; private set;}
		public MarshalInfo Marshal{get; private set;}
		
		public ParameterDefinition(string name, ParameterAttributes attributes, object defaultValue, MarshalInfo marshal)
		{
			Name = name;
			Attributes = attributes;
			DefaultValue = defaultValue;
			Marshal = marshal;
		}
		
		public void Define(MethodBuilder method, int position)
		{
			var param = method.DefineParameter(position, Attributes, Name);
			if(param.IsOptional)
			{
				param.SetConstant(DefaultValue);
			}
			if(Marshal != null)
			{
				Marshal.Apply(param);
			}
		}
	}
}
