/* Date: 1.12.2014, Time: 21:03 */
using System;
using System.Reflection;

namespace IllidanS4.SharpUtils.Reflection.Cpp
{
	public abstract class CppType
	{
		public bool HasElementType{
			get{
				return HasElementTypeImpl;
			}
		}
		
		internal virtual bool HasElementTypeImpl{
			get{
				return false;
			}
		}
		
		public virtual string Name{
			get{
				return FullName;
			}
		}
		public abstract string FullName{
			get;
		}
		
		public abstract string Namespace{
			get;
		}
		
		public abstract CppEnvironment Environment{
			get;
		}
		
		public CppType()
		{
			
		}
		
		public CppType GetElementType()
		{
			return GetElementTypeImpl();
		}
		
		internal virtual CppType GetElementTypeImpl()
		{
			throw new NotImplementedException();
		}
		
		public override string ToString()
		{
			return FullName;
		}
		
		public CppManagedArrayType MakeManagedArrayType()
		{
			return new CppManagedArrayType(this);
		}
		
		public CppManagedArrayType MakeManagedArrayType(int rank)
		{
			return new CppManagedArrayType(this, rank);
		}
		
		public CppHandleType MakeHandleType()
		{
			return new CppHandleType(this);
		}
		
		public CppManagedReferenceType MakeManagedReferenceType()
		{
			return new CppManagedReferenceType(this);
		}
		
		public CppPointerType MakePointerType()
		{
			return new CppPointerType(this);
		}
		
		public CppFixedArrayType MakeFixedArrayType()
		{
			return new CppFixedArrayType(this);
		}
		
		public CppFixedArrayType MakeFixedArrayType(int size)
		{
			return new CppFixedArrayType(this, size);
		}
		
		public CppUnmanagedReferenceType MakeUnmanagedReferenceType()
		{
			return new CppUnmanagedReferenceType(this);
		}
		
		public CppVolatileType MakeVolatileType()
		{
			return new CppVolatileType(this);
		}
		
		public CppConstType MakeConstType()
		{
			return new CppConstType(this);
		}
		
		public static CppType Create(Type type)
		{
			Type elemType = type.GetElementType();
			if(type.IsArray)
			{
				return new CppHandleType(new CppManagedArrayType(Create(elemType), type.GetArrayRank()));
			}else if(type.IsPointer)
			{
				return new CppPointerType(Create(elemType));
			}else if(type.IsByRef)
			{
				return new CppManagedReferenceType(Create(elemType));
			}else if(!type.IsValueType)
			{
				return new CppHandleType(new CppDataType(type));
			}else{
				return new CppDataType(type);
			}
		}
		
		public abstract Type ManagedType{
			get;
		}
		
		public virtual Type DataType{
			get{
				return ManagedType;
			}
		}
		
		public virtual object[] GetTemplateArguments()
		{
			return null;
		}
	}
}
