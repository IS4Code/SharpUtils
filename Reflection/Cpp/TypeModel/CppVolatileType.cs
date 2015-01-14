/* Date: 4.12.2014, Time: 19:16 */
using System;

namespace IllidanS4.SharpUtils.Reflection.Cpp
{
	/// <summary>
	/// Type qualified as "volatile".
	/// </summary>
	public class CppVolatileType : CppQualifiedType
	{
		public CppVolatileType(CppType elementType) : base(elementType, "volatile", CustomTypeModifier.IsVolatile)
		{
			
		}
	}
}
