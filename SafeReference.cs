/* Date: 13.6.2015, Time: 13:34 */
using System;
using System.Reflection;
using IllidanS4.SharpUtils.Accessing;
using IllidanS4.SharpUtils.Interop;
using IllidanS4.SharpUtils.Metadata;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils
{
	/// <summary>
	/// Contains a reference to a variable.
	/// </summary>
	public sealed class SafeReference : IDisposable, ITypedReference, IReadWriteAccessor, IEquatable<SafeReference>
	{
		private TypedRef? m_ref{get; set;}
		
		/// <summary>
		/// Checks whether this is 'out' (write-only) reference. If this has been written to with <see cref="Value"/> or <see cref="SetValue"/>, returns true.
		/// </summary>
		public bool IsOut{get; private set;}
		
		/// <summary>
		/// Checks whether this reference is valid and not disposed.
		/// </summary>
		public bool IsValid{
			get{
				return m_ref != null;
			}
		}
		
		private unsafe SafeReference(TypedReference tr, bool isOut)
		{
			m_ref = *(TypedRef*)(&tr);
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
		
		[CLSCompliant(false)]
		public static void Create(TypedReference tr, Action<SafeReference> act)
		{
			tr.Pin(
				tr2 => {using(var r = new SafeReference(tr2, false))act(r);}
			);
		}
		
		[CLSCompliant(false)]
		public static TRet Create<TRet>(TypedReference tr, Func<SafeReference, TRet> func)
		{
			return tr.Pin(
				tr2 => {using(var r = new SafeReference(tr2, false))return func(r);}
			);
		}
		
		public unsafe Type Type{
			get{
				var tr = m_ref.Value;
				return __reftype(*(TypedReference*)(&tr));
			}
		}
		
		public unsafe object Value{
			get{
				if(IsOut) throw new InvalidOperationException("This is a write-only reference.");
				var tr = m_ref.Value;
				return (*(TypedReference*)(&tr)).GetValue();
			}
			set{
				var tr = m_ref.Value;
				(*(TypedReference*)(&tr)).SetValue(value);
				IsOut = false;
			}
		}
		
		public unsafe void SetValue<T>(T value)
		{
			var tr = m_ref.Value;
			__refvalue(*(TypedReference*)(&tr), T) = value;
			IsOut = false;
		}
		
		public unsafe T GetValue<T>()
		{
			if(IsOut) throw new InvalidOperationException("This is a write-only reference.");
			var tr = m_ref.Value;
			return __refvalue(*(TypedReference*)(&tr), T);
		}
		
		[CLSCompliant(false)]
		public unsafe void GetReference(TypedReferenceTools.TypedRefAction act)
		{
			if(IsOut) throw new InvalidOperationException("This is a write-only reference.");
			var tr = m_ref.Value;
			act(*(TypedReference*)(&tr));
		}
		
		[CLSCompliant(false)]
		public unsafe TRet GetReference<TRet>(TypedReferenceTools.TypedRefFunc<TRet> func)
		{
			if(IsOut) throw new InvalidOperationException("This is a write-only reference.");
			var tr = m_ref.Value;
			return func(*(TypedReference*)(&tr));
		}
		
		TRet ITypedReference.GetReference<TRet>(Func<SafeReference,TRet> func)
		{
			return func(this);
		}
		
		object IReadWriteAccessor.Item{
			get{
				return Value;
			}
			set{
				Value = value;
			}
		}
		
		object IWriteAccessor.Item{
			set{
				Value = value;
			}
		}
		object IReadAccessor.Item{
			get{
				return Value;
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
		
		public bool IsNull{
			get{
				return m_ref.Value.Value == IntPtr.Zero;
			}
		}
		
		public bool IsEmpty{
			get{
				return m_ref.Value == default(TypedRef);
			}
		}
		
		public bool Equals(SafeReference other)
		{
			return m_ref == other.m_ref;
		}
		
		public override bool Equals(object obj)
		{
			SafeReference other = obj as SafeReference;
			if(other == null)
				return false;
			return Equals(other);
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (m_ref != null)
					hashCode += 1000000007 * m_ref.GetHashCode();
				hashCode += 1000000009 * IsOut.GetHashCode();
			}
			return hashCode;
		}
		
		
		public static void MakeReference(object target, FieldInfo[] fields, Action<SafeReference> act)
		{
			TypedReferenceTools.MakeTypedReference(target, fields, tr => Create(tr, act));
		}
		
		public static TRet MakeReference<TRet>(object target, FieldInfo[] fields, Func<SafeReference, TRet> func)
		{
			return TypedReferenceTools.MakeTypedReference(target, fields, tr => Create(tr, func));
		}
		
		public static void MakeReference(object target, Action<SafeReference> act, params FieldInfo[] fields)
		{
			MakeReference(target, fields, act);
		}
		
		public static TRet MakeReference<TRet>(object target, Func<SafeReference, TRet> func, params FieldInfo[] fields)
		{
			return MakeReference(target, fields, func);
		}
		
		public static void GetBoxedReference(ValueType target, Action<SafeReference> act)
		{
			MakeReference(target, act);
		}
		
		public static TRet GetBoxedReference<TRet>(ValueType target, Func<SafeReference,TRet> func)
		{
			return MakeReference(target, func);
		}
		
		internal void ChangeType(Type newType)
		{
			var tr = m_ref.Value;
			tr.Type = newType.TypeHandle.Value;
			m_ref = tr;
		}
		
		internal IntPtr GetAddress()
		{
			return m_ref.Value.Value;
		}
		
		internal void ChangeAddress(IntPtr addr)
		{
			var tr = m_ref.Value;
			tr.Value = addr;
			m_ref = tr;
		}
		
		public unsafe void SetValue(SafeReference value)
		{
			var tr1 = m_ref.Value;
			var tr2 = value.m_ref.Value;
			(*(TypedReference*)(&tr1)).SetValue(*(TypedReference*)(&tr2));
		}
	}
}
