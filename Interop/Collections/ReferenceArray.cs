/* Date: 23.5.2015, Time: 15:41 */
using System;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Metadata;

namespace IllidanS4.SharpUtils.Interop.Collections
{
	public class ReferenceArray<T> : IIndexRefReferable<int, T>
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
		
		public TRet GetReference<TRet>(int index, Reference.RefFunc<T, TRet> func)
		{
			return func(ref Array[index]);
		}
		
		public TRet GetReference<TRet>(int index, Reference.OutFunc<T, TRet> func)
		{
			return func(out Array[index]);
		}
		
		[CLSCompliant(false)]
		public TRet GetReference<TRet>(object index, TypedReferenceTools.TypedRefFunc<TRet> func)
		{
			return func(__makeref(Array[(int)index]));
		}
	}
}
