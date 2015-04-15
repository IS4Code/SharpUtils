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
using System.Security.Permissions;
using IllidanS4.SharpUtils.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.TypeSupport;

namespace IllidanS4.SharpUtils.Reflection
{
	using SysEmit = System.Reflection.Emit;
	
	public static partial class ReflectionTools
	{
		public static Type GetBindingType(this CallSiteBinder binder)
		{
    		IDictionary<Type,object> cache = GetCache(binder);
	    	if(cache == null) return null;
	    	Type ftype = cache.Select(t => t.Key).FirstOrDefault(t => t != null && t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Func<,,,>));
	    	if(ftype == null) return null;
	    	Type[] genargs = ftype.GetGenericArguments();
	    	if(genargs == null || genargs.Length <= 2) return null;
	    	return genargs[2];
		}
    	
    	public static Type NewCustomDelegateType(Type ret, params Type[] parameters)
		{
			Type[] args = new Type[parameters.Length+1];
			parameters.CopyTo(args, 0);
			args[args.Length-1] = ret;
			return MakeNewCustomDelegate(args);
		}
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
			fixed(byte* ptr = signature)
			{
				object sig = SignatureCreator(ptr, signature.Length, declaringType);
				return GetSignatureType(sig);
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
		
		public static readonly Func<IntPtr,Type> GetTypeFromHandle = Hacks.GetInvoker<Func<IntPtr,Type>>(typeof(Type), "GetTypeFromHandleUnsafe", false);
		
		public static CorElementType GetCorElementType(this Type type)
		{
			TypeConstruct tc = type as TypeConstruct;
			if(tc != null)
			{
				return tc.CorElementType;
			}else{
				return GetCorElType.Invoke(type);
			}
		}
		
		private static readonly Func<Type,CorElementType> GetCorElType = Hacks.GetInvoker<Func<Type,CorElementType>>(typeof(RuntimeTypeHandle), "GetCorElementType", false);

		public static ModifiedType MakeModifiedType(this Type type, params TypeModifier[] modifiers)
		{
			return new ModifiedType(type, modifiers);
		}
		
		public static byte[] GetSignature(this FieldInfo fld)
		{
			object metadata = GetSignatureHacks.GetMetadataImport(fld.Module);
			object sig = GetSignatureHacks.GetSigOfFieldDef(metadata, fld.MetadataToken);
			IntPtr ptr = GetSignatureHacks.GetSignaturePtr(sig);
			int len = GetSignatureHacks.GetSignatureLength(sig);
			byte[] data = new byte[len];
			Marshal.Copy(ptr, data, 0, len);
			return data;
		}
		
		public static MethodSignature GetDelegateSignature(this Type tDelegate)
		{
			return MethodSignature.FromDelegateType(tDelegate);
		}
		
		#region OpCodes
		
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
		
		public static void EmitCalli(this ILGenerator il, OpCode opcode, MethodSignature signature)
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
		
		#endregion
		
		public static Type GetTypeFromElementType(CorElementType elementType)
		{
			switch(elementType)
			{
				case CorElementType.SzArray:
				case CorElementType.Array:
					return TypeOf<Array>.TypeID;
				case CorElementType.Boolean:
					return TypeOf<bool>.TypeID;
				case CorElementType.FnPtr:
					return Types.Generated.FnPtr;
				case CorElementType.Char:
					return TypeOf<char>.TypeID;
				case CorElementType.I:
					return TypeOf<IntPtr>.TypeID;
				case CorElementType.I1:
					return TypeOf<sbyte>.TypeID;
				case CorElementType.I2:
					return TypeOf<short>.TypeID;
				case CorElementType.I4:
					return TypeOf<int>.TypeID;
				case CorElementType.I8:
					return TypeOf<long>.TypeID;
				case CorElementType.Object:
					return TypeOf<object>.TypeID;
				case CorElementType.Ptr:
					return typeof(void*);
				case CorElementType.R:
					return null;
				case CorElementType.R4:
					return TypeOf<float>.TypeID;
				case CorElementType.R8:
					return TypeOf<double>.TypeID;
				case CorElementType.String:
					return TypeOf<string>.TypeID;
				case CorElementType.TypedByRef:
					return typeof(TypedReference);
				case CorElementType.U:
					return TypeOf<UIntPtr>.TypeID;
				case CorElementType.U1:
					return TypeOf<byte>.TypeID;
				case CorElementType.U2:
					return TypeOf<ushort>.TypeID;
				case CorElementType.U4:
					return TypeOf<uint>.TypeID;
				case CorElementType.U8:
					return TypeOf<ulong>.TypeID;
				default:
					return null;
			}
		}
		
		public static Type MakePartialGenericType(this Type type, IDictionary<int, Type> typeArgs)
		{
			return new PartialGenericType(type, typeArgs);
		}
		
		#region RuntimeMethodHandle extensions
		
		#endregion
	}
}
