/* Date: 28.12.2014, Time: 17:56 */
using System;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Metadata;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils.Accessing
{
	public class AtomicContainer<T> : BasicReadWriteAccessor<T>, ITypedReference
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
		
		[Boxed(typeof(TypedReference))]
		public ValueType Reference{
			get{
				return UnsafeTools.Box(__makeref(Value));
			}
		}
		
		[CLSCompliant(false)]
		public unsafe void GetReference([Out]TypedReference* tr)
		{
			*tr = __makeref(Value);
		}
		
		Type ITypedReference.Type{
			get{
				return typeof(T);
			}
		}
	}
}
