/* Date: 25.4.2015, Time: 9:22 */
using System;
using System.Runtime.Remoting;
using IllidanS4.SharpUtils.Interop;

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
		
		public static Delegate CreateDelegate(DelegateAdapter adapter)
		{
			var mdel = new MarshalArgsDelegate(adapter);
			return mdel.CreateInvoker();
		}
		
		private class MarshalArgsDelegate : VarArgsDelegate
		{
			public DelegateAdapter Delegate{get; private set;}
			
			public MarshalArgsDelegate(DelegateAdapter del)
			{
				Delegate = del;
			}
			
			public Delegate CreateInvoker()
			{
				return CreateInvoker(Delegate.ProxyType);
			}
			
			public override object Invoke(__arglist)
			{
				ArgIterator ai = new ArgIterator(__arglist);
				ObjectTypeHandle[] args = new ObjectTypeHandle[ai.GetRemainingCount()];
				while(ai.GetRemainingCount() > 0)
				{
					int idx = args.Length-ai.GetRemainingCount();
					object arg = TypedReference.ToObject(ai.GetNextArg());
					args[idx] = AdapterTools.Marshal(arg);
				}
				var res = Delegate.InvokeMember("Invoke", ref args, new bool[args.Length]);
				ai = new ArgIterator(__arglist);
				while(ai.GetRemainingCount() > 0)
				{
					int idx = args.Length-ai.GetRemainingCount();
					ObjectHandle arg = args[idx];
					var tr = ai.GetNextArg();
					if(__reftype(tr).IsByRef)
						tr.SetValue(arg.Unwrap());
				}
				if(res != null)
					return res.Unwrap();
				else return null;
			}
		}
	}
}
