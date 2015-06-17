using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Accessing;
using IllidanS4.SharpUtils.Metadata;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils.Interop
{
	/// <summary>
	/// Generic equivalent of <see cref="System.IntPtr"/>.
	/// </summary>
	[Unsafe]
	public unsafe struct Pointer<T> : IPointer, IReadWriteAccessor<T>, IRefReference<T>, ITypedReference where T : struct
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
		
		[Obsolete("Converting managed pointer to unmanaged needs pinning, use UnsafeTools.GetPointer.", true)]
 		public Pointer(ref T value) : this(__makeref(value))
		{
			
		}
		
		[CLSCompliant(false)]
		[Obsolete("Converting managed pointer to unmanaged needs pinning, use UnsafeTools.GetPointer.", true)]
		public Pointer(TypedReference reference)
		{
			ptr = (void*)reference.ToPointer();
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
		
		public Type Type{
			get{
				return ptrType.MakePointerType();
			}
		}
		
		public bool IsNull{
			get{
				return ptr == null;
			}
		}
		
		public TRet GetReference<TRet>(Reference.OutFunc<T, TRet> func)
		{
			return GetReference(Reference.OutToRefFunc(func));
		}
		
		public TRet GetReference<TRet>(Reference.RefFunc<T, TRet> func)
		{
			return UnsafeTools.GetReference(this, func);
		}
		
		[CLSCompliant(false)]
		public TRet GetReference<TRet>(TypedReferenceTools.TypedRefFunc<TRet> func)
		{
			return GetReference<TRet>((ref T v)=>func(__makeref(v)));
		}
		
		public T Value{
			get{
				return GetReference((ref T r)=>r);
			}
			set{
				GetReference((ref T r)=>r = value);
			}
		}
		
		T IReadAccessor<T>.Item{
			get{
				return Value;
			}
		}
		
		T IWriteAccessor<T>.Item{
			set{
				Value = value;
			}
		}
		
		T IReadWriteAccessor<T>.Item{
			get{
				return Value;
			}
			set{
				Value = value;
			}
		}
		
		Type IStorageAccessor.Type{
			get{
				return ptrType;
			}
		}
		
		object IReadAccessor.Item{
			get{
				return Value;
			}
		}
		object IWriteAccessor.Item{
			set{
				Value = (T)value;
			}
		}
		object IReadWriteAccessor.Item{
			get{
				return Value;
			}
			set{
				Value = (T)value;
			}
		}
		
		object IStrongBox.Value{
			get{
				return Value;
			}
			set{
				Value = (T)value;
			}
		}
	}
}