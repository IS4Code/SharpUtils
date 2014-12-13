/* Date: 14.11.2014, Time: 12:27 */
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Reflection;

namespace IllidanS4.SharpUtils.Interop
{
	[CLSCompliant(false)]
	public static class InteropTools
	{
		const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
		
		#region StructureToPtr
		private static readonly Type SafeBufferType = typeof(SafeBuffer);
		public unsafe delegate void PtrToStructureNativeDelegate(byte* ptr, TypedReference structure, uint sizeofT);
		public unsafe delegate void StructureToPtrNativeDelegate(TypedReference structure, byte* ptr, uint sizeofT);
		private static readonly MethodInfo PtrToStructureNativeMethod = SafeBufferType.GetMethod("PtrToStructureNative", flags);
		private static readonly MethodInfo StructureToPtrNativeMethod = SafeBufferType.GetMethod("StructureToPtrNative", flags);
		public static readonly PtrToStructureNativeDelegate PtrToStructureNative = (PtrToStructureNativeDelegate)Delegate.CreateDelegate(typeof(PtrToStructureNativeDelegate), PtrToStructureNativeMethod);
		public static readonly StructureToPtrNativeDelegate StructureToPtrNative = (StructureToPtrNativeDelegate)Delegate.CreateDelegate(typeof(StructureToPtrNativeDelegate), StructureToPtrNativeMethod);
		
		private static readonly Func<Type,bool,int> SizeOfHelper_f = (Func<Type,bool,int>)Delegate.CreateDelegate(typeof(Func<Type,bool,int>), typeof(Marshal).GetMethod("SizeOfHelper", flags));
		
		/// <summary>
		/// Stores a structure in the memory.
		/// </summary>
		/// <param name="structure">A reference to the structure to be stored in the memory.</param>
		/// <param name="ptr">The pointer to the memory location.</param>
		/// <param name="size">The size of the structure.</param>
		public unsafe static void StructureToPtrDirect(TypedReference structure, IntPtr ptr, int size)
		{
			StructureToPtrNative(structure, (byte*)ptr, unchecked((uint)size));
		}
		
		public static void StructureToPtrDirect(TypedReference structure, IntPtr ptr)
		{
			StructureToPtrDirect(structure, ptr, SizeOf(__reftype(structure)));
		}
		
		public static void StructureToPtr<T>(ref T structure, IntPtr ptr)
		{
			StructureToPtrDirect(__makeref(structure), ptr);
		}
		
		public static void StructureToPtr<T>(T structure, IntPtr ptr)
		{
			StructureToPtrDirect(__makeref(structure), ptr);
		}
		
		public unsafe static void PtrToStructureDirect(IntPtr ptr, TypedReference structure, int size)
		{
			PtrToStructureNative((byte*)ptr, structure, unchecked((uint)size));
		}
		
		public static void PtrToStructureDirect(IntPtr ptr, TypedReference structure)
		{
			PtrToStructureDirect(ptr, structure, SizeOf(__reftype(structure)));
		}
		
		public static void PtrToStructure<T>(IntPtr ptr, out T structure)
		{
			structure = default(T);
			PtrToStructureDirect(ptr, __makeref(structure));
		}
		
		public static T PtrToStructure<T>(IntPtr ptr)
		{
			T obj;
			PtrToStructure(ptr, out obj);
			return obj;
		}
		
		public static int SizeOf<T>(T structure)
		{
			return SizeOf<T>();
		}
		
		public static int SizeOf<T>()
		{
			return SizeOf(typeof(T));
		}
		
		public static int SizeOf(Type t)
		{
			return SizeOfHelper_f(t, true);
		}
		#endregion
		
		#region Comparing typedref
		public unsafe static bool Equals(this TypedReference tr, TypedReference other)
		{
			IntPtr* a = ((IntPtr*)&tr);
			IntPtr* b = ((IntPtr*)&other);
			return a[0] == b[0] && a[1] == b[1];
		}
		
		public static bool Equals<T>(ref T a, ref T b)
		{
			return __makeref(a).Equals(__makeref(b));
		}
		#endregion
		
		#region Field typedref
		//private delegate void InternalMakeTypedReferenceDelegate(void* result, object target, IntPtr[] flds, Type lastFieldType);
		//private static readonly InternalMakeTypedReferenceDelegate InternalMakeTypedReference = (InternalMakeTypedReferenceDelegate)Delegate.CreateDelegate(typeof(InternalMakeTypedReferenceDelegate), typeof(TypedReference).GetMethod("InternalMakeTypedReference", BindingFlags.NonPublic | BindingFlags.Static));
		
