/* Date: ‎20.12.‎2012, Time: ‏‎17:00 */
using System;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils.Accessing
{
	public class ReferenceAccessor<T> : IReadWriteAccessor<T>
	{
		object @ref;
		
		public ReferenceAccessor(ref T value) : this(__makeref(value))
		{
			
		}
		
		[CLSCompliant(false)]
		public ReferenceAccessor(TypedReference value)
		{
			@ref = UnsafeTools.Box(value);
		}
		
		public unsafe ReferenceAccessor(object value)
		{
			if(value == null)throw new ArgumentNullException("value");
			if(!(value is ValueType))throw new ArgumentException("Value can be object only if its type is a value type.", "value");
			if(!(value is T))throw new ArgumentException("Argument must be of type "+TypeOf<T>.TypeID.ToString()+".", "value");
			
			TypedReference tr = __makeref(value);
			((IntPtr*)(&tr))[1] = value.GetType().TypeHandle.Value;
			((IntPtr*)(&tr))[0] = (**(IntPtr**)(&tr))+IntPtr.Size;
			
			@ref = UnsafeTools.Box(tr);
		}
		
		public T Value{
			get{
				return __refvalue((TypedReference)@ref, T);
			}
			set{
				__refvalue((TypedReference)@ref, T) = value;
			}
		}
	}
}