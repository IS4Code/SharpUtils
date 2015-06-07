/* Date: 23.5.2015, Time: 15:41 */
using System;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Metadata;

namespace IllidanS4.SharpUtils.Interop.Collections
{
	public class ReferenceArray<T> : IIndexReferable<int, T>
	{
		public T[] Array{get; private set;}
		
		public ReferenceArray(T[] array)
		{
			Array = array;
		}
		
		public T this[int index]{
			get{
				return Array[index];
			}
			set{
				Array[index] = value;
			}
		}
		
		[CLSCompliant(false)]
		public unsafe void GetReference(int index, [Out]TypedReference* tref)
		{
			TypedReferenceTools.ArrayAddress(Array, tref, index);
		}
		
		[return: Boxed(typeof(TypedReference))]
		public ValueType GetReference(int index)
		{
			return TypedReferenceTools.ArrayAddress(Array, index);
		}
	}
}
