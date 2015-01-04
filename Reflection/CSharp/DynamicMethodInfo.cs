using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;

namespace IllidanS4.SharpUtils.Reflection.CSharp
{
	partial class LanguageType
	{
		partial class DynamicTypeDescription
		{
			private partial class DynamicMethodInfo : MethodInfo
			{
				private readonly string name;
				private readonly BindingFlags bindingAttr;
				private readonly DynamicTypeDescription dynamictype;
				private readonly Dictionary<int,InvokeMemberBinder> cachebinder = new Dictionary<int,InvokeMemberBinder>();
				
				public DynamicMethodInfo(string name, BindingFlags bindingAttr, DynamicTypeDescription dynamictype)
				{
					this.name = name;
					this.bindingAttr = bindingAttr;
					this.dynamictype = dynamictype;
				}
				
				public override ICustomAttributeProvider ReturnTypeCustomAttributes{
					get{
						return null;
					}
				}
				
				public override MethodInfo GetBaseDefinition()
				{
					return this;
				}
				
				public override ParameterInfo[] GetParameters()
				{
					return new ParameterInfo[0];
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
				
				public override Type ReflectedType{
					get{
						return dynamictype;
					}
				}
				
				public override Type DeclaringType{
					get{
						return dynamictype;
					}
				}
				
				public override string Name{
					get{
						return name;
					}
				}
				
				public override object Invoke(object obj, BindingFlags invokeAttr, System.Reflection.Binder binder, object[] parameters, CultureInfo culture)
				{
					parameters = parameters??Empty<object[]>.Value;
					int args = parameters.Length;
					InvokeMemberBinder dbinder;
					bool named = false;
					foreach(object arg in parameters)
					{
						if(arg is NamedArgument)named = true;
						break;
					}
					if(!named)
					{
						if(!cachebinder.TryGetValue(args, out dbinder))
						{
							CSharpArgumentInfo[] csargs = new CSharpArgumentInfo[parameters.Length+1];
							csargs[0] = CSharpArgumentInfo.Create(0, null);
							for(int i = 0; i < args; i++)
							{
								csargs[i+1] = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant | CSharpArgumentInfoFlags.UseCompileTimeType, null);
							}
							dbinder = (InvokeMemberBinder)Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(
								0, name, null,
								TypeOf<DynamicMethodInfo>.TypeID, csargs
							);
							cachebinder.Add(args, dbinder);
						}
					}else{
						CSharpArgumentInfo[] csargs = new CSharpArgumentInfo[parameters.Length+1];
						csargs[0] = CSharpArgumentInfo.Create(0, null);
						for(int i = 0; i < args; i++)
						{
							NamedArgument narg = parameters[i] as NamedArgument;
							if(narg == null)
							{
								csargs[i+1] = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant | CSharpArgumentInfoFlags.UseCompileTimeType, null);
							}else{
								csargs[i+1] = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant | CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.NamedArgument, narg.Name);
							}
						}
						dbinder = (InvokeMemberBinder)Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(
							0, name, null,
							TypeOf<DynamicMethodInfo>.TypeID, csargs
						);
					}
					
					IDynamicMetaObjectProvider provider = obj as IDynamicMetaObjectProvider;
					DynamicMetaObject mobj;
					if(provider != null)
					{
						mobj = provider.GetMetaObject(Expression.Constant(obj));
					}else{
						mobj = new DynamicMetaObject(Expression.Constant(obj), BindingRestrictions.Empty, obj);
					}
					DynamicMetaObject[] margs = new DynamicMetaObject[parameters.Length];
					for(int i = 0; i < args; i++)
					{
						NamedArgument narg = parameters[i] as NamedArgument;
						if(narg == null)
						{
							margs[i] = new DynamicMetaObject(Expression.Constant(parameters[i]), BindingRestrictions.Empty, parameters[i]);
						}else{
							margs[i] = new DynamicMetaObject(Expression.Constant(narg.Value), BindingRestrictions.Empty, narg.Value);
						}
					}
					DynamicMetaObject ret = mobj.BindInvokeMember(dbinder, margs);
					BlockExpression final = Expression.Block(
						Expression.Label(CallSiteBinder.UpdateLabel),
						ret.Expression
					);
					Expression<Func<object>> lambda = Expression.Lambda<Func<object>>(final);
					Func<object> del = lambda.Compile();
					return del.Invoke();
				}
				
				public override Type ReturnType{
					get{
						return typeof(object);
					}
				}
				
				public override ParameterInfo ReturnParameter{
					get{
						return new DynamicMethodInfoReturnParameter(this);
					}
				}
				
				public override MethodAttributes Attributes{
					get{
						return MethodAttributes.Public | MethodAttributes.Virtual;
					}
				}
				
				public override RuntimeMethodHandle MethodHandle{
					get{
						return default(RuntimeMethodHandle);
					}
				}
				
				public override MethodImplAttributes GetMethodImplementationFlags()
				{
					return MethodImplAttributes.IL | MethodImplAttributes.Runtime;
				}
			}
		}
	}
}
