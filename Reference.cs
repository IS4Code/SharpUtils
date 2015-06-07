/* Date: 29.5.2015, Time: 23:19 */
using System;
using System.Reflection;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Interop;
using IllidanS4.SharpUtils.Reflection.Linq;

namespace IllidanS4.SharpUtils
{
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
		
		public static bool IsNull<T>(out T r)
		{
			return CacheHelper<T>.IsNull(out r);
		}
		
		public static void CheckNull<T>(out T r, string parameter)
		{
			if(IsNull(out r)) throw new ArgumentNullException(parameter);
		}
		
		public static void Fill<T>(out T variable)
		{
			variable = default(T);
		}
		
		public static void Use<T>(out T variable)
		{
			CacheHelper<T>.Use(out variable);
		}
		
		public static void Null<T>(RefAction<T> act)
		{
			CacheHelper<T>.LoadNull(act);
		}
		
		public static TRet Null<T, TRet>(RefFunc<T, TRet> func)
		{
			return CacheHelper<T>.WithRet<TRet>.LoadNull(func);
		}
		
		public static void GetBoxedData<T>(ValueType boxed, RefAction<T> act) where T : struct
		{
			CacheHelper<T>.Unbox(boxed, act);
		}
		
		public static TRet GetBoxedData<T, TRet>(ValueType boxed, RefFunc<T, TRet> func) where T : struct
		{
			return CacheHelper<T>.WithRet<TRet>.Unbox(boxed, func);
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
				OpCodes.Ldarg_0,
				OpCodes.Ldarg_1,
				OpCodes.Ceq,
				OpCodes.Ret
			);
			
			public delegate bool IsNullDelegate(out T variable);
			public static readonly IsNullDelegate IsNull = LinqEmit.CreateDynamicMethod<IsNullDelegate>(
				OpCodes.Ldarg_0,
				OpCodes.Ldc_I4_0,
				OpCodes.Conv_U,
				OpCodes.Ceq,
				OpCodes.Ret
			);
			
			public static readonly Action<RefAction<T>> LoadNull = LinqEmit.CreateDynamicMethod<Action<RefAction<T>>>(
				OpCodes.Ldarg_0,
				OpCodes.Ldc_I4_0,
				OpCodes.Conv_U,
				new Instruction(OpCodes.Call, Invoke),
				OpCodes.Ret
			);
			
			public static readonly Action<ValueType,RefAction<T>> Unbox = LinqEmit.CreateDynamicMethod<Action<ValueType,RefAction<T>>>(
				OpCodes.Ldarg_1,
				OpCodes.Ldarg_0,
				new Instruction(OpCodes.Unbox, typeof(T)),
				new Instruction(OpCodes.Call, Invoke),
				OpCodes.Ret
			);
			
			private static readonly MethodInfo ToPointer = typeof(IntPtr).GetMethod("ToPointer");
			
			public static readonly Action<IntPtr,RefAction<T>> FromPtr = LinqEmit.CreateDynamicMethod<Action<IntPtr,RefAction<T>>>(
				OpCodes.Ldarg_1,
				OpCodes.Ldarg_0,
				new Instruction(OpCodes.Call, ToPointer),
				new Instruction(OpCodes.Call, Invoke),
				OpCodes.Ret
			);
			
			public unsafe delegate void PtrDel(void* ptr);
			public delegate void ToPtrDel(out T r, PtrDel act);
			
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
			
			public static class WithRet<TRet>
			{
				private static readonly MethodInfo Invoke = typeof(RefFunc<T,TRet>).GetMethod("Invoke");
			
				public static readonly Func<RefFunc<T,TRet>,TRet> LoadNull = LinqEmit.CreateDynamicMethod<Func<RefFunc<T,TRet>,TRet>>(
					OpCodes.Ldarg_0,
					OpCodes.Ldc_I4_0,
					OpCodes.Conv_U,
					new Instruction(OpCodes.Call, Invoke),
					OpCodes.Ret
				);
			
				public static readonly Func<ValueType,RefFunc<T,TRet>,TRet> Unbox = LinqEmit.CreateDynamicMethod<Func<ValueType,RefFunc<T,TRet>,TRet>>(
					OpCodes.Ldarg_1,
					OpCodes.Ldarg_0,
					new Instruction(OpCodes.Unbox, typeof(T)),
					new Instruction(OpCodes.Call, Invoke),
					OpCodes.Ret
				);
			
				public static readonly Func<IntPtr,RefFunc<T,TRet>,TRet> FromPtr = LinqEmit.CreateDynamicMethod<Func<IntPtr,RefFunc<T,TRet>,TRet>>(
					OpCodes.Ldarg_1,
					OpCodes.Ldarg_0,
					new Instruction(OpCodes.Call, ToPointer),
					new Instruction(OpCodes.Call, Invoke),
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
			}
		}
		
		public delegate void RefAction<T>(ref T r);
		public delegate void OutAction<T>(out T r);
		public static void OutToRef<T>(out T r, RefAction<T> act)
		{
			RefToOutAction<T>(act)(out r);
		}
		
		public delegate TRet RefFunc<T, out TRet>(ref T r);
		public delegate TRet OutFunc<T, out TRet>(out T r);
		public static TRet OutToRef<T, TRet>(out T r, RefFunc<T, TRet> act)
		{
			return RefToOutFunc<T, TRet>(act)(out r);
		}
		
		public static RefAction<T> OutToRefAction<T>(OutAction<T> act)
		{
			return (RefAction<T>)Delegate.CreateDelegate(TypeOf<RefAction<T>>.TypeID, act.Target, act.Method);
		}
		
		public static OutAction<T> RefToOutAction<T>(RefAction<T> act)
		{
			return (OutAction<T>)Delegate.CreateDelegate(TypeOf<OutAction<T>>.TypeID, act.Target, act.Method);
		}
		
		public static RefFunc<T, TRet> OutToRefFunc<T, TRet>(OutFunc<T, TRet> func)
		{
			return (RefFunc<T, TRet>)Delegate.CreateDelegate(TypeOf<RefFunc<T, TRet>>.TypeID, func.Target, func.Method);
		}
		
		public static OutFunc<T, TRet> RefToOutFunc<T, TRet>(RefFunc<T, TRet> func)
		{
			return (OutFunc<T, TRet>)Delegate.CreateDelegate(TypeOf<OutFunc<T, TRet>>.TypeID, func.Target, func.Method);
		}
	}
}
