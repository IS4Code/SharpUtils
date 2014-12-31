/* Date: ‎20.12.‎2012, Time: ‏‎17:00 */
using System;
using IllidanS4.SharpUtils.Interop;
using IllidanS4.SharpUtils.Metadata;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils.Accessing
{
	public class OutputAccessor<T> : IWriteAccessor<T>
	{
		[Boxed(typeof(TypedReference))]
		protected readonly ValueType m_ref;
		
		public OutputAccessor(out T value)
		{
			Extensions.Use(out value);
			m_ref = UnsafeTools.Box(__makeref(value));
		}
		
		[CLSCompliant(false)]
		public OutputAccessor(TypedReference tr) : this(UnsafeTools.Box(tr))
		{
			
		}
		
		public OutputAccessor([Boxed(typeof(TypedReference))]ValueType tr)
		{
			m_ref = tr;
		}
		
		public unsafe OutputAccessor(object value) : this(TypedReferenceTools.MakeTypedReference(value))
		{
			if(value == null)throw new ArgumentNullException("value");
			if(!(value is T))throw new ArgumentException("Argument must be of type "+TypeOf<T>.TypeID.ToString()+".", "value");
		}
		
		public T Value{
			set{
				__refvalue((TypedReference)m_ref, T) = value;
			}
		}
	}
}