/* Date: 1.12.2014, Time: 21:03 */
using System;

namespace IllidanS4.SharpUtils.Reflection.Cpp
{
	public class CppPointerType : CppDerivedType
	{
		public CppPointerType(CppType type) : base(type, "*")
		{
			
		}
		
		public override Type ManagedType{
			get{
				return ElementType.ManagedType.MakePointerType();
			}
		}
	}
}
