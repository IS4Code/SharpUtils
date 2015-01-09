/* Date: 9.1.2015, Time: 14:54 */
using System;
using System.Reflection;

namespace IllidanS4.SharpUtils.Reflection
{
	/// <summary>
	/// Formal type representing a function pointer to a function with undefined signature.
	/// Defines deriving operations producing derived "(fnptr)" types
	/// due to the absence of an actual "(fnptr)" type defined in the environment.
	/// </summary>
	public class FunctionTypeDelegator : TypeDelegator
	{
		public FunctionTypeDelegator() : base(Types.Generated.FnPtr)
		{
			
		}
		
		public override Type MakeArrayType()
		{
			return Types.Generated.FnPtrSZArray;
		}
		
		public override Type MakeArrayType(int rank)
		{
			return Types.Generated.FnPtrMDArray[rank];
		}
		
		public override Type MakeByRefType()
		{
			return Types.Generated.FnPtrByRef;
		}
		
		public override Type MakePointerType()
		{
			return Types.Generated.FnPtrPointer;
		}
		
		public override string ToString()
		{
			return "(fnptr)";
		}
		
		public override string Name{
			get{
				return this.ToString();
			}
		}
		
		public override string FullName{
			get{
				return this.ToString();
			}
		}
	}
}
