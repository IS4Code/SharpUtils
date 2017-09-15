/* Date: 26.11.2014, Time: 20:53 */
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection.Emit
{
	public static class TypeEmitGenerator
	{
		public static Type GenerateTypeFromDescriptor(ITypeEmitDescriptor desc)
		{
			string name = desc.Name;
			Type t = desc.GetType();
			TypeBuilder tb = DynamicResources.DynamicModule.DefineType(name, TypeAttributes.Public, t);
			foreach(var mi in t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
			{
				var args = mi.GetParameters();
				if(args.Length == 1 && args[0].ParameterType == typeof(ILGenerator))
				{
					var attr = mi.GetCustomAttribute<MethodTargetAttribute>();
					if(attr != null)
					{
						MethodBuilder mb = null;
						
						string targetname = attr.TargetName ?? mi.Name;
						foreach(var mi2 in t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
						{
							if(mi2.IsAbstract || mi2.IsVirtual && mi2.Name == targetname)
							{
								if(attr.ReturnType == null || mi2.ReturnType == attr.ReturnType)
								{
									var args2 = mi2.GetParameters().Select(p => p.ParameterType);
									if(attr.ParameterTypes == null || args2.SequenceEqual(attr.ParameterTypes))
									{
										mb = tb.DefineMethod(mi.Name, (mi2.Attributes &~ MethodAttributes.Abstract) | MethodAttributes.Virtual, mi2.ReturnType, args2.ToArray());
										tb.DefineMethodOverride(mb, mi2);
										break;
									}
								}
							}
						}
						
						if(mb == null)
						{
							throw new Exception("Cannot find a method that matches the signature of "+mi.Name+".");
						}
						
						var il = mb.GetILGenerator();
						if(mi.IsStatic)
						{
							mi.Invoke(null, new object[]{il});
						}else{
							mi.Invoke(desc, new object[]{il});
						}
					}
				}
			}
			return tb.CreateType();
		}
		
		public static T GenerateInstanceFromDescriptor<T>(T desc) where T : ITypeEmitDescriptor
		{
			Type t = GenerateTypeFromDescriptor(desc);
			return (T)Activator.CreateInstance(t);
		}
	}
}
