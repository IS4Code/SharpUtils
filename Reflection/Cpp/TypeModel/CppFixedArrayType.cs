/* Date: 1.12.2014, Time: 21:59 */
using System;

namespace IllidanS4.SharpUtils.Reflection.Cpp
{
	/// <summary>
	/// Unmanaged array type.
	/// </summary>
	public class CppFixedArrayType : CppDerivedType
	{
		public int Size{get; private set;}
		
		public CppFixedArrayType(CppType type) : this(type, 0)
		{
			
		}
		
		public CppFixedArrayType(CppType type, int size) : base(type, "["+size+"]")
		{
			Size = size;
		}
		
		public override Type ManagedType{
			get{
				return null;
			}
		}
	}
}
