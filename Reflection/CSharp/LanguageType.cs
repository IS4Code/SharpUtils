using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using IllidanS4.SharpUtils.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.TypeSupport;
using Microsoft.CSharp.RuntimeBinder;

namespace IllidanS4.SharpUtils.Reflection.CSharp
{
	public abstract partial class LanguageType : TypeConstruct
	{
		public static readonly LanguageType Dynamic = new DynamicTypeDescription();
		public static readonly LanguageType Null = new NullTypeDescription();
		public static readonly LanguageType MethodGroup = new MethodGroupTypeDescription();
		public static readonly LanguageType AnonymousMethod = new AnonymousMethodTypeDescription();
		public static readonly LanguageType LambdaExpression = new LambdaExpressionTypeDescription();
		public static readonly LanguageType AnonymousType = new AnonymousTypeTypeDescription();
		public static readonly LanguageType Type = new TypeTypeDescription();
		public static readonly LanguageType Method = new MethodTypeDescription();
		public static readonly LanguageType Field = new FieldTypeDescription();
		public static readonly LanguageType Property = new PropertyTypeDescription();
		public static readonly LanguageType Event = new EventTypeDescription();
		public static readonly LanguageType TypeParameter = new TypeParameterTypeDescription();
		public static readonly LanguageType Variable = new VariableTypeDescription();

		public LanguageType() : base(typeof(void*))
		{
			
		}
		
		public LanguageType(Type underlyingType) : base(underlyingType)
		{
			
		}
		
		protected override void AddSignature(SignatureHelper signature)
		{
			throw new NotImplementedException();
		}
			
		public override CorElementType CorElementType{
			get{
				throw new NotImplementedException();
			}
		}
		
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return false;
		}
		
		public override object[] GetCustomAttributes(bool inherit)
		{
			return new object[0];
		}
		
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return new object[0];
		}
		
		public virtual string LocalizedName{
			get{
				return Name;
			}
		}
		
		public override Type UnderlyingSystemType{
			get{
				return null;
			}
		}
		
		public override RuntimeTypeHandle TypeHandle{
			get{
				throw new NotImplementedException();
			}
		}
		
		protected override bool HasElementTypeImpl()
		{
			return false;
		}
		
		public override Type GetElementType()
		{
			return null;
		}
		
		protected override bool IsCOMObjectImpl()
		{
			return false;
		}
		
		protected override bool IsPrimitiveImpl()
		{
			return false;
		}
		
		protected override bool IsPointerImpl()
		{
			return false;
		}
		
		protected override bool IsByRefImpl()
		{
			return false;
		}
		
		protected override bool IsArrayImpl()
		{
			return false;
		}
		
		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			return 0;
		}
		
		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			return new MemberInfo[0];
		}
		
		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			return null;
		}
		
		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			return new Type[0];
		}
		
		public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
		{
			return new PropertyInfo[0];
		}
		
		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, System.Reflection.Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			return null;
		}
		
		public override EventInfo[] GetEvents()
		{
			return new EventInfo[0];
		}
		
		public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
		{
			return null;
		}
		
		public override EventInfo[] GetEvents(BindingFlags bindingAttr)
		{
			return new EventInfo[0];
		}
		
		public override Type[] GetInterfaces()
		{
			return new Type[0];
		}
		
		public override Type GetInterface(string name, bool ignoreCase)
		{
			return null;
		}
		
		public override FieldInfo[] GetFields(BindingFlags bindingAttr)
		{
			return new FieldInfo[0];
		}
		
		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			return null;
		}
		
		public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
		{
			return new MethodInfo[0];
		}
		
		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, System.Reflection.Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return null;
		}
		
		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			return new ConstructorInfo[0];
		}
		
		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, System.Reflection.Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return null;
		}
		
		public override Type BaseType{
			get{
				return null;
			}
		}
		
		public override string AssemblyQualifiedName{
			get{
				return Name;
			}
		}
		
		public override string Namespace{
			get{
				return String.Empty;
			}
		}
		
		public override string FullName{
			get{
				return Name;
			}
		}
		
		public override Assembly Assembly{
			get{
				return null;
			}
		}
		
		public override Module Module{
			get{
				return null;
			}
		}
		
		public override object InvokeMember(string name, BindingFlags invokeAttr, System.Reflection.Binder binder, object target, object[] args, ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters)
		{
			throw new NotImplementedException();
		}
		
		public override Guid GUID{
			get{
				return Guid.Empty;
			}
		}
		
		public override string ToString()
		{
			return Name;
		}
		
		public override bool Equals(object o)
		{
			if(o is Type)
			{
				return Equals((Type)o);
			}
			return false;
		}
		
		public override bool Equals(Type o)
		{
			if(o==null)return false;
			return this.GetType() == o.GetType();
		}
		
		public override int GetHashCode()
		{
			return this.GetType().GetHashCode();
		}
		
		public static bool operator !=(LanguageType lhs, LanguageType rhs)
		{
			return !(lhs == rhs);
		}

		public static bool operator ==(LanguageType lhs, LanguageType rhs)
		{
			return lhs==null?rhs==null:rhs==null?false:lhs.GetType()==rhs.GetType();
		}
	}
}
