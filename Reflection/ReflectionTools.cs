/* Date: 12.11.2014, Time: 15:32 */
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using IllidanS4.SharpUtils.Interop;
using IllidanS4.SharpUtils.Metadata;
using IllidanS4.SharpUtils.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.Linq;
using IllidanS4.SharpUtils.Reflection.TypeSupport;
using IllidanS4.SharpUtils.Unsafe;
using Microsoft.CSharp.RuntimeBinder;

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
		
		public static IEnumerable<Type> GetTypeArguments(this InvokeMemberBinder binder)
		{
			return GetTypeArgs(binder);
		}
		
		public static IEnumerable<CSharpArgumentInfo> GetArgumentInfo(this CallSiteBinder binder)
		{
			var fi = binder.GetType().GetField("m_argumentInfo", BindingFlags.NonPublic | BindingFlags.Instance);
			if(fi != null)
			{
				return (IEnumerable<CSharpArgumentInfo>)fi.GetValue(binder);
			}else{
				return null;
			}
		}
		
		public static CSharpBinderFlags GetBinderFlags(this CallSiteBinder binder)
		{
			var fi = binder.GetType().GetField("m_flags", BindingFlags.NonPublic | BindingFlags.Instance);
			if(fi != null)
			{
				return EnumTools.Parse<CSharpBinderFlags>(fi.GetValue(binder).ToString());
			}else{
				return 0;
			}
		}
		
		public static Type GetCallingContext(this CallSiteBinder binder)
		{
			var fi = binder.GetType().GetField("m_callingContext", BindingFlags.NonPublic | BindingFlags.Instance);
			if(fi != null)
			{
				return (Type)fi.GetValue(binder);
			}else{
				return null;
			}
		}
		
		public static CSharpArgumentInfoFlags GetFlags(this CSharpArgumentInfo argInfo)
		{
			return GetArgFlags(argInfo);
		}
		
		public static string GetName(this CSharpArgumentInfo argInfo)
		{
			return GetArgName(argInfo);
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
		
		public static void SetValueDirect(this FieldInfo fi, SafeReference sr, object value)
		{
			sr.GetReference(tr => fi.SetValueDirect(tr, value));
		}
		
		public static object GetValueDirect(this FieldInfo fi, SafeReference sr)
		{
			return sr.GetReference(tr => fi.GetValueDirect(tr));
		}
		
		public static void SetValue<T>(this FieldInfo fi, ref T obj, object value) where T : struct
		{
			fi.SetValueDirect(__makeref(obj), value);
		}
		
		public static object GetValue<T>(this FieldInfo fi, ref T obj) where T : struct
		{
			return fi.GetValueDirect(__makeref(obj));
		}
		
		public static unsafe void SetValue<TField>(this FieldInfo fi, object obj, TField value)
		{
			TypedReference tr;
			TypedReferenceTools.MakeTypedReference(&tr, obj, fi);
			__refvalue(tr, TField) = value;
		}
		
		public static unsafe TField GetValue<TField>(this FieldInfo fi, object obj)
		{
			TypedReference tr;
			TypedReferenceTools.MakeTypedReference(&tr, obj, fi);
			return __refvalue(tr, TField);
		}
		
		public static void SetValue<T, TField>(this FieldInfo fi, ref T obj, TField value) where T : struct
		{
			FieldHelper<T, TField>.SetValue(fi.GetOffset(), ref obj, value);
		}
		
		public static TField GetValue<T, TField>(this FieldInfo fi, ref T obj) where T : struct
		{
			return FieldHelper<T, TField>.GetValue(fi.GetOffset(), ref obj);
		}
		
		private static class FieldHelper<T, TField> where T : struct
		{
			public delegate void Set(int offset, ref T obj, TField value);
			public delegate TField Get(int offset, ref T obj);
			
			public static readonly Set SetValue = LinqEmit.CreateDynamicMethod<Set>(
				SysEmit.OpCodes.Ldarg_1,
				SysEmit.OpCodes.Ldarg_0,
				SysEmit.OpCodes.Add,
				SysEmit.OpCodes.Ldarg_2,
				new Instruction(SysEmit.OpCodes.Stobj, TypeOf<TField>.TypeID),
				SysEmit.OpCodes.Ret
			);
			
			public static readonly Get GetValue = LinqEmit.CreateDynamicMethod<Get>(
				SysEmit.OpCodes.Ldarg_1,
				SysEmit.OpCodes.Ldarg_0,
				SysEmit.OpCodes.Add,
				new Instruction(SysEmit.OpCodes.Ldobj, TypeOf<TField>.TypeID),
				SysEmit.OpCodes.Ret
			);
		}
		
		[CLSCompliant(false)]
		public static void SetValueDirect<TField>(this FieldInfo fi, TypedReference tr, TField value)
		{
			FieldRefHelper<TField>.SetValue(fi.GetOffset(), tr, value);
		}
		
		[CLSCompliant(false)]
		public static TField GetValueDirect<TField>(this FieldInfo fi, TypedReference tr)
		{
			return FieldRefHelper<TField>.GetValue(fi.GetOffset(), tr);
		}
		
		public static void SetValueDirect<TField>(this FieldInfo fi, SafeReference sr, TField value)
		{
			sr.GetReference(tr => SetValueDirect<TField>(fi, tr, value));
		}
		
		public static TField GetValueDirect<TField>(this FieldInfo fi, SafeReference sr)
		{
			return sr.GetReference(tr => GetValueDirect<TField>(fi, tr));
		}
		
		private static class FieldRefHelper<TField>
		{
			public delegate void Set(int offset, TypedReference tr, TField value);
			public delegate TField Get(int offset, TypedReference tr);
			
			public static readonly Set SetValue = LinqEmit.CreateDynamicMethod<Set>(
				new Instruction(SysEmit.OpCodes.Ldarga_S, 1),
				SysEmit.OpCodes.Ldind_I,
				SysEmit.OpCodes.Conv_U,
				SysEmit.OpCodes.Ldarg_0,
				SysEmit.OpCodes.Add,
				SysEmit.OpCodes.Ldarg_2,
				new Instruction(SysEmit.OpCodes.Stobj, TypeOf<TField>.TypeID),
				SysEmit.OpCodes.Ret
			);
			
			public static readonly Get GetValue = LinqEmit.CreateDynamicMethod<Get>(
				new Instruction(SysEmit.OpCodes.Ldarga_S, 1),
				SysEmit.OpCodes.Ldind_I,
				SysEmit.OpCodes.Conv_U,
				SysEmit.OpCodes.Ldarg_0,
				SysEmit.OpCodes.Add,
				new Instruction(SysEmit.OpCodes.Ldobj, TypeOf<TField>.TypeID),
				SysEmit.OpCodes.Ret
			);
		}
		
		//TODO Type safety (TProperty == pi.PropertyType)
		public static void SetValue<TType, TProperty>(this PropertyInfo pi, ref TType obj, TProperty value) where TType : struct
		{
			PropertyHelper<TType, TProperty>.SetValue(pi.GetSetMethod(true).MethodHandle.GetFunctionPointer(), ref obj, value);
		}
		
		public static TProperty GetValue<TType, TProperty>(this PropertyInfo pi, ref TType obj) where TType : struct
		{
			return PropertyHelper<TType, TProperty>.GetValue(pi.GetGetMethod(true).MethodHandle.GetFunctionPointer(), ref obj);
		}
		
		private static class PropertyHelper<T, TProperty> where T : struct
		{
			public delegate void Set(IntPtr method, ref T obj, TProperty value);
			public delegate TProperty Get(IntPtr method, ref T obj);
			
			public static readonly Set SetValue = LinqEmit.CreateDynamicMethod<Set>(
				SysEmit.OpCodes.Ldarg_1,
				SysEmit.OpCodes.Ldarg_2,
				SysEmit.OpCodes.Ldarg_0,
				new Instruction(SysEmit.OpCodes.Calli, CallingConventions.Standard, Types.Void, new[]{TypeOf<T>.TypeID.MakeByRefType(), TypeOf<TProperty>.TypeID}, null),
				SysEmit.OpCodes.Ret
			);
			
			public static readonly Get GetValue = LinqEmit.CreateDynamicMethod<Get>(
				SysEmit.OpCodes.Ldarg_1,
				SysEmit.OpCodes.Ldarg_0,
				new Instruction(SysEmit.OpCodes.Calli, CallingConventions.Standard, TypeOf<TProperty>.TypeID, new[]{TypeOf<T>.TypeID.MakeByRefType()}, null),
				SysEmit.OpCodes.Ret
			);
		}
		
		[CLSCompliant(false)]
		public static void SetValueDirect<TProperty>(this PropertyInfo pi, TypedReference tr, TProperty value)
		{
			PropertyRefHelper<TProperty>.SetValue(pi.GetSetMethod(true).MethodHandle.GetFunctionPointer(), tr, value);
		}
		
		[CLSCompliant(false)]
		public static TProperty GetValueDirect<TProperty>(this PropertyInfo pi, TypedReference tr)
		{
			return PropertyRefHelper<TProperty>.GetValue(pi.GetGetMethod(true).MethodHandle.GetFunctionPointer(), tr);
		}
		
		internal static void SetValueDirect<TProperty>(this PropertyInfo pi, [Boxed(typeof(TypedReference))]ValueType tr, TProperty value)
		{
			SetValueDirect<TProperty>(pi, (TypedReference)tr, value);
		}
		
		internal static TProperty GetValueDirect<TProperty>(this PropertyInfo pi, [Boxed(typeof(TypedReference))]ValueType tr)
		{
			return GetValueDirect<TProperty>(pi, (TypedReference)tr);
		}
		
		private static class PropertyRefHelper<TProperty>
		{
			public delegate void Set(IntPtr method, TypedReference tr, TProperty value);
			public delegate TProperty Get(IntPtr method, TypedReference tr);
			
			public static readonly Set SetValue = LinqEmit.CreateDynamicMethod<Set>(
				new Instruction(SysEmit.OpCodes.Ldarga_S, 1),
				SysEmit.OpCodes.Ldind_I,
				SysEmit.OpCodes.Conv_U,
				SysEmit.OpCodes.Ldarg_2,
				SysEmit.OpCodes.Ldarg_0,
				new Instruction(SysEmit.OpCodes.Calli, CallingConventions.Standard, Types.Void, new[]{typeof(TypedReference), TypeOf<TProperty>.TypeID}, null),
				SysEmit.OpCodes.Ret
			);
			
			public static readonly Get GetValue = LinqEmit.CreateDynamicMethod<Get>(
				new Instruction(SysEmit.OpCodes.Ldarga_S, 1),
				SysEmit.OpCodes.Ldind_I,
				SysEmit.OpCodes.Conv_U,
				SysEmit.OpCodes.Ldarg_0,
				new Instruction(SysEmit.OpCodes.Calli, CallingConventions.Standard, TypeOf<TProperty>.TypeID, new[]{typeof(TypedReference)}, null),
				SysEmit.OpCodes.Ret
			);
		}
		
		public static unsafe int GetOffset(this FieldInfo field)
		{
			return InteropTools.Pin(
				FormatterServices.GetUninitializedObject(field.DeclaringType),
				delegate(object inst)
				{
					TypedReference tr0, tr1;
					TypedReferenceTools.MakeTypedReference(&tr0, inst);
					TypedReferenceTools.MakeTypedReference(&tr1, inst, field);
					byte* p0 = (byte*)tr0.ToPointer();
					byte* p1 = (byte*)tr1.ToPointer();
					return (int)(p1-p0);
				}
			);
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
		
		/*#region RuntimeMethodHandle extensions
		
		#endregion*/
		
		public static StackValueType[] GetTypes(this StackBehaviour stackBehaviour)
		{
			switch(stackBehaviour)
			{
				case StackBehaviour.Push0:
				case StackBehaviour.Pop0: return new StackValueType[0];
				
				case StackBehaviour.Push1:
				case StackBehaviour.Pop1: return new[]{StackValueType.Val};
				
				case StackBehaviour.Push1_push1:
				case StackBehaviour.Pop1_pop1: return new[]{StackValueType.Val, StackValueType.Val};
				
				case StackBehaviour.Pushi:
				case StackBehaviour.Popi: return new[]{StackValueType.I};
				
				case StackBehaviour.Popi_pop1: return new[]{StackValueType.I, StackValueType.Val};
				case StackBehaviour.Popi_popi: return new[]{StackValueType.I, StackValueType.I};
				case StackBehaviour.Popi_popi8: return new[]{StackValueType.I, StackValueType.I8};
				case StackBehaviour.Popi_popi_popi: return new[]{StackValueType.I, StackValueType.I, StackValueType.I};
				case StackBehaviour.Popi_popr4: return new[]{StackValueType.I, StackValueType.R4};
				case StackBehaviour.Popi_popr8: return new[]{StackValueType.I, StackValueType.R8};
				case StackBehaviour.Pushref:
				case StackBehaviour.Popref: return new[]{StackValueType.Ref};
				case StackBehaviour.Popref_pop1: return new[]{StackValueType.Ref, StackValueType.Val};
				case StackBehaviour.Popref_popi: return new[]{StackValueType.Ref, StackValueType.I};
				case StackBehaviour.Popref_popi_pop1: return new[]{StackValueType.Ref, StackValueType.I, StackValueType.Val};
				case StackBehaviour.Popref_popi_popi: return new[]{StackValueType.Ref, StackValueType.I, StackValueType.I};
				case StackBehaviour.Popref_popi_popi8: return new[]{StackValueType.Ref, StackValueType.I, StackValueType.I8};
				case StackBehaviour.Popref_popi_popr4: return new[]{StackValueType.Ref, StackValueType.I, StackValueType.R4};
				case StackBehaviour.Popref_popi_popr8: return new[]{StackValueType.Ref, StackValueType.I, StackValueType.R8};
				case StackBehaviour.Popref_popi_popref: return new[]{StackValueType.Ref, StackValueType.I, StackValueType.Ref};
				
				case StackBehaviour.Pushi8: return new[]{StackValueType.I8};
				case StackBehaviour.Pushr4: return new[]{StackValueType.R4};
				case StackBehaviour.Pushr8: return new[]{StackValueType.R8};
				case StackBehaviour.Varpop: return null;
				case StackBehaviour.Varpush: return null;
				
				default: throw new ArgumentException("stackBehaviour");
			}
		}
		
		public static int GetCount(this StackBehaviour stackBehaviour)
		{
			return GetTypes(stackBehaviour).Length;
		}
		
		public static int GetStackChange(this OpCode opCode)
		{
			return GetCount(opCode.StackBehaviourPush) - GetCount(opCode.StackBehaviourPop);
		}
	}
}
