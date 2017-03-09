/* Date: 14.2.2015, Time: 12:58 */
using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace IllidanS4.SharpUtils.Proxies
{
	/// <summary>
	/// Obtains proxies to <see cref="System.MarshalByRefObject"/> classes with custom implementations.
	/// </summary>
	public static class ProxyImplementationBinder
	{
		/// <summary>
		/// Creates a <see cref="System.MarshalByRefObject"/> class proxy from its custom implementation.
		/// </summary>
		/// <param name="implementation">The custom implementation of the class.</param>
		/// <returns>The proxy for the class.</returns>
		public static TBound GetProxy<TBound, TImplementation>(this IProxyReplacer<TBound, TImplementation> implementation) where TBound : MarshalByRefObject where TImplementation : class, IProxyReplacer<TBound, TImplementation>
		{
			return (TBound)new InterfaceProxy<TBound, TImplementation>(implementation).GetTransparentProxy();
		}
		
		/// <summary>
		/// Obtains the custom implementation from a <see cref="System.MarshalByRefObject"/> class proxy.
		/// </summary>
		/// <param name="proxy">The proxy for the class.</param>
		/// <returns>The implmentation of the proxy.</returns>
		public static TImplementation GetImplementation<TBound, TImplementation>(TBound proxy) where TBound : MarshalByRefObject where TImplementation : class, IProxyReplacer<TBound, TImplementation>
		{
			var rp = RemotingServices.GetRealProxy(proxy) as InterfaceProxy<TBound, TImplementation>;
			if(rp != null)
			{
				return (TImplementation)(object)rp.Implementation;
			}
			return null;
		}
		
		/// <summary>
		/// Obtains the custom implementation from a <see cref="System.MarshalByRefObject"/> class proxy.
		/// </summary>
		/// <param name="proxy">The proxy for the class.</param>
		/// <returns>The implmentation of the proxy.</returns>
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
			
			public virtual bool CanCastTo(Type fromType, object o)
			{
				return fromType.IsAssignableFrom(Implementation.GetType());
			}
		}
		
		private class InterfaceProxy<TBound, TImplementation> : InterfaceProxyBase where TBound : MarshalByRefObject where TImplementation : class, IProxyReplacer<TBound, TImplementation>
		{
			private static readonly MethodInfo GetTypeMethod = typeof(object).GetMethod("GetType");
			
			public InterfaceProxy(IProxyReplacer<TBound, TImplementation> impl) : base((MarshalByRefObject)(object)impl, TypeOf<TBound>.TypeID, TypeOf<TImplementation>.TypeID)
			{
				
			}
			
			private Type GetBoundType()
			{
				return ((TImplementation)(object)Implementation).GetBoundType();
			}
			
			public override IMessage Invoke(IMessage msg)
			{
				IMethodCallMessage msgCall = msg as IMethodCallMessage;
				if(msgCall != null)
				{
					if(msgCall.MethodBase == GetTypeMethod)
					{
						Type bound = GetBoundType();
						if(!BoundType.IsAssignableFrom(bound))
						{
							throw new InvalidOperationException("Bound type must inherit TBound.");
						}
						return new ReturnMessage(bound, null, 0, msgCall.LogicalCallContext, msgCall);
					}
					
					var method = ImplementationType.GetMethod(msgCall.MethodName, msgCall.MethodSignature as Type[]);
					if(method == null) method = msgCall.MethodBase as MethodInfo;
					
					var args = msgCall.Args;
					object ret = method.Invoke(Implementation, args);
					return new ReturnMessage(ret, args, args.Length, msgCall.LogicalCallContext, msgCall);
				}
				return null;
			}
			
			public override bool CanCastTo(Type fromType, object o)
			{
				return base.CanCastTo(fromType, o) || (typeof(TBound).IsAssignableFrom(fromType) && fromType.IsAssignableFrom(GetBoundType()));
			}
		}
	}
}
