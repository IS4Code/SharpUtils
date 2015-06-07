/* Date: ‎20.12.‎2012, Time: ‏‎17:00 */
using System;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Interop;
using IllidanS4.SharpUtils.Metadata;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils.Accessing
{
	public class OutputAccessor<T> : BasicWriteAccessor<T>, ITypedReference
	{
		[Boxed(typeof(TypedReference))]
		protected readonly ValueType m_ref;
		
		public OutputAccessor(out T value)
		{
			IllidanS4.SharpUtils.Reference.Use(out value);
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
		
		public unsafe OutputAccessor(object boxed) : this(TypedReferenceTools.MakeTypedReference(boxed))
		{
			if(boxed == null)throw new ArgumentNullException("value");
			if(!(boxed is T))throw new ArgumentException("Argument must be of type "+TypeOf<T>.TypeID.ToString()+".", "value");
		}
		
		public override T Item{
			set{
				__refvalue((TypedReference)m_ref, T) = value;
			}
		}
		
		[Boxed(typeof(TypedReference))]
		public ValueType Reference{
			get{
				return m_ref;
			}
		}
		
		[CLSCompliant(false)]
		public unsafe void GetReference([Out]TypedReference* tr)
		{
			*tr = (TypedReference)m_ref;
		}
		
		Type ITypedReference.Type{
			get{
				return __reftype((TypedReference)m_ref);
			}
		}
	}
}