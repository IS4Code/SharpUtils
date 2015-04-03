/* Date: 28.3.2015, Time: 16:24 */
using System;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Interop;
using IllidanS4.SharpUtils.Metadata;

namespace IllidanS4.SharpUtils.Accessing
{
	/// <summary>
	/// Represents an accessor to an array element.
	/// </summary>
	public class ArrayAccessor<T> : ListAccessor<T>, ITypedReference
	{
		public T[] Array{get; private set;}
		
		public ArrayAccessor(T[] array, int index) : base(array, index)
		{
			Array = array;
		}
		
		public new T Item{
			get{
				return Array[Index];
			}
			set{
				Array[Index] = value;
			}
		}
		
		[Boxed(typeof(TypedReference))]
		public ValueType Reference{
			get{
				return TypedReferenceTools.ArrayAddress(Array, Index);
			}
		}
		
		[CLSCompliant(false)]
		public unsafe void GetReference([Out]TypedReference* tr)
		{
			TypedReferenceTools.ArrayAddress(Array, tr, Index);
		}
		
		Type ITypedReference.Type{
			get{
				return typeof(T);
			}
		}
	}
}
