/* Date: 4.12.2014, Time: 19:15 */
using System;

namespace IllidanS4.SharpUtils.Reflection.Cpp
{
	public class CppConstType : CppQualifiedType
	{
		public CppConstType(CppType elementType) : base(elementType, "const", CustomTypeModifier.IsConst)
		{
			
		}
	}
}
