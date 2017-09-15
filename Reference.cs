/* Date: 29.5.2015, Time: 23:19 */
using System;
using System.Reflection;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Interop;
using IllidanS4.SharpUtils.Reflection.Linq;

namespace IllidanS4.SharpUtils
{
	/// <summary>
	/// Various tools to operate on references.
	/// </summary>
	public static class Reference
	{
		/// <summary>
		/// Compares two references for equality.
		/// </summary>
		/// <param name="a">The first reference.</param>
		/// <param name="b">The second reference.</param>
		/// <returns>true if they are equal (point to the same location).</returns>
		public static bool Equals<T>(out T a, out T b)
		{
			return CacheHelper<T>.EqualsRef(out a, out b);
		}
		
		/// <summary>
		/// Checks whether the passed variable reference is null or not.
		/// </summary>
		/// <param name="r">The reference to check.</param>
		/// <returns>True is the passed reference is null.</returns>
		public static bool IsNull<T>(out T r)
		{
			return CacheHelper<T>.IsNull(out r);
		}
		
		/// <summary>
		/// Throws an exception if the parameter is a null variable reference.
		/// </summary>
		/// <param name="r">The reference.</param>
		/// <param name="parameter">The name of the parameter.</param>
		public static void CheckNull<T>(out T r, string parameter)
		{
			if(IsNull(out r)) throw new ArgumentNullException(parameter);
		}
		
		/// <summary>
		/// Fills the reference with the default value of its type.
		/// </summary>
		/// <param name="variable">The reference to fill.</param>
		public static void Fill<T>(out T variable)
		{
			variable = default(T);
		}
		
		/// <summary>
		/// Marks an "out" reference "used", so its content can be accessed, without having to assign its value.
		/// </summary>
		/// <param name="variable">The reference to "use".</param>
		public static void Use<T>(out T variable)
		{
			CacheHelper<T>.Use(out variable);
		}
		
		/// <summary>
		/// Creates a null reference.
		/// </summary>
		/// <param name="act">The action to receive the reference.</param>
		public static void Null<T>(RefAction<T> act)
		{
			CacheHelper<T>.LoadNull(act);
		}
		
		/// <summary>
		/// Creates a null reference.
		/// </summary>
		/// <param name="func">The function to receive the reference.</param>
		/// <returns>The value returned by <paramref name="func"/></returns>
		public static TRet Null<T, TRet>(RefFunc<T, TRet> func)
		{
			return CacheHelper<T>.WithRet<TRet>.LoadNull(func);
		}
		
		/// <summary>
		/// Obtains the reference to a boxed value.
		/// </summary>
		/// <param name="boxed">The boxed value.</param>
		/// <param name="act">The action to receive the reference.</param>
		public static void GetBoxedData<T>(ValueType boxed, RefAction<T> act) where T : struct
		{
			CacheHelper<T>.Unbox(boxed, act);
		}
		
		/// <summary>
		/// Obtains the reference to a boxed value.
		/// </summary>
		/// <param name="boxed">The boxed value.</param>
		/// <param name="func">The function to receive the reference.</param>
		/// <returns>The value returned by <paramref name="func"/></returns>
		public static TRet GetBoxedData<T, TRet>(ValueType boxed, RefFunc<T, TRet> func) where T : struct
		{
			return CacheHelper<T>.WithRet<TRet>.Unbox(boxed, func);
		}
		
		/// <summary>
		/// Pins an "out" reference, so its internal address doesn't change.
		/// </summary>
		/// <param name="variable">The reference.</param>
		/// <param name="act">The action to receive the pinned reference.</param>
		public static void Pin<T>(out T variable, OutAction<T> act)
		{
			CacheHelper<T>.Pin(out variable, OutToRefAction(act));
		}
		
