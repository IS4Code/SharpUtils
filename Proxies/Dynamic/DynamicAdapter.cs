/* Date: 24.4.2015, Time: 15:31 */
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using Microsoft.CSharp.RuntimeBinder;

namespace IllidanS4.SharpUtils.Proxies.Dynamic
{
	/// <summary>
	/// An adapter that handles dynamic invokes through the DLR.
	/// </summary>
	public class DynamicAdapter : StaticAdapter
	{
		public DynamicAdapter(object obj) : base(obj)
		{
			
		}
		
		public DynamicAdapter(ObjectHandle objHandle) : base(objHandle)
		{
			
		}
		
		public ObjectHandle InvokeMember(BindingInfo binding, ref ObjectHandle[] args)
		{
			object[] oArgs = AdapterTools.Unmarshal(args);
			
			var binder = binding.CreateBinder();
			object[] argsObj = new object[args.Length+1];
			argsObj[0] = Value;
			oArgs.CopyTo(argsObj, 1);
			
			Expression dynExpr;
			object ret;
			const CSharpArgumentInfoFlags flagsRefOrOut = CSharpArgumentInfoFlags.IsRef | CSharpArgumentInfoFlags.IsOut;
			var argsConst = Expression.Constant(argsObj);
			var argsExp = argsObj.Select((o,i) => (binding.ArgumentFlags[i]&flagsRefOrOut)!=0?(Expression)Expression.ArrayAccess(argsConst, Expression.Constant(i)):Expression.Constant(o));
			try{
				if((binding.Flags & CSharpBinderFlags.ResultDiscarded) != 0)
				{
					dynExpr = Expression.Dynamic(binder, typeof(void), argsExp);
					Expression.Lambda<Action>(dynExpr).Compile().Invoke();
					ret = null;
				}else{
					dynExpr = Expression.Dynamic(binder, typeof(object), argsExp);
					ret = Expression.Lambda<Func<object>>(dynExpr).Compile().Invoke();
				}
			}catch(TargetInvocationException e)
			{
				throw e.InnerException;
			}
			for(int i = 0; i < binding.ArgumentFlags.Length; i++)
			{
				if((binding.ArgumentFlags[i] & flagsRefOrOut) == 0) argsObj[i] = null;
			}
			
			Array.Copy(argsObj, 1, oArgs, 0, oArgs.Length);
			args = AdapterTools.Marshal(oArgs);
			return AdapterTools.Marshal(ret);
		}
	}
}
