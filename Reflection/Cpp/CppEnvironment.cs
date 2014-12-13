/* Date: 11.12.2014, Time: 21:02 */
using System;
using System.Reflection;

namespace IllidanS4.SharpUtils.Reflection.Cpp
{
	public class CppEnvironment
	{
		public Assembly Assembly{get; private set;}
		
		public CppEnvironment(Assembly assembly)
		{
			Assembly = assembly;
		}
		
		public CppType GetType(string name)
		{
			return null;
		}
	}
}
