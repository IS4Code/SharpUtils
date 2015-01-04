using System;

namespace IllidanS4.SharpUtils.Reflection.CSharp
{
	[Serializable]
	public class NamedArgument
	{
		public object Value{
			get; private set;
		}
		
		public string Name{
			get; private set;
		}
		
		public NamedArgument(string name, object value)
		{
			Name = name;
			Value = value;
		}
	}
}
