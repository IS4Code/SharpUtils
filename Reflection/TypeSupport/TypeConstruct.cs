/* Date: 3.12.2014, Time: 16:07 */
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
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
		
		public TypeConstruct(Type elementType, Type delegatingType) : base(delegatingType ?? typeof(object))
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
		
		protected virtual Instance GetInstance(object target)
		{
			return new Instance(target);
		}
		
		public virtual object InvokeMethod(MethodBase method, object target, object[] args)
		{
			string name = method.Name;
			const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod;
			var inst = GetInstance(target);
			return ((object)inst).GetType().InvokeMember(name, flags, null, inst, args);
		}
		
		protected class Instance
		{
			public object Object{get; private set;}
			public VirtualObjectProxy Proxy{get; private set;}
			
			public Instance(object obj)
			{
				Object = obj;
				Proxy = RemotingServices.GetRealProxy(obj) as VirtualObjectProxy;
			}
			
			public override string ToString()
			{
				return Object.GetType().ToString();
			}
			
			public override bool Equals(object obj)
			{
				return RuntimeHelpers.Equals(Object, obj);
			}
			
			public override int GetHashCode()
			{
				return RuntimeHelpers.GetHashCode(Object);
			}
			
			public new Type GetType()
			{
				if(Proxy != null)
				{
					return Proxy.VirtualType;
				}else{
					return Object.GetType();
				}
			}
			public new object MemberwiseClone()
			{
				return ((VirtualObjectProxy)Proxy.Clone()).GetTransparentProxy();
			}
			public void FieldSetter(string typeName, string fieldName, object val)
			{
				Proxy.Fields[Proxy.Fields[fieldName]] = val;
			}
			public void FieldGetter(string typeName, string fieldName, ref object val)
			{
				val = Proxy.Fields[Proxy.Fields[fieldName]];
			}
			public FieldInfo GetFieldInfo(string typeName, string fieldName)
			{
				return Proxy.Fields[fieldName];
			}
		}
	}
}
