/* Date: ‎20.12.‎2012, Time: ‏‎17:00 */
using System;
using System.Runtime.CompilerServices;
using IllidanS4.SharpUtils.Interop;
using IllidanS4.SharpUtils.Metadata;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils.Accessing
{
	public class ReferenceAccessor<T> : OutputAccessor<T>, IReadWriteAccessor<T>, IRefReference<T>, ITypedReference
	{
		public ReferenceAccessor(SafeReference r) : base(r)
		{
			
		}
		
		public new T Item{
			set{
				base.Item = value;
			}
			get{
				return Ref.GetValue<T>();
			}
		}
		
		object IReadAccessor.Item{
			get{
				return Item;
			}
		}
		object IReadWriteAccessor.Item{
			get{
				return Item;
			}
			set{
				Item = (T)value;
			}
		}
		
		public static void Create(ref T value, Action<ReferenceAccessor<T>> act)
		{
			SafeReference.Create(
				ref value,
				r => act(new ReferenceAccessor<T>(r))
			);
		}
		
		public static TRet Create<TRet>(ref T value, Func<ReferenceAccessor<T>, TRet> func)
		{
			return SafeReference.Create(
				ref value,
				r => func(new ReferenceAccessor<T>(r))
			);
		}
		
		object IStrongBox.Value{
			get{
				return Item;
			}
			set{
				Item = (T)value;
			}
		}
		
		public TRet GetReference<TRet>(Reference.RefFunc<T, TRet> func)
		{
			return Ref.GetReference(tr => tr.AsRef(func));
		}
		
		[CLSCompliant(false)]
		public TRet GetReference<TRet>(TypedReferenceTools.TypedRefFunc<TRet> func)
		{
			return Ref.GetReference(func);
		}
	}
	
	public static class ReferenceAccessor
	{
		public static void Create<T>(ref T value, Action<ReferenceAccessor<T>> act)
		{
			ReferenceAccessor<T>.Create(ref value, act);
		}
		
		public static TRet Create<T, TRet>(ref T value, Func<ReferenceAccessor<T>, TRet> func)
		{
			return ReferenceAccessor<T>.Create(ref value, func);
		}
	}
}