/* Date: 29.4.2015, Time: 15:17 */
using System;

namespace IllidanS4.SharpUtils.Proxies.Dynamic
{
	/// <summary>
	/// An object that can provide a <see cref="DynamicAdapter"/> allowing dynamic calls.
	/// </summary>
	public interface IDynamicAdapterProvider : IStaticAdapterProvider
	{
		new DynamicAdapter GetAdapter();
	}
}
