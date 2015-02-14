/* Date: 14.2.2015, Time: 14:17 */
using System;

namespace IllidanS4.SharpUtils.Proxies
{
	public interface IProxyReplacer<TBound, TImplementation> where TBound : MarshalByRefObject where TImplementation : class, IProxyReplacer<TBound, TImplementation>
	{
		
	}
}