		private unsafe delegate void InternalMakeTypedReferenceDelegate(void* result, object target, IntPtr[] flds, Type lastFieldType);
		private static readonly Type RuntimeTypeType = typeof(Type).Module.GetType("System.RuntimeType");
		private static readonly InternalMakeTypedReferenceDelegate InternalMakeTypedReference = GetInternalMakeTypedReference();
		private static InternalMakeTypedReferenceDelegate GetInternalMakeTypedReference()
		{
			MethodInfo mi = typeof(TypedReference).GetMethod("InternalMakeTypedReference", flags);
			DynamicMethod method = new DynamicMethod("InternalMakeTypedReference", typeof(void), new[]{typeof(void*), typeof(object), typeof(IntPtr[]), typeof(Type)}, RuntimeTypeType.Module, true);
			var il = method.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Ldarg_3);
			il.Emit(OpCodes.Castclass, RuntimeTypeType);
			il.EmitCall(OpCodes.Call, mi, null);
			il.Emit(OpCodes.Ret);
			return (InternalMakeTypedReferenceDelegate)method.CreateDelegate(typeof(InternalMakeTypedReferenceDelegate));
		}
		
		public unsafe static void MakeTypedReference([Out]TypedReference* result, object target, params FieldInfo[] fields)
		{
			if(target == null)
			{
				MakeStaticTypedReference(result, fields);
				return;
			}
			IntPtr[] flds = new IntPtr[fields.Length];
			Type lastType = target.GetType();
			for(int i = 0; i < fields.Length; i++)
			{
				var field = fields[i];
				if(field.IsStatic)
				{
					throw new ArgumentException("Field cannot be static.", "fields");
				}
				flds[i] = field.FieldHandle.Value;
				lastType = field.FieldType;
			}
			InternalMakeTypedReference(result, target, flds, lastType);
		}
		
		private unsafe delegate void FieldTypedRef(void* result);
		private unsafe static void MakeStaticTypedReference([Out]TypedReference* result, params FieldInfo[] fields)
		{
			DynamicMethod mb = new DynamicMethod("GetStaticTypedReference", typeof(void), new[]{typeof(void*)}, typeof(InteropTools).Module, true);
			var il = mb.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			Type ftype = null;
			foreach(var field in fields)
			{
				OpCode opcode;
				if(ftype == null)
				{
					opcode = OpCodes.Ldsflda;
					if(!field.IsStatic)
					{
						throw new ArgumentException("First field must be static.", "fields");
					}
				}else{
					opcode = OpCodes.Ldflda;
					if(field.IsStatic)
					{
						throw new ArgumentException("Next field cannot be static.", "fields");
					}
				}
				ftype = field.FieldType;
				il.Emit(opcode, field);
			}
			il.Emit(OpCodes.Mkrefany, ftype);
			il.Emit(OpCodes.Stobj, typeof(TypedReference));
			il.Emit(OpCodes.Ret);
			FieldTypedRef del = (FieldTypedRef)mb.CreateDelegate(typeof(FieldTypedRef));
			del.Invoke(result);
		}
		#endregion
		
		#region Object typedref
		
		/*private static class TypedRefBox<T>
		{
			public delegate 
		}
		
		private static readonly MethodInfo MakeObjectTypedReferenceMethod = typeof(InteropTools).GetMethod("MakeObjectTypedReference", BindingFlags.Public | BindingFlags.Static);
		
		public static void MakeObjectTypedReference([Out]TypedReference* tr, ValueType boxed)
		{
			if(boxed == null) throw new ArgumentNullException("boxed");
			Type t = boxed.GetType();
			MakeObjectTypedReferenceMethod.MakeGenericMethod(t).Invoke(null,new[]{boxed});
		}
		
		public static void MakeObjectTypedReference<T>([Out]TypedReference* tr, ValueType boxed)
		{
			if(boxed == null) throw new ArgumentNullException("boxed");
		}*/
		
		#endregion
		
		public static IntPtr GetFunctionPointer(MethodInfo method)
		{
			DynamicMethod dyn = new DynamicMethod("Ldftn", typeof(IntPtr), Type.EmptyTypes, method.Module);
			var il = dyn.GetILGenerator();
			il.Emit(OpCodes.Ldftn, method);
			il.Emit(OpCodes.Ret);
			return (IntPtr)dyn.Invoke(null,null);
		}
		public static IntPtr GetVirtualFunctionPointer(MethodInfo method)
		{
			DynamicMethod dyn = new DynamicMethod("Ldvirtftn", typeof(IntPtr), Type.EmptyTypes, method.Module);
			var il = dyn.GetILGenerator();
			il.Emit(OpCodes.Ldvirtftn, method);
			il.Emit(OpCodes.Ret);
			return (IntPtr)dyn.Invoke(null,null);
		}
	}
}
