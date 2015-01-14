/* Date: 1.12.2014, Time: 21:03 */
using System;

namespace IllidanS4.SharpUtils.Reflection.Cpp
{
	/// <summary>
	/// Managed array type.
	/// </summary>
	public class CppManagedArrayType : CppDerivedType
	{
		public int Rank{get; private set;}
		
		public CppManagedArrayType(CppType type) : this(type, 1)
		{
			
		}
		
		public CppManagedArrayType(CppType type, int rank) : base(type, "array")
		{
			Rank = rank;
		}
		
		private string AddPrefix(string name)
		{
			if(Rank == 1)
			{
				return "array<"+name+">";
			}else{
				return "array<"+name+","+Rank+">";
			}
		}
		
		public override string Name{
			get{
				return AddPrefix(ElementType.Name);
			}
		}
		
		public override string FullName{
			get{
				return AddPrefix(ElementType.FullName);
			}
		}
		
		public override string ToString()
		{
			return AddPrefix(ElementType.ToString());
		}

		
		public override Type ManagedType{
			get{
				return null;
			}
		}
		
		public override Type DataType{
			get{
				if(Rank == 1)
				{
					return ElementType.ManagedType.MakeArrayType();
				}else{
					return ElementType.ManagedType.MakeArrayType(Rank);
				}
			}
		}
		
		public override object[] GetTemplateArguments()
		{
			return new object[]{ElementType, Rank};
		}
	}
}
