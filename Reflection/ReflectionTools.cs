/* Date: 12.11.2014, Time: 15:32 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection
{
	using SysEmit = System.Reflection.Emit;
	
	public static class ReflectionTools
	{
		private static readonly FieldInfo CallSiteBinder_Cache = typeof(CallSiteBinder).GetField("Cache", BindingFlags.NonPublic | BindingFlags.Instance);
    	public static Type GetBindingType(this CallSiteBinder binder)
		{
	    	IDictionary<Type,object> cache = (IDictionary<Type,object>)CallSiteBinder_Cache.GetValue(binder);
	    	if(cache == null) return null;
	    	Type ftype = cache.Select(t => t.Key).FirstOrDefault(t => t != null && t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Func<,,,>));
	    	if(ftype == null) return null;
	    	Type[] genargs = ftype.GetGenericArguments();
	    	if(genargs == null || genargs.Length <= 2) return null;
	    	return genargs[2];
		}
    	
    	private static readonly Type DelegateHelpersType = typeof(Expression).Assembly.GetType("System.Linq.Expressions.Compiler.DelegateHelpers");
    	private static readonly Func<Type[],Type> MakeNewCustomDelegate = (Func<Type[],Type>)Delegate.CreateDelegate(typeof(Func<Type[],Type>), DelegateHelpersType.GetMethod("MakeNewCustomDelegate", BindingFlags.NonPublic | BindingFlags.Static));
		public static Type NewCustomDelegateType(Type ret, params Type[] parameters)
		{
			Type[] args = new Type[parameters.Length+1];
			parameters.CopyTo(args, 0);
			args[args.Length-1] = ret;
			return MakeNewCustomDelegate(args);
		}
    	private static readonly Func<Type[],Type> MakeNewDelegate = (Func<Type[],Type>)Delegate.CreateDelegate(typeof(Func<Type[],Type>), DelegateHelpersType.GetMethod("MakeNewDelegate", BindingFlags.NonPublic | BindingFlags.Static));
		public static Type GetDelegateType(Type ret, params Type[] parameters)
		{
			Type[] args = new Type[parameters.Length+1];
			parameters.CopyTo(args, 0);
			args[args.Length-1] = ret;
			try{
				return MakeNewDelegate(args);
			}catch{
				return MakeNewCustomDelegate(args);
			}
		}
		
		public static unsafe Type GetTypeFromFieldSignature(byte[] signature, Type declaringType = null)
		{
			declaringType = declaringType ?? typeof(object);
			Type sigtype = typeof(Type).Module.GetType("System.Signature");
			Type rtype = typeof(Type).Module.GetType("System.RuntimeType");
			var ctor = sigtype.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[]{typeof(void*), typeof(int), rtype}, null);
			fixed(byte* ptr = signature)
			{
				object sigobj = ctor.Invoke(new object[]{(IntPtr)ptr, signature.Length, declaringType});
				return (Type)sigtype.InvokeMember("FieldType", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, sigobj, null);
			}
		}
		
		public static void SetValue<T>(this FieldInfo fi, ref T obj, object value)
		{
			fi.SetValueDirect(__makeref(obj), value);
		}
		
		public static object GetValue<T>(this FieldInfo fi, ref T obj)
		{
			return fi.GetValueDirect(__makeref(obj));
		}
		
		public static readonly Func<IntPtr,Type> GetTypeFromHandle = (Func<IntPtr,Type>)Delegate.CreateDelegate(typeof(Func<IntPtr,Type>), typeof(Type).GetMethod("GetTypeFromHandleUnsafe", BindingFlags.Static | BindingFlags.NonPublic));
		
		public static byte[] GetSignature(this FieldInfo fld)
		{
			var rmtype = fld.Module.GetType();
			var mimp = rmtype.InvokeMember("MetadataImport", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, fld.Module, null);
			var sig = mimp.GetType().InvokeMember("GetSigOfFieldDef", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, mimp, new object[]{fld.MetadataToken});
			var ctype = sig.GetType();
			IntPtr ptr = (IntPtr)ctype.InvokeMember("Signature", BindingFlags.GetProperty, null, sig, null);
			int len = (int)ctype.InvokeMember("Length", BindingFlags.GetProperty, null, sig, null);
			byte[] data = new byte[len];
			Marshal.Copy(ptr, data, 0, len);
			return data;
		}
		
		public static void EmitLdarg(this ILGenerator il, int index)
		{
			if(index <= 3)
			{
				il.Emit(ReflectionTools.OpCodes.Ldarg[index]);
			}else if(index <= 255)
			{
				il.Emit(SysEmit.OpCodes.Ldarg_S, (byte)index);
			}else{
				il.Emit(SysEmit.OpCodes.Ldarg, (ushort)index);
			}
		}
		
		public static void EmitCalli(this ILGenerator il, OpCode opcode, MethodCallSite signature)
		{
			if(signature.IsUnmanaged)
			{
				il.EmitCalli(opcode, signature.UnmanagedCallingConvention, signature.ReturnType, signature.ParameterTypes);
			}else{
				il.EmitCalli(opcode, signature.CallingConvention, signature.ReturnType, signature.ParameterTypes, signature.OptionalParameterTypes);
			}
		}
		
		public sealed class OpCodes
		{
			private readonly OpCode[] opcodes;
			private readonly int offset;
			public OpCode this[int index]
			{
				get{
					return opcodes[index+offset];
				}
			}
			
			private OpCodes(OpCode[] array) : this(array, 0)
			{
				
			}
			
			private OpCodes(OpCode[] array, int offset)
			{
				opcodes = array;
				this.offset = offset;
			}
			
			public static implicit operator OpCodes(OpCode[] arr)
			{
				return new OpCodes(arr);
			}
			
			public static readonly OpCodes Ldarg = new OpCode[]{
				SysEmit.OpCodes.Ldarg_0,
				SysEmit.OpCodes.Ldarg_1,
				SysEmit.OpCodes.Ldarg_2,
				SysEmit.OpCodes.Ldarg_3,
			};
			
			
			public static readonly OpCodes Ldc_I4 = new OpCodes(
				new OpCode[]{
					SysEmit.OpCodes.Ldc_I4_M1,
					SysEmit.OpCodes.Ldc_I4_0,
					SysEmit.OpCodes.Ldc_I4_1,
					SysEmit.OpCodes.Ldc_I4_2,
					SysEmit.OpCodes.Ldc_I4_3,
					SysEmit.OpCodes.Ldc_I4_4,
					SysEmit.OpCodes.Ldc_I4_5,
					SysEmit.OpCodes.Ldc_I4_6,
					SysEmit.OpCodes.Ldc_I4_7,
					SysEmit.OpCodes.Ldc_I4_8,
				}, 1
			);
			
			public static readonly OpCodes Ldloc = new OpCode[]{
				SysEmit.OpCodes.Ldloc_0,
				SysEmit.OpCodes.Ldloc_1,
				SysEmit.OpCodes.Ldloc_2,
				SysEmit.OpCodes.Ldloc_3,
			};
			
			public static readonly OpCodes Stloc = new OpCode[]{
				SysEmit.OpCodes.Stloc_0,
				SysEmit.OpCodes.Stloc_1,
				SysEmit.OpCodes.Stloc_2,
				SysEmit.OpCodes.Stloc_3,
			};
		}
	}
}
