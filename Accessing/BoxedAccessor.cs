/* Date: 19.6.2015, Time: 19:40 */
using System;
using IllidanS4.SharpUtils.Interop;

namespace IllidanS4.SharpUtils.Accessing
{
	public class BoxedAccessor<T> : BasicReadWriteAccessor<T>, IRefReference<T>, IBoxedAccessor where T : struct
	{
		public ValueType Instance{get; private set;}
		
		public BoxedAccessor(ValueType obj)
		{
			Instance = obj;
		}
		
		public BoxedAccessor() : this(default(T))
		{
			
		}
		
		public override T Item{
			get{
				return (T)Instance;
			}
			set{
				GetReference((ref T r)=>r=value);
			}
		}
		
		public TRet GetReference<TRet>(Reference.RefFunc<T, TRet> func)
		{
			return Reference.GetBoxedData(Instance, func);
		}
		
		public TRet GetReference<TRet>(Reference.OutFunc<T, TRet> func)
		{
			return Reference.GetBoxedData(Instance, Reference.OutToRefFunc(func));
		}
		
		[CLSCompliant(false)]
		public TRet GetReference<TRet>(TypedReferenceTools.TypedRefFunc<TRet> func)
		{
			return TypedReferenceTools.MakeTypedReference(Instance, func);
		}
		
		public TRet GetReference<TRet>(Func<SafeReference,TRet> func)
		{
			return SafeReference.GetBoxedReference(Instance, func);
		}
	}
	
	public interface IBoxedAccessor : IReadWriteAccessor, ITypedReference
	{
		ValueType Instance{get;}
	}
}
