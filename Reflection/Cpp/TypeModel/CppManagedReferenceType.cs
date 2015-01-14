/* Date: 1.12.2014, Time: 21:05 */
using System;

namespace IllidanS4.SharpUtils.Reflection.Cpp
{
	/// <summary>
	/// A managed reference to a variable.
	/// </summary>
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
