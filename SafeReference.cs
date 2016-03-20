/* Date: 13.6.2015, Time: 13:34 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Accessing;
using IllidanS4.SharpUtils.Interop;
using IllidanS4.SharpUtils.Metadata;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils
{
	/// <summary>
	/// Contains a disposable reference to a variable.
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
		
		/// <summary>
		/// Creates a new <see cref="SafeReference"/> from a "ref" reference.
		/// </summary>
		/// <param name="obj">The reference to a variable.</param>
		/// <param name="act">The action to receive the <see cref="SafeReference"/>.</param>
		public static void Create<T>(ref T obj, Action<SafeReference> act)
		{
			Reference.Pin(
				ref obj,
				(ref T o) => {using(var r = new SafeReference(__makeref(o), false))act(r);}
			);
		}
		
		/// <summary>
		/// Creates a new <see cref="SafeReference"/> from an "out" reference.
		/// </summary>
		/// <param name="obj">The reference to a variable.</param>
		/// <param name="act">The action to receive the <see cref="SafeReference"/>.</param>
		public static void CreateOut<T>(out T obj, Action<SafeReference> act)
		{
			Reference.Pin(
				out obj,
				Reference.RefToOutAction(
					(ref T o) => {using(var r = new SafeReference(__makeref(o), true))act(r);}
				)
			);
		}
		
		/// <summary>
		/// Creates a new <see cref="SafeReference"/> from a "ref" reference.
		/// </summary>
		/// <param name="obj">The reference to a variable.</param>
		/// <param name="func">The function to receive the <see cref="SafeReference"/>.</param>
		/// <returns>The value returned by <paramref name="func"/></returns>
		public static TRet Create<T, TRet>(ref T obj, Func<SafeReference, TRet> func)
		{
			return Reference.Pin(
				ref obj,
				(ref T o) => {using(var r = new SafeReference(__makeref(o), true))return func(r);}
			);
		}
		
		/// <summary>
		/// Creates a new <see cref="SafeReference"/> from an "out" reference.
		/// </summary>
		/// <param name="obj">The reference to a variable.</param>
		/// <param name="func">The function to receive the <see cref="SafeReference"/>.</param>
		/// <returns>The value returned by <paramref name="func"/></returns>
		public static TRet CreateOut<T, TRet>(out T obj, Func<SafeReference, TRet> func)
		{
			return Reference.Pin(
				out obj,
				Reference.RefToOutFunc(
					(ref T o) => {using(var r = new SafeReference(__makeref(o), true))return func(r);}
				)
			);
		}
		
		/// <summary>
		/// Creates a new <see cref="SafeReference"/> from a <see cref="TypedReference"/>.
		/// </summary>
		/// <param name="tr">The typed reference.</param>
		/// <param name="act">The action to receive the <see cref="SafeReference"/>.</param>
		[CLSCompliant(false)]
		public static void Create(TypedReference tr, Action<SafeReference> act)
		{
			tr.Pin(
				tr2 => {using(var r = new SafeReference(tr2, false))act(r);}
			);
		}
		
		/// <summary>
		/// Creates a new <see cref="SafeReference"/> from a <see cref="TypedReference"/>.
		/// </summary>
		/// <param name="tr">The typed reference.</param>
		/// <param name="func">The function to receive the <see cref="SafeReference"/>.</param>
		/// <returns>The value returned by <paramref name="func"/></returns>
		[CLSCompliant(false)]
		public static TRet Create<TRet>(TypedReference tr, Func<SafeReference, TRet> func)
		{
			return tr.Pin(
				tr2 => {using(var r = new SafeReference(tr2, false))return func(r);}
			);
		}
		
		/// <summary>
		/// Obtains an array of <see cref="SafeReference"/> from a variable number of typed references.
		/// </summary>
		/// <param name="act">The action to receive the array of <see cref="SafeReference"/>.</param>
		/// <param name="__arglist">A variable number of typed references.</param>
		[CLSCompliant(false)]
		public static void Create(Action<SafeReference[]> act, __arglist)
		{
			Create(act, new ArgIterator(__arglist));
		}
		
		/// <summary>
		/// Obtains an array of <see cref="SafeReference"/> from a variable number of typed references.
		/// </summary>
		/// <param name="act">The action to receive the array of <see cref="SafeReference"/>.</param>
		/// <param name="refs">A variable number of typed references.</param>
		public static void Create(Action<SafeReference[]> act, ArgIterator refs)
		{
			Create<Unit>(r=>{act(r);return 0;}, refs);
		}
		
		/// <summary>
		/// Obtains an array of <see cref="SafeReference"/> from a variable number of typed references.
		/// </summary>
		/// <param name="func">The function to receive the array of <see cref="SafeReference"/>.</param>
		/// <param name="refs">A variable number of typed references.</param>
		/// <returns>The value returned by <paramref name="func"/></returns>
		public static TRet Create<TRet>(Func<SafeReference[], TRet> func, ArgIterator refs)
		{
			var arr = new SafeReference[refs.GetRemainingCount()];
			if(arr.Length > 0)
			{
				return PinHelper<TRet>.PinRecursive(refs.GetNextArg(), refs, arr, func);
			}else{
				return func(arr);
			}
		}
		
		/// <summary>
		/// The type of the <see cref="SafeReference"/>.
		/// </summary>
		public unsafe Type Type{
			get{
				var tr = m_ref.Value;
				return __reftype(*(TypedReference*)(&tr));
			}
		}
		
		/// <summary>
		/// The value of the <see cref="SafeReference"/>.
		/// </summary>
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
		
		/// <summary>
		/// Sets the value of the <see cref="SafeReference"/>.
		/// </summary>
		public unsafe void SetValue<T>(T value)
		{
			var tr = m_ref.Value;
			__refvalue(*(TypedReference*)(&tr), T) = value;
			IsOut = false;
		}
		
		/// <summary>
		/// Returns the value of the <see cref="SafeReference"/>.
		/// </summary>
		/// <returns>The value of the <see cref="SafeReference"/>.</returns>
		public unsafe T GetValue<T>()
		{
			if(IsOut) throw new InvalidOperationException("This is a write-only reference.");
			var tr = m_ref.Value;
			return __refvalue(*(TypedReference*)(&tr), T);
		}
		
		/// <summary>
		/// Obtains a <see cref="TypedReference"/> from this object.
		/// </summary>
		/// <param name="act">The action to receive the typed reference.</param>
		[CLSCompliant(false)]
		public unsafe void GetReference(TypedReferenceTools.TypedRefAction act)
		{
			if(IsOut) throw new InvalidOperationException("This is a write-only reference.");
			var tr = m_ref.Value;
			act(*(TypedReference*)(&tr));
		}
		
		/// <summary>
		/// Obtains a <see cref="TypedReference"/> from this object.
		/// </summary>
		/// <param name="func">The function to receive the typed reference.</param>
		/// <returns>The value returned by <paramref name="func"/></returns>
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
		
		/// <summary>
		/// Disposes the reference and clears all pointers stored in it.
		/// </summary>
		public void Dispose()
		{
			m_ref = null;
			GC.SuppressFinalize(this);
		}
		
		~SafeReference()
		{
			Dispose();
		}
		
		/// <summary>
		/// True if this is a null reference.
		/// </summary>
		public bool IsNull{
			get{
				return m_ref.Value.Value == IntPtr.Zero;
			}
		}
		
		/// <summary>
		/// True if this is an empty reference (null and no type).
		/// </summary>
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
		
		private static class PinHelper<TRet>
		{
			public delegate TRet Del(TypedReference first, ArgIterator refs, SafeReference[] sr, Func<SafeReference[],TRet> func);
			
			public static readonly Del PinRecursive = MakePinRecursive();
			
			static Del MakePinRecursive()
			{
				var tai = typeof(ArgIterator);
				var tsr = typeof(SafeReference);
				var tvr = typeof(Func<SafeReference[],TRet>);
				var trt = typeof(TRet);
				var dyn = new DynamicMethod("PinRecursive", trt, new[]{typeof(TypedReference), tai, typeof(SafeReference[]), tvr}, typeof(PinHelper<TRet>));
				
				var ctor = tsr.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Single();
				
				var il = dyn.GetILGenerator();
				
				var next = il.DefineLabel();
				var end = il.DefineLabel();
				
				il.DeclareLocal(typeof(void).MakeByRefType(), true); //void&
				il.DeclareLocal(tsr); //SafeReference
				il.DeclareLocal(trt); //TRet
				
				il.Emit(OpCodes.Ldarga_S, 0); //loc_0 = ref *(void**)(&arg_0);
				il.Emit(OpCodes.Ldind_I);
				il.Emit(OpCodes.Stloc_0);
				
				il.Emit(OpCodes.Ldarg_0); //loc_1 = new SafeReference(arg_0, false);
				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(OpCodes.Newobj, ctor);
				il.Emit(OpCodes.Stloc_1);
				
				il.BeginExceptionBlock(); //try{
				
				il.Emit(OpCodes.Ldarg_2); //arg_2[arg_2.Length-arg_1.GetRemainingCount()-1] = loc_1;
				il.Emit(OpCodes.Ldarg_2);
				il.Emit(OpCodes.Ldlen);
				il.Emit(OpCodes.Ldarga_S, 1);
				il.Emit(OpCodes.Call, tai.GetMethod("GetRemainingCount"));
				il.Emit(OpCodes.Sub);
				il.Emit(OpCodes.Ldc_I4_1);
				il.Emit(OpCodes.Sub);
				il.Emit(OpCodes.Ldloc_1);
				il.Emit(OpCodes.Stelem_Ref);
				
				il.Emit(OpCodes.Ldarga_S, 1); //if(arg_1.GetRemainincCount() > 0) goto next;
				il.Emit(OpCodes.Call, tai.GetMethod("GetRemainingCount"));
				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(OpCodes.Bgt_S, next);
				
				il.Emit(OpCodes.Ldarg_3); //loc_2 = arg_3(arg_2);
				il.Emit(OpCodes.Ldarg_2);
				il.Emit(OpCodes.Callvirt, tvr.GetMethod("Invoke"));
				il.Emit(OpCodes.Stloc_2);
				
				il.Emit(OpCodes.Br_S, end); //goto end;
				
				il.MarkLabel(next); //next:
				
				il.Emit(OpCodes.Ldarga_S, 1); //loc_2 = PinRecursive(arg_1.GetNextArg(), arg_1, arg_2, arg_3);
				il.Emit(OpCodes.Call, tai.GetMethod("GetNextArg", Type.EmptyTypes));
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldarg_2);
				il.Emit(OpCodes.Ldarg_3);
				il.Emit(OpCodes.Call, dyn);
				il.Emit(OpCodes.Stloc_2);
				
				il.MarkLabel(end); //end:
				
				il.BeginFinallyBlock(); //}finally{
				
				il.Emit(OpCodes.Ldloc_1); //loc_1.Dispose();
				il.Emit(OpCodes.Callvirt, tsr.GetMethod("Dispose"));
				
				il.EndExceptionBlock(); //}
				
				il.Emit(OpCodes.Ldloc_2); //return loc_2;
				il.Emit(OpCodes.Ret);
				
				return (Del)dyn.CreateDelegate(typeof(Del));
			}
		}
	}
}
