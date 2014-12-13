using System;
using System.Dynamic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;

namespace IllidanS4.SharpUtils.Templates
{
	partial class Template
	{
		static readonly Type typeofIDynamicMetaObjectProvider = TypeOf<IDynamicMetaObjectProvider>.TypeID;
		static readonly Type typeofDynamicMetaObject = TypeOf<DynamicMetaObject>.TypeID;
		static readonly Type typeofTemplateDynamicMetaObject = TypeOf<TemplateDynamicMetaObject>.TypeID;
		static readonly Type typeofExpression = TypeOf<Expression>.TypeID;
		static readonly Type[] m_IDynamicMetaObjectProvider_GetMetaObject_parameters = new Type[]{typeofExpression};
		static readonly Type[] c_TemplateDynamicMetaObject_parameters = new Type[]{typeofExpression, typeofITemplate};
		static readonly MethodInfo m_IDynamicMetaObjectProvider_GetMetaObject = typeofIDynamicMetaObjectProvider.GetMethod("GetMetaObject");
		static readonly ConstructorInfo c_TemplateDynamicMetaObject = typeofTemplateDynamicMetaObject.GetConstructor(c_TemplateDynamicMetaObject_parameters);
		
		public sealed class TemplateDynamicMetaObject : DynamicMetaObject
		{
			readonly Type m_class;
			//readonly Expression m_class_expression;
			
			public TemplateDynamicMetaObject(Expression expression, ITemplate value) : base(expression, BindingRestrictions.Empty, value)
			{
				m_class = value.Class;
			}
			
			public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
			{
				DynamicMetaObject getMember = new DynamicMetaObject(
					Expression.Convert(
						StaticPropertyOrField(m_class, binder.Name),
						binder.ReturnType
					),
					BindingRestrictions.GetTypeRestriction(Expression, LimitType)
				);
				return getMember;
			}
			
			public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
			{
				DynamicMetaObject setMember = new DynamicMetaObject(
					Expression.Assign(
						StaticPropertyOrField(m_class, binder.Name),
						value.Expression
					),
					BindingRestrictions.GetTypeRestriction(Expression, LimitType)
				);
				return setMember;
			}
			
			public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
			{
				DynamicMetaObject invokeMember = new DynamicMetaObject(
					Expression.Convert(
						Expression.Call(
							m_class.GetMethod(
								binder.Name,
								(from arg in args select arg.LimitType).ToArray()),
							from arg in args select arg.Expression
						),
						binder.ReturnType
					),
					BindingRestrictions.GetTypeRestriction(Expression, LimitType)
				);
				return invokeMember;
			}
			
	        private static MemberExpression StaticPropertyOrField(Type type, string propertyOrFieldName)
	        {
	        	if(type == null) throw new ArgumentNullException("type");
	            PropertyInfo property = type.GetProperty(propertyOrFieldName, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Static);
	            if(property != null)
	            {
	                return Expression.Property(null, property);
	            }
	            FieldInfo field = type.GetField(propertyOrFieldName, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Static);
	            if(field == null)
	            {
	                property = type.GetProperty(propertyOrFieldName, BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Static);
	                if(property != null)
	                {
	                    return Expression.Property(null, property);
	                }
	                field = type.GetField(propertyOrFieldName, BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Static);
	                if(field == null)
	                {
	                    throw new ArgumentException(string.Format("{0} NotAMemberOfType {1}",propertyOrFieldName, type));
	                }
	            }
	            return Expression.Field(null, field);
	        }
		}
	}
}
