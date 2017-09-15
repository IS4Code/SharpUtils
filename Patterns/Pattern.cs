using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace IllidanS4.SharpUtils.Patterns
{
	public static partial class Pattern
	{
		static readonly Type typeofIPattern = TypeOf<IPattern>.TypeID;
		static readonly Type typeofObject = TypeOf<object>.TypeID;
		static readonly Type typeofString = TypeOf<string>.TypeID;
		static readonly Type typeofType = TypeOf<Type>.TypeID;
		static readonly Type typeofStaticTypeAttribute = TypeOf<StaticTypeAttribute>.TypeID;
		static readonly Type typeofBindNameAttribute = TypeOf<BindNameAttribute>.TypeID;
		static readonly Type typeofStaticTypeArg = TypeOf<StaticTypeArg>.TypeID;
		
		static readonly MethodInfo m_Type_GetTypeFromHandle = typeofType.GetMethod("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static);
		static readonly MethodInfo m_IPattern_get_Class = typeofIPattern.GetMethod("get_Class");
		static readonly MethodInfo m_Object_ToString = typeofObject.GetMethod("ToString");
		
		public static object Create(Type staticType)
		{
			return Create(typeofIPattern, staticType);
		}
		
		public static Type CreateType(Type templateType, Type staticType)
		{
			if(templateType == null) templateType = typeofIPattern;
			
			if(!templateType.IsAbstract)throw new ArgumentException("Argument is not an interface or abstract class.", "templateType");
			if(templateType.IsGenericTypeDefinition)
			{
				Type[] genArgs = templateType.GetGenericArguments();
				for(int i = 0; i < genArgs.Length; i++)
				{
					Type t = genArgs[i];
					if(!t.IsDefined(typeofStaticTypeAttribute, false))
					{
						throw new ArgumentException("Template type cannot be generic type definition.", "templateType");
					}else{
						genArgs[i] = staticType;
					}
				}
				templateType = templateType.MakeGenericType(genArgs);
			}else if(templateType.IsGenericType)
			{
				var def = templateType.GetGenericTypeDefinition();
				Type[] genArgs = templateType.GetGenericArguments();
				for(int i = 0; i < genArgs.Length; i++)
				{
					if(genArgs[i] == typeofStaticTypeArg)
					{
						genArgs[i] = staticType;
					}
				}
				templateType = def.MakeGenericType(genArgs);
			}
			var tb = DynamicResources.DynamicModule.DefineType("DynamicTemplate", TypeAttributes.Public);
			if(templateType.IsInterface)
			{
				tb.SetParent(typeofObject);
				tb.AddInterfaceImplementation(templateType);
			}else{
				tb.SetParent(templateType);
			}
			tb.AddInterfaceImplementation(typeofIPattern);
			tb.AddInterfaceImplementation(typeofIDynamicMetaObjectProvider);
			
			{
				MethodBuilder m_ToString = tb.DefineMethod("ToString", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final, typeofString, Type.EmptyTypes);
				var il = m_ToString.GetILGenerator();
				il.Emit(OpCodes.Ldstr, templateType.ToString()+":"+staticType.ToString());
				il.Emit(OpCodes.Ret);
				tb.DefineMethodOverride(m_ToString, m_Object_ToString);
			}
			{
				PropertyBuilder p_Class = tb.DefineProperty("Class", PropertyAttributes.None, typeofType, Type.EmptyTypes);
				MethodBuilder m_get_Class = tb.DefineMethod("get_Class", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final, typeofType, Type.EmptyTypes);
				var il = m_get_Class.GetILGenerator();
				il.Emit(OpCodes.Ldtoken, staticType);
				il.Emit(OpCodes.Call, m_Type_GetTypeFromHandle);
				il.Emit(OpCodes.Ret);
				p_Class.SetGetMethod(m_get_Class);
				tb.DefineMethodOverride(m_get_Class, m_IPattern_get_Class);
			}
			{
				MethodBuilder m_GetMetaObject = tb.DefineMethod("GetMetaObject", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final, typeofDynamicMetaObject, m_IDynamicMetaObjectProvider_GetMetaObject_parameters);
				var il = m_GetMetaObject.GetILGenerator();
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Newobj, c_TemplateDynamicMetaObject);
				il.Emit(OpCodes.Ret);
				tb.DefineMethodOverride(m_GetMetaObject, m_IDynamicMetaObjectProvider_GetMetaObject);
			}
			
			if(templateType != typeofIPattern)
			{
				Dictionary<string,MethodBuilder> implemented = new Dictionary<string, MethodBuilder>();
				Dictionary<string,MethodInfo> notfound = new Dictionary<string, MethodInfo>();
				foreach(MethodInfo mi in templateType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
				{
					if(!mi.IsVirtual || mi.IsFinal || (mi.DeclaringType != templateType && !mi.IsAbstract))continue;
					string findname = mi.Name;
					if(mi.IsDefined(typeofBindNameAttribute))
					{
						BindNameAttribute attr = (BindNameAttribute)mi.GetCustomAttribute(typeofBindNameAttribute);
						findname = attr.Name;
					}
					Type[] paramtypes = GetParameterTypes(mi.GetParameters());
					MethodBuilder mb;
					if(findname == ".ctor")
					{
						if(!mi.ReturnType.IsAssignableFrom(staticType))throw new Exception("Return type for method "+mi.Name+" does not match.");
						ConstructorInfo fci = staticType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, paramtypes, null);
						if(fci == null)
						{
							if(mi.IsAbstract) notfound.Add(findname, mi);
							continue;
						}
						mb = tb.DefineMethod(mi.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final, mi.ReturnType, paramtypes);
						var il = mb.GetILGenerator();
						for(int i = 1; i <= paramtypes.Length; i++)
						{
							il.Emit(OpCodes.Ldarg, i);
						}
						il.Emit(OpCodes.Newobj, fci);
						if(staticType.IsValueType != mi.ReturnType.IsValueType)
						{
							il.Emit(OpCodes.Box, staticType);
						}
						il.Emit(OpCodes.Ret);
						
					}else{
						MethodInfo fmi = staticType.GetMethod(findname, BindingFlags.Public | BindingFlags.Static, null, paramtypes, null);
						if(fmi == null)
						{
							if(mi.IsAbstract) notfound.Add(findname, mi);
							continue;
						}
						//if(mi.ReturnType != fmi.ReturnType)throw new Exception("Return type for method "+mi.Name+" does not match.");
						if(!mi.ReturnType.IsAssignableFrom(fmi.ReturnType))throw new Exception("Return type for method "+mi.Name+" does not match.");
						mb = tb.DefineMethod(mi.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final, mi.ReturnType, paramtypes);
						var il = mb.GetILGenerator();
						for(int i = 1; i <= paramtypes.Length; i++)
						{
							il.Emit(OpCodes.Ldarg, i);
						}
						il.Emit(OpCodes.Call, fmi);
						if(fmi.ReturnType.IsValueType != mi.ReturnType.IsValueType)
						{
							il.Emit(OpCodes.Box, fmi.ReturnType);
						}
						il.Emit(OpCodes.Ret);
					}
					tb.DefineMethodOverride(mb, mi);
					implemented.Add(mb.Name, mb);
				}
				foreach(PropertyInfo pi in templateType.GetProperties())
				{
					string findname = pi.Name;
					Type[] indextypes = GetParameterTypes(pi.GetIndexParameters());
					PropertyBuilder pb = tb.DefineProperty(pi.Name, PropertyAttributes.None, pi.PropertyType, indextypes);
					MethodBuilder m_acc;
					
					if(pi.GetGetMethod(true) != null)
					{
						if(implemented.TryGetValue("get_"+findname, out m_acc))
						{
							pb.SetGetMethod(m_acc);
						}else if(indextypes.Length == 0)
						{
							FieldInfo fi = staticType.GetField(findname, BindingFlags.Public | BindingFlags.Static);
							if(fi != null && fi.FieldType == pi.PropertyType)
							{
								string mname = "get_"+pi.Name;
								MethodBuilder m_f_get = tb.DefineMethod(mname, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final, fi.FieldType, Type.EmptyTypes);
								var il = m_f_get.GetILGenerator();
								if(fi.IsLiteral)
								{
									EmitConstantValueOpCode(il, fi);
								}else{
									il.Emit(OpCodes.Ldsfld, fi);
								}
								il.Emit(OpCodes.Ret);
								MethodInfo m_imp;
								if(notfound.TryGetValue(mname, out m_imp))
								{
									tb.DefineMethodOverride(m_f_get, m_imp);
									notfound.Remove(mname);
								}
							}
						}
					}
					
					if(pi.GetSetMethod(true) != null)
					{
						if(implemented.TryGetValue("set_"+findname, out m_acc))
						{
							pb.SetSetMethod(m_acc);
						}else if(indextypes.Length == 0)
						{
							FieldInfo fi = staticType.GetField(findname, BindingFlags.Public | BindingFlags.Static);
							if(fi != null && fi.FieldType == pi.PropertyType && !fi.IsInitOnly)
							{
								string mname = "set_"+pi.Name;
								MethodBuilder m_f_set = tb.DefineMethod(mname, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final, null, new[]{fi.FieldType});
								var il = m_f_set.GetILGenerator();
								il.Emit(OpCodes.Ldarg_1);
								il.Emit(OpCodes.Stsfld, fi);
								il.Emit(OpCodes.Ret);
								MethodInfo m_imp;
								if(notfound.TryGetValue(mname, out m_imp))
								{
									tb.DefineMethodOverride(m_f_set, m_imp);
									notfound.Remove(mname);
								}
							}
						}
					}
				}
				foreach(MethodInfo mi in notfound.Values)
				{
					throw new Exception("Cannot find static method for "+mi.Name+".");
				}
			}
			
			Type createdType = tb.CreateType();
			return createdType;
		}
		
		public static Func<IPattern> CreateFactory(Type templateType, Type staticType)
		{
			Type createdType = CreateType(templateType, staticType);
			return ()=>((IPattern)Activator.CreateInstance(createdType));
		}
		
		public static IPattern Create(Type templateType, Type staticType)
		{
			return (IPattern)Activator.CreateInstance(CreateType(templateType, staticType));
		}
		
		public static Func<IPattern> CreateTemplateFactory<T>(Type staticType)
		{
			Type createdType = CreateType(TypeOf<T>.TypeID, staticType);
			return ()=>((IPattern)Activator.CreateInstance(createdType));
		}
		
		public static T Create<T>(Type templateType, Type staticType) where T : class
		{
			return Create(templateType, staticType) as T;
		}
		
		public static T Create<T>(Type staticType) where T : class
		{
			return Create(TypeOf<T>.TypeID, staticType) as T;
		}
		
		private static Type[] GetParameterTypes(ParameterInfo[] parameters)
		{
			int length = parameters.Length;
			Type[] types = new Type[length];
			for(int i = 0; i < length; i++)
			{
				types[i] = parameters[i].ParameterType;
			}
			return types;
		}
		
		private static void EmitConstantValueOpCode(ILGenerator il, FieldInfo fi)
		{
			object value = fi.GetRawConstantValue();
			TypeCode type = Type.GetTypeCode(fi.FieldType);
			switch(type)
			{
				case TypeCode.Boolean:
					il.Emit(((bool)value)?OpCodes.Ldc_I4_1:OpCodes.Ldc_I4_0);
					return;
				case TypeCode.Byte:
					il.Emit(OpCodes.Ldc_I4, (byte)value);
					return;
				case TypeCode.Double:
					il.Emit(OpCodes.Ldc_R8, (double)value);
					return;
				case TypeCode.Char:
					il.Emit(OpCodes.Ldc_I4, (char)value);
					return;
				case TypeCode.Int16:
					il.Emit(OpCodes.Ldc_I4, (short)value);
					return;
				case TypeCode.Int32:
					il.Emit(OpCodes.Ldc_I4, (int)value);
					return;
				case TypeCode.Int64:
					il.Emit(OpCodes.Ldc_I8, (long)value);
					return;
				case TypeCode.SByte:
					il.Emit(OpCodes.Ldc_I4, (sbyte)value);
					return;
				case TypeCode.Single:
					il.Emit(OpCodes.Ldc_R4, (float)value);
					return;
				case TypeCode.UInt16:
					il.Emit(OpCodes.Ldc_I4, (ushort)value);
					return;
				case TypeCode.UInt32:
					il.Emit(OpCodes.Ldc_I4, (uint)value);
					return;
				case TypeCode.UInt64:
					il.Emit(OpCodes.Ldc_I8, (ulong)value);
					return;
			}
		}
		
		static readonly Type typeofPParseable = typeof(PParseable<>);
		public static IParseable GePParseable(Type t)
		{
			return Create<IParseable>(typeofPParseable, t);
		}
		
		public static PParseable<T> GePParseable<T>()
		{
			return Create<PParseable<T>>(TypeOf<T>.TypeID);
		}
		
		static readonly Type typeofPRanged = typeof(PRanged<>);
		public static IRanged GePRanged(Type t)
		{
			return Create<IRanged>(typeofPRanged, t);
		}
		
		public static PRanged<T> GePRanged<T>()
		{
			return Create<PRanged<T>>(TypeOf<T>.TypeID);
		}
		
		static readonly Type typeofTStaticSingleTypeComparer = typeof(PStaticSingleTypeComparer<>);
		public static IComparer GetComparer(Type t)
		{
			return Create<IComparer>(typeofTStaticSingleTypeComparer, t);
		}
		
		public static PStaticSingleTypeComparer<T> GetComparer<T>()
		{
			return Create<PStaticSingleTypeComparer<T>>(TypeOf<T>.TypeID);
		}
		
		public static PStaticComparer<T1,T2> GetComparer<T1,T2>()
		{
			return Create<PStaticComparer<T1,T2>>(TypeOf<T1>.TypeID);
		}
		
		static readonly Type typeofTStaticSingleTypeEqualityComparer = typeof(PStaticSingleTypeComparer<>);
		public static IEqualityComparer GetEqualityComparer(Type t)
		{
			return Create<IEqualityComparer>(typeofTStaticSingleTypeEqualityComparer, t);
		}
		
		public static PStaticSingleTypeEqualityComparer<T> GetEqualityComparer<T>()
		{
			return Create<PStaticSingleTypeEqualityComparer<T>>(TypeOf<T>.TypeID);
		}
		
		public static PStaticEqualityComparer<T1,T2> GetEqualityComparer<T1,T2>()
		{
			return Create<PStaticEqualityComparer<T1,T2>>(TypeOf<T1>.TypeID);
		}
	}
}
