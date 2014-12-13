/* Date: 1.12.2014, Time: 21:05 */
using System;

namespace IllidanS4.SharpUtils.Reflection.Cpp
{
	public class CppManagedReferenceType : CppDerivedType
	{
		public CppManagedReferenceType(CppType type) : base(type, "%")
		{
			
		}
		
		public override Type ManagedType{
			get{
				return ElementType.ManagedType.MakeByRefType();
			}
		}
	}
}
