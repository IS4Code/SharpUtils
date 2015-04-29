/* Date: 14.2.2015, Time: 13:38 */
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Proxies;

namespace IllidanS4.SharpUtils.Proxies
{
	public abstract class ProxyImplementation<TBound, TImplementation> : MarshalByRefObject where TBound : MarshalByRefObject where TImplementation : class, IProxyReplacer<TBound, TImplementation>
	{
		protected TBound GetProxy(TImplementation impl)
		{
			return ProxyImplementationBinder.GetProxy<TBound, TImplementation>(impl);
		}
		
		public TBound GetProxy()
		{
			return ProxyImplementationBinder.GetProxy<TBound, TImplementation>((TImplementation)(object)this);
		}
	}
}
