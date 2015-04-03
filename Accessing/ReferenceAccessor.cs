/* Date: ‎20.12.‎2012, Time: ‏‎17:00 */
using System;
using IllidanS4.SharpUtils.Interop;
using IllidanS4.SharpUtils.Metadata;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils.Accessing
{
	public class ReferenceAccessor<T> : OutputAccessor<T>, IReadAccessor<T>
	{
		public ReferenceAccessor(ref T value) : base(out value)
		{
			
		}
		
		[CLSCompliant(false)]
		public ReferenceAccessor(TypedReference tr) : base(tr)
		{
			
		}
		
		public ReferenceAccessor([Boxed(typeof(TypedReference))]ValueType tr) : base(tr)
		{
			
		}
		
		public ReferenceAccessor(object boxed) : base(boxed)
		{
			
		}
		
		public new T Item{
			set{
				base.Item = value;
			}
			get{
				return __refvalue((TypedReference)m_ref, T);
			}
		}
		
		object IReadAccessor.Item{
			get{
				return this.Item;
			}
		}
		
		/*public ReferenceAccessor(ref T value) : this(__makeref(value))
		{
			
		}
		
		[CLSCompliant(false)]
		public ReferenceAccessor(TypedReference value)
		{
			@ref = UnsafeTools.Box(value);
		}
		
		public ReferenceAccessor(ValueType value)
		{
			if(value == null)throw new ArgumentNullException("value");
			if(!(value is T))throw new ArgumentException("Argument must be of type "+TypeOf<T>.TypeID.ToString()+".", "value");
			
			@ref = TypedReferenceTools.MakeTypedReference(value);
		}
		
		public T Value{
			get{
				return __refvalue((TypedReference)@ref, T);
			}
			set{
				__refvalue((TypedReference)@ref, T) = value;
			}
		}*/
	}
}