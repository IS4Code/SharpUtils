/* Date: 29.4.2015, Time: 15:16 */
using System;

namespace IllidanS4.SharpUtils.Proxies.Dynamic
{
	/// <summary>
	/// An object that can provide a <see cref="StaticAdapter"/> to itself, allowing member invokes via reflection.
	/// </summary>
	public interface IStaticAdapterProvider
	{
		StaticAdapter GetAdapter();
	}
}
