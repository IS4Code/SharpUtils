/* Date: 14.2.2015, Time: 13:38 */
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Proxies;

namespace IllidanS4.SharpUtils.Proxies
{
	/// <summary>
	/// The base class for custom proxy implementations.
	/// </summary>
	public abstract class ProxyImplementation<TBound, TImplementation> : MarshalByRefObject where TBound : MarshalByRefObject where TImplementation : class, IProxyReplacer<TBound, TImplementation>
	{
		/// <summary>
		/// A shortcut to <see cref="ProxyImplementationBinder.GetProxy"/>.
		/// </summary>
		/// <param name="impl">The custom implementation of the class.</param>
		/// <returns>The proxy for the class.</returns>
		protected TBound GetProxy(TImplementation impl)
		{
			return ProxyImplementationBinder.GetProxy<TBound, TImplementation>(impl);
		}
		
		/// <summary>
		/// Creates a new proxy object from the current implementation instance. A shortcut to <see cref="ProxyImplementationBinder.GetProxy"/> on the current instance.
		/// </summary>
		/// <returns>The new proxy.</returns>
		public TBound GetProxy()
		{
			return ProxyImplementationBinder.GetProxy<TBound, TImplementation>((TImplementation)(object)this);
		}
	}
}
