/* Date: 24.4.2015, Time: 15:31 */
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using IllidanS4.SharpUtils.Reflection.Emit;

namespace IllidanS4.SharpUtils.Proxies.Dynamic
{
	/// <summary>
	/// An object adapter that allows member invokes via reflection.
	/// </summary>
	public class StaticAdapter : ObjectTypeHandle
	{
		protected object Value{get; private set;}
		
		public StaticAdapter(object obj) : base(obj)
		{
			Value = obj;
		}
		
		public StaticAdapter(ObjectHandle objHandle) : this(objHandle.Unwrap())
		{
			
		}
		
		public Type ProxyType{
			get{
				return Value.GetType();
			}
		}
		
		public string ProxyTypeString{
			get{
				return Value.GetType().AssemblyQualifiedName;
			}
		}
		
		public override string ToString()
		{
			return Value.ToString();
		}
				
		public ObjectTypeHandle InvokeMember(string member, ref ObjectTypeHandle[] args, bool[] isref)
		{
			Type[] argTypes = args.Select((a,i) => {
			                              	Type t = a==null?TypeOf<object>.TypeID:a.Type;
			                              	return isref[i]?t.MakeByRefType():t;
			                              }).ToArray();
			MethodInfo mi = ProxyType.GetMethod(member, argTypes);
			if(mi == null) throw new MissingMethodException();
			return InvokeMember(mi, ref args);
		}
		
		public ObjectTypeHandle InvokeMember(MethodBase method, ref ObjectTypeHandle[] args)
		{
			object[] oArgs = AdapterTools.Unmarshal(args);
			object ret;
			try{
				ret = method.Invoke(Value, oArgs);
			}catch(TargetInvocationException e)
			{
				throw e.InnerException;
			}
			var pars = method.GetParameters();
			for(int i = 0; i < pars.Length; i++)
			{
				if(!pars[i].ParameterType.IsByRef) oArgs[i] = null;
			}
			args = AdapterTools.Marshal(oArgs);
			return AdapterTools.Marshal(ret);
		}
		
		public static StaticProxy CreateProxy(StaticAdapter adapter)
		{
			return new StaticProxy(adapter);
		}
	}
}
