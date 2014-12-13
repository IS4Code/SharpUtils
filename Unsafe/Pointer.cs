using System;
using IllidanS4.SharpUtils.Accessing;
using IllidanS4.SharpUtils.Metadata;

namespace IllidanS4.SharpUtils.Unsafe
{
	/// <summary>
	/// Generic equivalent of <see cref="System.IntPtr"/>.
	/// </summary>
	[Unsafe]
	public unsafe struct Pointer<T> : IPointer, IReadWriteAccessor<T> where T : struct
	{
		void* ptr;
		private static readonly Type ptrType = TypeOf<T>.TypeID;
		
		public Pointer(IntPtr pointer)
		{
			ptr = (void*)pointer;
		}
		
		[CLSCompliant(false)]
		public Pointer(void* pointer)
		{
			ptr = pointer;
		}
		
		public Pointer(ref T value) : this(__makeref(value))
		{
			
		}
		
		[CLSCompliant(false)]
		public Pointer(TypedReference reference)
		{
			ptr = *(void**)(&reference);
		}
		
		public IntPtr ToIntPtr()
		{
			return (IntPtr)ptr;
		}
		
		[CLSCompliant(false)]
		public void* ToPointer()
		{
			return (void*)ptr;
		}
		
		public bool IsNull
		{
			get{
				return ptr == default(void*);
			}
		}
		
		public T Value{
			get{
				TypedReference tr = default(TypedReference);
				var tptr = (void**)(&tr);
				tptr[0] = ptr;
				tptr[1] = (void*)ptrType.TypeHandle.Value;
				return __refvalue(tr, T);
			}
			set{
				TypedReference tr = default(TypedReference);
				var tptr = (void**)(&tr);
				tptr[0] = ptr;
				tptr[1] = (void*)ptrType.TypeHandle.Value;
				__refvalue(tr, T) = value;
			}
		}
	}
}