		/// <summary>
		/// Pins a "ref" reference, so its internal address doesn't change.
		/// </summary>
		/// <param name="variable">The reference.</param>
		/// <param name="act">The action to receive the pinned reference.</param>
		public static void Pin<T>(ref T variable, RefAction<T> act)
		{
			CacheHelper<T>.Pin(out variable, act);
		}
		
		/// <summary>
		/// Pins an "out" reference, so its internal address doesn't change.
		/// </summary>
		/// <param name="variable">The reference.</param>
		/// <param name="func">The function to receive the pinned reference.</param>
		/// <returns></returns>
		/// <returns>The value returned by <paramref name="func"/></returns>
		public static TRet Pin<T, TRet>(out T variable, OutFunc<T, TRet> func)
		{
			return CacheHelper<T>.WithRet<TRet>.Pin(out variable, OutToRefFunc(func));
		}
		
		/// <summary>
		/// Pins a "ref" reference, so its internal address doesn't change.
		/// </summary>
		/// <param name="variable">The reference.</param>
		/// <param name="func">The function to receive the pinned reference.</param>
		/// <returns></returns>
		/// <returns>The value returned by <paramref name="func"/></returns>
		public static TRet Pin<T, TRet>(ref T variable, RefFunc<T, TRet> func)
		{
			return CacheHelper<T>.WithRet<TRet>.Pin(out variable, func);
		}
		
		/// <summary>
		/// An action taking a single "ref" parameter.
		/// </summary>
		public delegate void RefAction<T>(ref T r);
		/// <summary>
		/// An action taking a single "out" parameter.
		/// </summary>
		public delegate void OutAction<T>(out T r);
		
		/// <summary>
		/// Passes an "out" parameter to an action taking a "ref" parameter.
		/// </summary>
		/// <param name="r">"out" reference.</param>
		/// <param name="act">The action taking the reference.</param>
		public static void OutToRef<T>(out T r, RefAction<T> act)
		{
			RefToOutAction<T>(act)(out r);
		}
		
		/// <summary>
		/// A function taking a single "ref" parameter.
		/// </summary>
		public delegate TRet RefFunc<T, out TRet>(ref T r);
		/// <summary>
		/// A function taking a single "out" parameter.
		/// </summary>
		public delegate TRet OutFunc<T, out TRet>(out T r);
		
		/// <summary>
		/// Passes an "out" parameter to a function taking a "ref" parameter.
		/// </summary>
		/// <param name="r">"out" reference.</param>
		/// <param name="func">The function taking the reference.</param>
		/// <returns>The value returned by <paramref name="func"/>.</returns>
		public static TRet OutToRef<T, TRet>(out T r, RefFunc<T, TRet> func)
		{
			return RefToOutFunc<T, TRet>(func)(out r);
		}
		
		/// <summary>
		/// Converts an action taking an "out" parameter to an action taking a "ref" parameter.
		/// </summary>
		/// <param name="act">The action to be converted.</param>
		/// <returns>The converted action.</returns>
		public static RefAction<T> OutToRefAction<T>(OutAction<T> act)
		{
			return (RefAction<T>)Delegate.CreateDelegate(TypeOf<RefAction<T>>.TypeID, act.Target, act.Method);
		}
		
		/// <summary>
		/// Converts an action taking an "ref" parameter to an action taking a "out" parameter.
		/// </summary>
		/// <param name="act">The action to be converted.</param>
		/// <returns>The converted action.</returns>
		public static OutAction<T> RefToOutAction<T>(RefAction<T> act)
		{
			return (OutAction<T>)Delegate.CreateDelegate(TypeOf<OutAction<T>>.TypeID, act.Target, act.Method);
		}
		
		/// <summary>
		/// Converts a function taking an "out" parameter to a function taking a "ref" parameter.
		/// </summary>
		/// <param name="func">The function to be converted.</param>
		/// <returns>The converted function.</returns>
		public static RefFunc<T, TRet> OutToRefFunc<T, TRet>(OutFunc<T, TRet> func)
		{
			return (RefFunc<T, TRet>)Delegate.CreateDelegate(TypeOf<RefFunc<T, TRet>>.TypeID, func.Target, func.Method);
		}
		
