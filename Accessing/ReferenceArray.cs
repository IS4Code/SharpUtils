/* Date: 23.5.2015, Time: 15:41 */
using System;

namespace IllidanS4.SharpUtils.Accessing
{
	public class ReferenceArray<T> : IIndexRefReference<int, T>
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
		
		public TRet GetReference<TRet>(int index, Func<SafeReference, TRet> func)
		{
			return SafeReference.Create(ref Array[index], func);
		}
		
		public TRet GetReference<TRet>(object index, Func<SafeReference, TRet> func)
		{
			return GetReference<TRet>((object)index, func);
		}
	}
}
