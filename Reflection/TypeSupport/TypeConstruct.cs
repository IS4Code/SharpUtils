/* Date: 3.12.2014, Time: 16:07 */
using System;
using System.Reflection;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection.TypeSupport
{
	/// <summary>
	/// Base Type class supporting creating derived types.
	/// </summary>
	public abstract class TypeConstruct : TypeDelegator, ISignatureElement
	{
		public Type ElementType{get; private set;}
		public abstract CorElementType CorElementType{get;}
		
		public TypeConstruct(Type delegatingType) : this(null, delegatingType)
		{
			
		}
		
		public TypeConstruct(Type elementType, Type delegatingType) : base(delegatingType)
		{
			ElementType = elementType;
		}
		
		protected override bool HasElementTypeImpl()
		{
			return ElementType != null;
		}
		
		public override Type GetElementType()
		{
			return ElementType;
		}
		
		public override Type MakeArrayType()
		{
			return new ArrayType(this);
		}
		
		public override Type MakeArrayType(int rank)
		{
			return new ArrayType(this, rank);
		}
		
		public override Type MakeByRefType()
		{
			return new ByRefType(this);
		}
		
		public override Type MakePointerType()
		{
			return new PointerType(this);
		}
		
		public override Type MakeGenericType(params Type[] typeArguments)
		{
			return new GenericType(this, typeArguments);
		}
		
		public override string Name{
			get{
				if(ElementType != null)
					return ElementType.Name;
				else
					return base.Name;
			}
		}
		
		public override string Namespace{
			get{
				if(ElementType != null)
					return ElementType.Namespace;
				else
					return base.Namespace;
			}
		}
		
		public override string FullName{
			get{
				if(ElementType != null)
					return ElementType.FullName;
				else
					return base.FullName;
			}
		}
		
		public override string ToString()
		{
			if(ElementType != null)
				return ElementType.ToString();
			else
				return base.ToString();
		}
		
		protected abstract void AddSignature(SignatureHelper signature);
		
		void ISignatureElement.AddSignature(SignatureHelper signature)
		{
			this.AddSignature(signature);
		}
	}
}
