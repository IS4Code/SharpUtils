/* Date: ‎20.12.‎2012, Time: ‏‎17:00 */
using System;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Interop;
using IllidanS4.SharpUtils.Metadata;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils.Accessing
{
	public class OutputAccessor<T> : BasicWriteAccessor<T>, IOutReference<T>
	{
		public SafeReference Ref{get; private set;}
		
		public OutputAccessor(SafeReference r)
		{
			Ref = r;
		}
		
		public override T Item{
			set{
				Ref.SetValue<T>(value);
			}
		}
		
		public TRet GetReference<TRet>(Reference.OutFunc<T, TRet> func)
		{
			return Ref.GetReference(tr => tr.AsRef(Reference.OutToRefFunc(func)));
		}
		
		public TRet GetReference<TRet>(Func<SafeReference, TRet> func)
		{
			return func(Ref);
		}
		
		public static void Create(out T value, Action<OutputAccessor<T>> act)
		{
			SafeReference.CreateOut(
				out value,
				r => act(new OutputAccessor<T>(r))
			);
		}
		
		public static TRet Create<TRet>(out T value, Func<OutputAccessor<T>, TRet> func)
		{
			return SafeReference.CreateOut(
				out value,
				r => func(new OutputAccessor<T>(r))
			);
		}
	}
	
	public static class OutputAccessor
	{
		public static void Create<T>(out T value, Action<OutputAccessor<T>> act)
		{
			OutputAccessor<T>.Create(out value, act);
		}
		
		public static TRet Create<T, TRet>(out T value, Func<OutputAccessor<T>, TRet> func)
		{
			return OutputAccessor<T>.Create(out value, func);
		}
	}
}