		/// <summary>
		/// Converts a function taking an "ref" parameter to a function taking a "out" parameter.
		/// </summary>
		/// <param name="func">The function to be converted.</param>
		/// <returns>The converted function.</returns>
		public static OutFunc<T, TRet> RefToOutFunc<T, TRet>(RefFunc<T, TRet> func)
		{
			return (OutFunc<T, TRet>)Delegate.CreateDelegate(TypeOf<OutFunc<T, TRet>>.TypeID, func.Target, func.Method);
		}
		
		
		
		internal static class CacheHelper<T>
		{
			private static readonly MethodInfo Invoke = typeof(RefAction<T>).GetMethod("Invoke");
			
			public static readonly OutAction<T> Use = RefToOutAction<T>(_Use);
			
			private static void _Use(ref T variable)
			{
				
			}
			
			public delegate bool EqualsDelegate(out T a, out T b);
			public static readonly EqualsDelegate EqualsRef = LinqEmit.CreateDynamicMethod<EqualsDelegate>(
				//return &a == &b
				OpCodes.Ldarg_0,
				OpCodes.Ldarg_1,
				OpCodes.Ceq,
				OpCodes.Ret
			);
			
			public delegate bool IsNullDelegate(out T variable);
			public static readonly IsNullDelegate IsNull = LinqEmit.CreateDynamicMethod<IsNullDelegate>(
				//return &variable == nullptr
				OpCodes.Ldarg_0,
				OpCodes.Ldc_I4_0,
				OpCodes.Conv_U,
				OpCodes.Ceq,
				OpCodes.Ret
			);
			
			public static readonly Action<RefAction<T>> LoadNull = LinqEmit.CreateDynamicMethod<Action<RefAction<T>>>(
				//act(ref *nullptr)
				OpCodes.Ldarg_0,
				OpCodes.Ldc_I4_0,
				OpCodes.Conv_U,
				new Instruction(OpCodes.Callvirt, Invoke),
				OpCodes.Ret
			);
			// Can be done in unsafe for simple types: ref *(int*)0
			// CLR is smart enough to throw NullReferenceException
			// if the reference is accessed.
			
			public static readonly Action<ValueType,RefAction<T>> Unbox = LinqEmit.CreateDynamicMethod<Action<ValueType,RefAction<T>>>(
				//act(ref (T)obj)
				OpCodes.Ldarg_1,
				OpCodes.Ldarg_0,
				new Instruction(OpCodes.Unbox, typeof(T)),
				new Instruction(OpCodes.Callvirt, Invoke),
				OpCodes.Ret
			);
			// NOTICE: The "unbox" IL instruction actually returns something that is called
			// "controlled-mutability managed pointer", meaning the reference is considered
			// read-only. The CLR doesn't attempt to check this at all.
			
			private static readonly MethodInfo ToPointer = typeof(IntPtr).GetMethod("ToPointer");
			
			public static readonly Action<IntPtr,RefAction<T>> FromPtr = LinqEmit.CreateDynamicMethod<Action<IntPtr,RefAction<T>>>(
				//act(ref *ptr)
				OpCodes.Ldarg_1,
				new Instruction(OpCodes.Ldarga_S, 0),
				new Instruction(OpCodes.Callvirt, ToPointer),
				new Instruction(OpCodes.Callvirt, Invoke),
				OpCodes.Ret
			);
			
			public unsafe delegate void PtrDel(void* ptr);
			public delegate void ToPtrDel(out T r, PtrDel act);
			
