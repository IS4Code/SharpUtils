/* Date: 1.12.2014, Time: 21:03 */
using System;

namespace IllidanS4.SharpUtils.Reflection.Cpp
{
	public sealed class CppDataType : CppType
	{
		private readonly Type dataType;
		public override Type DataType{get{return dataType;}}
		
		public CppDataType(Type type)
		{
			dataType = type;
		}
		
		public override string Namespace{
			get{
				return DataType.Namespace.Replace(".", "::");
			}
		}
		
		public override string Name{
			get{
				return DataType.Name;
			}
		}
		
		public override string FullName{
			get{
				return Namespace+"::"+Name;
			}
		}
		
		public override Type ManagedType{
			get{
				if(DataType.IsValueType)
				{
					return DataType;
				}else{
					return new ModifiedType(DataType, new CustomTypeModifier(CustomTypeModifier.IsByValue, true));
				}
			}
		}
		
		public override CppEnvironment Environment{
			get{
				return new CppEnvironment(DataType.Assembly);
			}
		}
	}
}
