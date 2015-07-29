/* Date: 25.7.2015, Time: 14:07 */
using System;
using System.Reflection;
using IllidanS4.SharpUtils.Accessing;
using IllidanS4.SharpUtils.Interop;

namespace IllidanS4.SharpUtils.Reflection
{
	public struct TypedContainer : IReadAccessor, IEquatable<TypedContainer>
	{
		private Type _type;
		public Type Type{
			get{
				if(_type == null) _type = TypeOf<object>.TypeID;
				return _type;
			}
			private set{
				_type = value;
			}
		}
		public object Value{get; private set;}
		private TempValueRef valueRef;
		
		public TypedContainer(SafeReference sr) : this()
		{
			Type = sr.Type;
			Value = sr.Value;
		}
		
		[CLSCompliant(false)]
		public TypedContainer(TypedReference tr) : this()
		{
			Type = TypedReference.GetTargetType(tr);
			Value = TypedReference.ToObject(tr);
		}
		
		private TypedContainer(object value, Type type) : this()
		{
			Type = type;
			Value = value;
		}
		
		public static TypedContainer Create<T>(T value)
		{
			return new TypedContainer(value, TypeOf<T>.TypeID);
		}
		
		public static TypedContainer Create(object value)
		{
			if(value == null) return default(TypedContainer);
			return new TypedContainer(value, value.GetType());
		}
		
		private void SetValueRef()
		{
			if(valueRef == null)
				valueRef = (TempValueRef)Activator.CreateInstance(typeof(TempValueRef<>).MakeGenericType(Type));
		}
		
		object IReadAccessor.Item{
			get{
				return Value;
			}
		}
		
		public bool Equals(TypedContainer other)
		{
			return Object.Equals(Type, other.Type) && Object.Equals(Value, other.Value);
		}
		
		public override bool Equals(object obj)
		{
			return (obj is TypedContainer) && Equals((TypedContainer)obj);
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (Type != null)
					hashCode += 1000000007 * Type.GetHashCode();
				if (Value != null)
					hashCode += 1000000009 * Value.GetHashCode();
			}
			return hashCode;
		}
		
		public static bool operator ==(TypedContainer lhs, TypedContainer rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(TypedContainer lhs, TypedContainer rhs)
		{
			return !(lhs == rhs);
		}
		
		
		public T GetValue<T>()
		{
			return (T)Value;
		}
		
		[CLSCompliant(false)]
		public void GetTempReference(TypedReferenceTools.TypedRefAction act)
		{
			GetTempReference<Unit>(tr => {act(tr); return 0;});
		}
		
		[CLSCompliant(false)]
		public TRet GetTempReference<TRet>(TypedReferenceTools.TypedRefFunc<TRet> func)
		{
			SetValueRef();
			return valueRef.GetTempReference(this, func);
		}
		
		public void GetTempReference(Action<SafeReference> act)
		{
			GetTempReference(tr => SafeReference.Create(tr, act));
		}
		
		public TRet GetTempReference<TRet>(Func<SafeReference, TRet> func)
		{
			return GetTempReference(tr => SafeReference.Create(tr, func));
		}
		
		private abstract class TempValueRef
		{
			public abstract TRet GetTempReference<TRet>(TypedContainer cont, TypedReferenceTools.TypedRefFunc<TRet> func);
		}
		
		private class TempValueRef<T> : TempValueRef
		{
			public override TRet GetTempReference<TRet>(TypedContainer cont, TypedReferenceTools.TypedRefFunc<TRet> func)
			{
				T val = cont.GetValue<T>();
				return TypedReferenceTools.GetTypedReference(ref val, func);
			}
		}
	}
}
