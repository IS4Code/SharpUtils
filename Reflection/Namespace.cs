using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IllidanS4.SharpUtils.Reflection
{
	public sealed class Namespace : IEnumerable<Type>, IEnumerable<Namespace>
	{
		public string Name{get; private set;}
		
		public Namespace(string name)
		{
			Name = name;
		}
		
		public override string ToString()
		{
			return Name;
		}
		
		public Type[] GetTypes()
		{
			return this.ToArray<Type>();
		}
		
		public Namespace[] GetNamespaces()
		{
			return this.ToArray<Namespace>();
		}
		
		public IEnumerator<Type> GetEnumerator()
		{
			string namedot = Name+".";
			foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach(Type t in a.GetExportedTypes())
				{
					if(t.Namespace.StartsWith(namedot))
						yield return t;
				}
			}
		}
		
		IEnumerator<Namespace> IEnumerable<Namespace>.GetEnumerator()
		{
			string namedot = Name+".";
			List<string> ns = new List<string>();
			foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach(Type t in a.GetExportedTypes())
				{
					if(t.Namespace.StartsWith(namedot) && !ns.Contains(t.Namespace))
					{
						ns.Add(t.Namespace);
						yield return new Namespace(t.Namespace);;
					}
				}
			}
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
