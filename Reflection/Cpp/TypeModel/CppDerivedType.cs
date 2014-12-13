/* Date: 1.12.2014, Time: 21:09 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IllidanS4.SharpUtils.Reflection.Cpp
{
	public abstract class CppDerivedType : CppType
	{
		public CppType ElementType{get; private set;}
		public string Symbol{get; private set;}
		
		internal override bool HasElementTypeImpl{
			get{
				return true;
			}
		}
		
		public CppDerivedType(CppType inner, string symbol)
		{
			ElementType = inner;
			Symbol = symbol;
		}
		
		internal override CppType GetElementTypeImpl()
		{
			return ElementType;
		}
		
		private string GetFormat(FormatType type)
		{
			StringBuilder name = new StringBuilder();
			int pos = 0;
			StepInner(name, ref pos, type);
			name.Insert(pos, Symbol);
			return name.ToString();
		}
		
		private void StepInner(StringBuilder builder, ref int pos, FormatType type)
		{
			CppDerivedType derived = ElementType as CppDerivedType;
			if(derived != null)
			{
				derived.StepInner(builder, ref pos, type);
				CppFixedArrayType arr = derived as CppFixedArrayType;
				if(arr != null)
				{
					if(this is CppFixedArrayType)
					{
						builder.Insert(pos, arr.Symbol);
					}else{
						builder.Insert(pos, "(");
						pos += 1;
						builder.Insert(pos, ")"+arr.Symbol);
					}
				}else{
					builder.Insert(pos, derived.Symbol+" ");
					pos += derived.Symbol.Length+1;
				}
			}else{
				string name;
				switch(type)
				{
					case FormatType.Name:
						name = ElementType.Name;
						break;
					case FormatType.FullName:
						name = ElementType.FullName;
						break;
					default:
						name = ElementType.ToString();
						break;
				}
				builder.Insert(pos, name+" ");
				pos += name.Length+1;
			}
		}
		
		enum FormatType
		{
			Name, FullName, ToString
		}
		
		public override string Name{
			get{
				return GetFormat(FormatType.Name);
			}
		}
		
		public override string FullName{
			get{
				return GetFormat(FormatType.FullName);
			}
		}
		
		public override string ToString()
		{
			return GetFormat(FormatType.ToString);
		}
		
		public override string Namespace{
			get{
				return ElementType.Namespace;
			}
		}
		
		public override CppEnvironment Environment{
			get{
				return ElementType.Environment;
			}
		}
	}
}
