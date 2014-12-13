using System;

namespace IllidanS4.SharpUtils.Metadata
{
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	[CLSCompliant(true)]
	public sealed class DataAttribute : Attribute
	{
		public object[] Data{get; private set;}
		
		public DataAttribute(params object[] args)
		{
			Data = args;
		}
		
		public DataAttribute(object data) : this(new[]{data})
		{
			
		}
		
		public DataAttribute(object data1, object data2) : this(new[]{data1, data2})
		{
			
		}
		
		public DataAttribute(object data1, object data2, object data3) : this(new[]{data1, data2, data3})
		{
			
		}
		
		public DataAttribute(object data1, object data2, object data3, object data4) : this(new[]{data1, data2, data3, data4})
		{
			
		}
	}
}
