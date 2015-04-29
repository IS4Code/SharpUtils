/* Date: 14.2.2015, Time: 12:58 */
using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace IllidanS4.SharpUtils.Proxies
{
	public static class ProxyImplementationBinder
	{
		public static TBound GetProxy<TBound, TImplementation>(this IProxyReplacer<TBound, TImplementation> implementation) where TBound : MarshalByRefObject where TImplementation : class, IProxyReplacer<TBound, TImplementation>
		{
			return (TBound)new InterfaceProxy<TBound, TImplementation>(implementation).GetTransparentProxy();
		}
		
		public static TImplementation GetImplementation<TBound, TImplementation>(TBound proxy) where TBound : MarshalByRefObject where TImplementation : class, IProxyReplacer<TBound, TImplementation>
		{
			var rp = RemotingServices.GetRealProxy(proxy) as InterfaceProxy<TBound, TImplementation>;
			if(rp != null)
			{
				return (TImplementation)(object)rp.Implementation;
			}
			return null;
		}
		
		public static object GetImplementation(MarshalByRefObject proxy)
		{
			var rp = RemotingServices.GetRealProxy(proxy) as InterfaceProxyBase;
			if(rp != null)
			{
				return rp.Implementation;
			}
			return null;
		}
		
		private abstract class InterfaceProxyBase : RealProxy, IRemotingTypeInfo
		{
			public MarshalByRefObject Implementation{get; private set;}
			public Type ImplementationType{get; private set;}
			public Type BoundType{get; private set;}
			
			public InterfaceProxyBase(MarshalByRefObject impl, Type boundType, Type implType) : base(boundType)
			{
				Implementation = impl;
				ImplementationType = implType;
				BoundType = boundType;
				TypeName = boundType.FullName;
			}
			
			public string TypeName{
				get; set;
			}
			
			public bool CanCastTo(Type fromType, object o)
			{
				if(fromType.IsAssignableFrom(Implementation.GetType()))
				{
					return true;
				}
				return false;
			}
		}
		
		private class InterfaceProxy<TBound, TImplementation> : InterfaceProxyBase where TBound : MarshalByRefObject where TImplementation : class, IProxyReplacer<TBound, TImplementation>
		{
			private static readonly MethodInfo GetTypeMethod = typeof(object).GetMethod("GetType");
			
			public InterfaceProxy(IProxyReplacer<TBound, TImplementation> impl) : base((MarshalByRefObject)(object)impl, TypeOf<TBound>.TypeID, TypeOf<TImplementation>.TypeID)
			{
				
			}
			
			public override IMessage Invoke(IMessage msg)
			{
				IMethodCallMessage msgCall = msg as IMethodCallMessage;
				if(msgCall != null)
				{
					if(msgCall.MethodBase == GetTypeMethod)
					{
						return new ReturnMessage(BoundType, null, 0, msgCall.LogicalCallContext, msgCall);
					}
					return RemotingServices.ExecuteMessage(Implementation, msgCall);
				}
				return null;
			}
		}
	}
}
