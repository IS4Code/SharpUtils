/* Date: 14.2.2015, Time: 13:38 */
using System;

namespace IllidanS4.SharpUtils.Proxies
{
	public abstract class ProxyImplementationHelper<TBound, TImplementation> where TBound : MarshalByRefObject where TImplementation : class, IProxyReplacer<TBound, TImplementation>
	{
		protected TBound GetProxy(TImplementation impl)
		{
			return ProxyImplementationBinder.GetProxy<TBound, TImplementation>(impl);
		}
		
		
	}
}
