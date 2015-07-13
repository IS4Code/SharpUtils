/* Date: 28.3.2015, Time: 16:24 */
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Interop;
using IllidanS4.SharpUtils.Metadata;

namespace IllidanS4.SharpUtils.Accessing
{
	/// <summary>
	/// Represents an accessor to an array element.
	/// </summary>
	public class ArrayAccessor<T> : ListAccessor<T>, IRefReference<T>, ITypedReference, IArrayAccessor
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
		
		public TRet GetReference<TRet>(Reference.OutFunc<T, TRet> func)
		{
			return GetReference<TRet>(Reference.OutToRefFunc(func));
		}
		
		public TRet GetReference<TRet>(Reference.RefFunc<T, TRet> func)
		{
			return func(ref Array[Index]);
		}
		
		[CLSCompliant(false)]
		public TRet GetReference<TRet>(TypedReferenceTools.TypedRefFunc<TRet> func)
		{
			return func(__makeref(Array[Index]));
		}
		
		public TRet GetReference<TRet>(Func<SafeReference,TRet> func)
		{
			return SafeReference.Create(__makeref(Array[Index]), func);
		}
		
		Array IArrayAccessor.Array{
			get{
				return Array;
			}
		}
		
		object IStrongBox.Value{
			get{
				return Item;
			}
			set{
				Item = (T)value;
			}
		}
	}
	
	public interface IArrayAccessor : ICollectionAccessor
	{
		Array Array{get;}
	}
	
	public static class ArrayAccessor
	{
		public static ArrayAccessor<T> Create<T>(T[] arr, int index)
		{
			return new ArrayAccessor<T>(arr, index);
		}
	}
}
