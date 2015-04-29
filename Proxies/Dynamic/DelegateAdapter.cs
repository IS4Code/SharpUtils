/* Date: 25.4.2015, Time: 9:22 */
using System;

namespace IllidanS4.SharpUtils.Proxies.Dynamic
{
	/// <summary>
	/// Adapter used to marshal delegates.
	/// </summary>
	public class DelegateAdapter : StaticAdapter
	{
		public DelegateAdapter(Delegate del) : base(del)
		{
			
		}
	}
}
