/* Date: 4.12.2015, Time: 12:06 */
using System;
using System.Threading;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils.Threads
{
	public static class ThreadingTools
	{
		private delegate void ExchangeDelegate(TypedReference tr1, TypedReference tr2);
		private static readonly ExchangeDelegate _Exchange = Hacks.GetInvoker<ExchangeDelegate>(typeof(Interlocked), "_Exchange", false, 0);
		
		[CLSCompliant(false)]
		public static void Exchange(TypedReference location1, TypedReference location2)
		{
			if(__reftype(location1) != __reftype(location2)) throw new ArgumentException("Reference types do not match.");
			if(UnsafeTools.SizeOf(__reftype(location1)) > IntPtr.Size) throw new ArgumentException("Value is longer than native reference size.");
			_Exchange(location1, location2);
		}
		
		public static T Exchange<T>(ref T location1, T value)
		{
			if(UnsafeTools.SizeOf(TypeOf<T>.TypeID) > IntPtr.Size) throw new ArgumentException("Value is longer than native reference size.");
			_Exchange(__makeref(location1), __makeref(value));
			return value;
		}
		
		public static void Exchange<T>(ref T location1, ref T location2)
		{
			if(UnsafeTools.SizeOf(TypeOf<T>.TypeID) > IntPtr.Size) throw new ArgumentException("Value is longer than native reference size.");
			_Exchange(__makeref(location1), __makeref(location2));
		}
	}
}
