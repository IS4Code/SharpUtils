using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;

namespace IllidanS4.SharpUtils.Reflection
{
	partial class LanguageType
	{
		partial class DynamicTypeDescription
		{
			public class DynamicPropertyInfo : PropertyInfo
			{
				private readonly string name;
				private readonly BindingFlags bindingAttr;
				private readonly DynamicTypeDescription dynamictype;
				private readonly GetMemberBinder dbinder;
				private readonly Dictionary<object, Func<object>> cache = new Dictionary<object, Func<object>>();
				
				public override string Name{
					get{
						return name;
					}
				}
				
				public DynamicPropertyInfo(string name, BindingFlags bindingAttr, DynamicTypeDescription dynamictype)
				{
					this.name = name;
					this.bindingAttr = bindingAttr;
					this.dynamictype = dynamictype;
					this.dbinder = (GetMemberBinder)Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, name, TypeOf<DynamicPropertyInfo>.TypeID, new CSharpArgumentInfo[]{CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)});
				}
				
				public override bool CanRead{
					get{
						return true;
					}
				}
				
				public override bool CanWrite{
					get{
						return true;
					}
				}
				
				public override Type DeclaringType{
					get{
						return dynamictype;
					}
				}
				
				public override PropertyAttributes Attributes{
					get{
						return PropertyAttributes.None;
					}
				}
				
				public override object GetValue(object obj, BindingFlags invokeAttr, System.Reflection.Binder binder, object[] index, System.Globalization.CultureInfo culture)
				{
					Func<object> del;
					if(cache.TryGetValue(obj, out del))
					{
						return del.Invoke();
					}
					IDynamicMetaObjectProvider provider = obj as IDynamicMetaObjectProvider;
					DynamicMetaObject mobj;
					if(provider != null)
					{
						mobj = provider.GetMetaObject(Expression.Constant(obj));
					}else{
						mobj = new DynamicMetaObject(Expression.Constant(obj), BindingRestrictions.Empty, obj);
					}
					DynamicMetaObject ret = mobj.BindGetMember(dbinder);
					BlockExpression final = Expression.Block(
						Expression.Label(CallSiteBinder.UpdateLabel),
						ret.Expression
					);
					Expression<Func<object>> lambda = Expression.Lambda<Func<object>>(final);
					del = lambda.Compile();
					cache[obj] = del;
					return del.Invoke();
				}
				
				public override ParameterInfo[] GetIndexParameters()
				{
					throw new NotImplementedException();
				}
				
				public override MethodInfo GetSetMethod(bool nonPublic)
				{
					throw new NotImplementedException();
				}
				
				public override MethodInfo GetGetMethod(bool nonPublic)
				{
					throw new NotImplementedException();
				}
				
				public override MethodInfo[] GetAccessors(bool nonPublic)
				{
					throw new NotImplementedException();
				}
				
				public override void SetValue(object obj, object value, BindingFlags invokeAttr, System.Reflection.Binder binder, object[] index, System.Globalization.CultureInfo culture)
				{
					throw new NotImplementedException();
				}
				
				public override Type PropertyType{
					get{
						throw new NotImplementedException();
					}
				}
				
				public override bool IsDefined(Type attributeType, bool inherit)
				{
					return false;
				}
				
				public override object[] GetCustomAttributes(Type attributeType, bool inherit)
				{
					return Empty<object[]>.Value;
				}
				
				public override object[] GetCustomAttributes(bool inherit)
				{
					return Empty<object[]>.Value;
				}
				
				public override Type ReflectedType{
					get{
						return dynamictype;
					}
				}
			}
		}
	}
}
