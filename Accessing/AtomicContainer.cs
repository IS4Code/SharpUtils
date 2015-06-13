/* Date: 28.12.2014, Time: 17:56 */
using System;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Interop;
using IllidanS4.SharpUtils.Metadata;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils.Accessing
{
	public class AtomicContainer<T> : BasicReadWriteAccessor<T>, IRefReference<T>, ITypedReference
	{
		public T Value;
		
		public AtomicContainer()
		{
			
		}
		
		public AtomicContainer(T value)
		{
			Value = value;
		}
		
		public override T Item{
			get{
				return Value;
			}
			set{
				Value = value;
			}
		}
		
		public TRet GetReference<TRet>(Reference.OutFunc<T, TRet> func)
		{
			return GetReference<TRet>(Reference.OutToRefFunc(func));
		}
		
		public TRet GetReference<TRet>(Reference.RefFunc<T, TRet> func)
		{
			return func(ref Value);
		}
		
		[CLSCompliant(false)]
		public TRet GetReference<TRet>(TypedReferenceTools.TypedRefFunc<TRet> func)
		{
			return func(__makeref(Value));
		}
	}
}
