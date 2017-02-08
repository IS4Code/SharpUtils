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
	/// <remarks>
	/// An adapter encapsulates an object instance and provides methods to invoke members on the instance.
	/// </remarks>
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
		
		/// <summary>
		/// Gets the type of the stored instance.
		/// </summary>
		public Type ProxyType{
			get{
				return Value.GetType();
			}
		}
		
		/// <summary>
		/// Gets the assembly qualified name of the type of the stored instance.
		/// </summary>
		public string ProxyTypeString{
			get{
				return Value.GetType().AssemblyQualifiedName;
			}
		}
		
		/// <summary>
		/// Returns the string representation of the stored value.
		/// </summary>
		/// <returns>The string representation of the stored value.</returns>
		public override string ToString()
		{
			return Value.ToString();
		}
		
		/// <summary>
		/// Invokes a method on the stored instance. An array of <see cref="ObjectTypeHandle"/> must be used to pass the arguments.
		/// </summary>
		/// <param name="member">The method's name.</param>
		/// <param name="args">The arguments to invoke the method with.</param>
		/// <param name="isref">An array of the same length as <paramref name="args"/> specifying whether a parameter is passed by reference.</param>
		/// <returns>The return value of the method invocation.</returns>
		public ObjectTypeHandle InvokeMember(string member, ref ObjectTypeHandle[] args, bool[] isref)
		{
			Type[] argTypes = args.Select(
				(a,i) => {
					Type t = a==null?TypeOf<object>.TypeID:a.Type;
			        return isref[i]?t.MakeByRefType():t;
				}
			).ToArray();
			MethodInfo mi = ProxyType.GetMethod(member, argTypes);
			if(mi == null) throw new MissingMethodException();
			return InvokeMember(mi, ref args);
		}
		
		/// <summary>
		/// Invokes a method on the stored instance. An array of <see cref="ObjectTypeHandle"/> must be used to pass the arguments.
		/// </summary>
		/// <param name="method">The method reference to invoke on the instance.</param>
		/// <param name="args">The arguments to invoke the method with.</param>
		/// <returns>The return value of the method invocation.</returns>
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
		
		/// <summary>
		/// Assigns a newly created proxy to a passed adapter, proxying all calls on it.
		/// </summary>
		/// <param name="adapter">The adapter to assign.</param>
		/// <returns>The new static proxy transmitting all calls on it to the adapter.</returns>
		public static StaticProxy CreateProxy(StaticAdapter adapter)
		{
			return new StaticProxy(adapter);
		}
	}
}
