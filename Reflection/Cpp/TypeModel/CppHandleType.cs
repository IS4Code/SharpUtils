/* Date: 1.12.2014, Time: 21:04 */
using System;

namespace IllidanS4.SharpUtils.Reflection.Cpp
{
	/// <summary>
	/// Description of CppHandleType.
	/// </summary>
	public class CppHandleType : CppDerivedType
	{
		public CppHandleType(CppType type) : base(type, "^")
		{
			
		}
		
		public override Type ManagedType{
			get{
				var type = DataType;
				if(type.IsValueType)
				{
					return new ModifiedType(typeof(ValueType), new CustomTypeModifier(type), new CustomTypeModifier(CustomTypeModifier.IsBoxed));
				}else{
					return type;
				}
			}
		}
		
		public override Type DataType{
			get{
				return ElementType.DataType;
			}
		}
	}
}
