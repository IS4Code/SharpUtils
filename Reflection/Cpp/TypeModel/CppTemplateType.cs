/* Date: 11.12.2014, Time: 18:15 */
using System;
using System.Linq;
using System.Reflection;

namespace IllidanS4.SharpUtils.Reflection.Cpp
{
	public class CppTemplateType : CppType
	{
		private readonly CppEnvironment env;
		private readonly string ns;
		private readonly string name;
		
		public CppTemplateType(string name, CppEnvironment environment)
		{
			env = environment;
			string[] split = name.Split(new[]{"::"}, 0);
			ns = String.Join("::", split.Take(split.Length-1));
			this.name = split[split.Length-1];
		}
		
		public override string Namespace{
			get{
				return ns;
			}
		}
		
		public override Type ManagedType{
			get{
				return null;
			}
		}
		
		public override string FullName{
			get{
				if(String.IsNullOrEmpty(ns)) return name;
				else return ns+"::"+name;
			}
		}
		
		public override string Name{
			get{
				return name;
			}
		}
		
		public override string ToString()
		{
			return FullName+"<>";
		}
		
		public override CppEnvironment Environment{
			get{
				return env;
			}
		}
		
		public CppType GetTemplateInstance(params CppType[] args)
		{
			string name = FullName+"<"+String.Join(",", args.Select(a => a.ToString()))+">";
			name = name.Replace(">>", "> >");
			return env.GetType(name);
		}
		
		public override object[] GetTemplateArguments()
		{
			return new object[0];
		}
	}
}
