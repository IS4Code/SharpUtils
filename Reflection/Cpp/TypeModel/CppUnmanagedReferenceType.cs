/* Date: 1.12.2014, Time: 21:05 */
using System;

namespace IllidanS4.SharpUtils.Reflection.Cpp
{
	/// <summary>
	/// Description of CppUnmanagedReferenceType.
	/// </summary>
	public class CppUnmanagedReferenceType : CppDerivedType
	{
		public CppUnmanagedReferenceType(CppType type) : base(type, "&")
		{
			
		}
		
		public override Type ManagedType{
			get{
				return null;
			}
		}
	}
}
