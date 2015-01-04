/* Date: 3.12.2014, Time: 16:04 */
using System;
using System.Reflection;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection.TypeSupport
{
	public class ArrayType : TypeAppendConstruct
	{
		private readonly int rank;
		private readonly bool md;
		
		public override CorElementType CorElementType{
			get{
				return md ? CorElementType.Array : CorElementType.SzArray;
			}
		}
		
		public ArrayType(Type elementType) : base(elementType, UnderlyingArrayType(elementType))
		{
			rank = 1;
		}
		
		public ArrayType(Type elementType, int rank) : base(elementType, UnderlyingArrayType(elementType, rank))
		{
			this.rank = rank;
			md = true;
		}
		
		public override int GetArrayRank()
		{
			return rank;
		}
		
		protected override string Append(string name)
		{
			return name+(md?(rank==1?"[*]":("["+new string(',', rank-1)+"]")):"[]");
		}
		
		private static Type UnderlyingArrayType(Type elementType)
		{
			if(elementType is FunctionPointerType)
			{
				return Types.Generated.FnPtrSZArray;
			}else{
				return elementType.UnderlyingSystemType.MakeArrayType();
			}
		}
		
		private static Type UnderlyingArrayType(Type elementType, int rank)
		{
			if(elementType is FunctionPointerType)
			{
				return Types.Generated.FnPtrMDArray[rank];
			}else{
				return elementType.UnderlyingSystemType.MakeArrayType(rank);
			}
		}
		
		protected override void AddSignature(SignatureHelper signature)
		{
			if(md)
			{
				signature.AddElementType(CorElementType.Array);
				signature.AddArgumentSignature(ElementType);
				signature.AddData(rank);
				signature.AddData(0);
				signature.AddData(0);
			}else{
				signature.AddElementType(CorElementType.SzArray);
				signature.AddArgumentSignature(ElementType);
			}
		}
	}
}
