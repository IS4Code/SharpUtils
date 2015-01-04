/* Date: 11.12.2014, Time: 21:02 */
using System;
using System.Reflection;

namespace IllidanS4.SharpUtils.Reflection.Cpp
{
	public class CppEnvironment
	{
		public Module Module{get; private set;}
		
		public CppEnvironment(Module module)
		{
			Module = module;
		}
		
		public CppType GetType(string name)
		{
			return null;
		}
	}
}
