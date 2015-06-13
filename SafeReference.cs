/* Date: 13.6.2015, Time: 13:34 */
using System;
using IllidanS4.SharpUtils.Accessing;
using IllidanS4.SharpUtils.Interop;
using IllidanS4.SharpUtils.Metadata;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils
{
	public sealed class SafeReference : IDisposable, ITypedReference, IReadAccessor, IWriteAccessor
	{
		[Boxed(typeof(TypedReference))]
		private ValueType m_ref{get; set;}
		
		public bool IsOut{get; private set;}
		
		public bool IsValid{
			get{
				return m_ref != null;
			}
		}
		
		private SafeReference(TypedReference tr, bool isOut)
		{
			m_ref = UnsafeTools.Box(tr);
			IsOut = isOut;
		}
		
		public static void Create<T>(ref T obj, Action<SafeReference> act)
		{
			Reference.Pin(
				ref obj,
				(ref T o) => {using(var r = new SafeReference(__makeref(o), false))act(r);}
			);
		}
		
		public static void CreateOut<T>(out T obj, Action<SafeReference> act)
		{
			Reference.Pin(
				out obj,
				Reference.RefToOutAction(
					(ref T o) => {using(var r = new SafeReference(__makeref(o), true))act(r);}
				)
			);
		}
		
		public static TRet Create<T, TRet>(ref T obj, Func<SafeReference, TRet> func)
		{
			return Reference.Pin(
				ref obj,
				(ref T o) => {using(var r = new SafeReference(__makeref(o), true))return func(r);}
			);
		}
		
		public static TRet CreateOut<T, TRet>(out T obj, Func<SafeReference, TRet> func)
		{
			return Reference.Pin(
				out obj,
				Reference.RefToOutFunc(
					(ref T o) => {using(var r = new SafeReference(__makeref(o), true))return func(r);}
				)
			);
		}
		
		public Type Type{
			get{
				return __reftype((TypedReference)m_ref);
			}
		}
		
		public object Value{
			get{
				if(IsOut) throw new InvalidOperationException("This is a write-only reference.");
				return ((TypedReference)m_ref).GetValue();
			}
			set{
				((TypedReference)m_ref).SetValue(value);
			}
		}
		
		public void SetValue<T>(T value)
		{
			__refvalue((TypedReference)m_ref, T) = value;
		}
		
		public T GetValue<T>()
		{
			if(IsOut) throw new InvalidOperationException("This is a write-only reference.");
			return __refvalue((TypedReference)m_ref, T);
		}
		
		[CLSCompliant(false)]
		public TRet GetReference<TRet>(TypedReferenceTools.TypedRefFunc<TRet> func)
		{
			if(IsOut) throw new InvalidOperationException("This is a write-only reference.");
			return func((TypedReference)m_ref);
		}
		
		object IReadAccessor.Item{
			get{
				return Value;
			}
		}
		
		object IWriteAccessor.Item{
			set{
				Value = this;
			}
		}
		
		public void Dispose()
		{
			m_ref = null;
			GC.SuppressFinalize(this);
		}
		
		~SafeReference()
		{
			Dispose();
		}
	}
}
