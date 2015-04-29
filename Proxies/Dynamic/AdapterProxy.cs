/* Date: 29.4.2015, Time: 15:02 */
using System;

namespace IllidanS4.SharpUtils.Proxies.Dynamic
{
	/// <summary>
	/// A basic interface for a proxy that is connected to an adapter.
	/// </summary>
	public interface IAdapterProxy
	{
		StaticAdapter Adapter{get;}
		Type ProxyType{get;}
		string ProxyTypeString{get;}
	}
}
