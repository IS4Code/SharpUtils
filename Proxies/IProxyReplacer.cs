/* Date: 14.2.2015, Time: 14:17 */
using System;

namespace IllidanS4.SharpUtils.Proxies
{
	/// <summary>
	/// Used to mark an interface sufficient for providing an implementation for a proxiable class.
	/// </summary>
	public interface IProxyReplacer<TBound, TImplementation> where TBound : MarshalByRefObject where TImplementation : class, IProxyReplacer<TBound, TImplementation>
	{
		Type GetBoundType();
	}
}