			public static readonly ToPtrDel ToPtr = LinqEmit.CreateDynamicMethod<ToPtrDel>(
				//act(&r)
				Instruction.DeclareLocal(typeof(T).MakeByRefType(), true),
				OpCodes.Ldarg_0,
				OpCodes.Stloc_0,
				OpCodes.Ldarg_1,
				OpCodes.Ldloc_0,
				OpCodes.Conv_I,
				new Instruction(OpCodes.Callvirt, typeof(PtrDel).GetMethod("Invoke")),
				OpCodes.Ldc_I4_0,
				OpCodes.Conv_U,
				OpCodes.Stloc_0,
				OpCodes.Ret
			);
			
			public delegate void PinDel(out T variable, RefAction<T> act);
			
			public static readonly PinDel Pin = LinqEmit.CreateDynamicMethod<PinDel>(
				Instruction.DeclareLocal(typeof(T).MakeByRefType(), true),
				OpCodes.Ldarg_0,
				OpCodes.Stloc_0,
				OpCodes.Ldarg_1,
				OpCodes.Ldloc_0,
				new Instruction(OpCodes.Callvirt, Invoke),
				OpCodes.Ret
			);
			
			public static class WithRet<TRet>
			{
				private static readonly MethodInfo Invoke = typeof(RefFunc<T,TRet>).GetMethod("Invoke");
			
				public static readonly Func<RefFunc<T,TRet>,TRet> LoadNull = LinqEmit.CreateDynamicMethod<Func<RefFunc<T,TRet>,TRet>>(
					OpCodes.Ldarg_0,
					OpCodes.Ldc_I4_0,
					OpCodes.Conv_U,
					new Instruction(OpCodes.Callvirt, Invoke),
					OpCodes.Ret
				);
			
				public static readonly Func<ValueType,RefFunc<T,TRet>,TRet> Unbox = LinqEmit.CreateDynamicMethod<Func<ValueType,RefFunc<T,TRet>,TRet>>(
					OpCodes.Ldarg_1,
					OpCodes.Ldarg_0,
					new Instruction(OpCodes.Unbox, typeof(T)),
					new Instruction(OpCodes.Callvirt, Invoke),
					OpCodes.Ret
				);
			
				public static readonly Func<IntPtr,RefFunc<T,TRet>,TRet> FromPtr = LinqEmit.CreateDynamicMethod<Func<IntPtr,RefFunc<T,TRet>,TRet>>(
					OpCodes.Ldarg_1,
					new Instruction(OpCodes.Ldarga_S, 0),
					new Instruction(OpCodes.Callvirt, ToPointer),
					OpCodes.Conv_U,
					new Instruction(OpCodes.Callvirt, Invoke),
					OpCodes.Ret
				);
				
				public unsafe delegate TRet PtrDel(void* ptr);
				public delegate TRet ToPtrDel(out T r, PtrDel act);
				
				public static readonly ToPtrDel ToPtr = LinqEmit.CreateDynamicMethod<ToPtrDel>(
					Instruction.DeclareLocal(typeof(T).MakeByRefType(), true),
					OpCodes.Ldarg_0,
					OpCodes.Stloc_0,
					OpCodes.Ldarg_1,
					OpCodes.Ldloc_0,
					OpCodes.Conv_I,
					new Instruction(OpCodes.Call, typeof(PtrDel).GetMethod("Invoke")),
					OpCodes.Ldc_I4_0,
					OpCodes.Conv_U,
					OpCodes.Stloc_0,
					OpCodes.Ret
				);
			
				public delegate TRet PinDel(out T variable, RefFunc<T, TRet> act);
				
				public static readonly PinDel Pin = LinqEmit.CreateDynamicMethod<PinDel>(
					Instruction.DeclareLocal(typeof(T).MakeByRefType(), true),
					OpCodes.Ldarg_0,
					OpCodes.Stloc_0,
					OpCodes.Ldarg_1,
					OpCodes.Ldloc_0,
					new Instruction(OpCodes.Call, Invoke),
					OpCodes.Ret
				);
			}
		}
	}
}
