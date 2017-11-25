/* Date: 25.11.2017, Time: 12:56 */
using System;

namespace IllidanS4.SharpUtils.Threads
{
	public abstract class FiberBase : IDisposable
	{
		public abstract FiberState State{get;}
		public abstract void Switch();
		public abstract void Dispose();
	}
}
