/* Date: 14.11.2014, Time: 12:27 */
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using IllidanS4.SharpUtils.Reflection;
using IllidanS4.SharpUtils.Reflection.Linq;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils.Interop
{
	public static partial class InteropTools
	{
		#region StructureToPtr
		/// <summary>
		/// Stores a structure in the unmanaged memory.
		/// </summary>
		/// <param name="structure">A reference to the structure to be stored in the memory.</param>
		/// <param name="ptr">The pointer to the memory location.</param>
		/// <param name="size">The size of the structure.</param>
		[CLSCompliant(false)]
		public unsafe static void StructureToPtrDirect(TypedReference structure, IntPtr ptr, int size)
		{
			SafeBufferHacks.StructureToPtrNative(structure, (byte*)ptr, unchecked((uint)size));
		}
		
		/// <summary>
		/// Stores a structure in the unmanaged memory.
		/// </summary>
		/// <param name="structure">A reference to the structure to be stored in the memory.</param>
		/// <param name="ptr">The pointer to the memory location.</param>
		[CLSCompliant(false)]
		public static void StructureToPtrDirect(TypedReference structure, IntPtr ptr)
		{
			StructureToPtrDirect(structure, ptr, SizeOf(__reftype(structure)));
		}
		
		/// <summary>
		/// Stores a structure in the unmanaged memory.
		/// </summary>
		/// <param name="structure">The sturecture to be stored in the memory.</param>
		/// <param name="ptr">The pointer to the memory location.</param>
		public static void StructureToPtr<T>(ref T structure, IntPtr ptr) where T : struct
		{
			StructureToPtrDirect(__makeref(structure), ptr);
		}
		
		/// <summary>
		/// Stores a structure in the unmanaged memory.
		/// </summary>
		/// <param name="structure">The structure to be stored in the memory.</param>
		/// <param name="ptr">The pointer to the memory location.</param>
		public static void StructureToPtr<T>(T structure, IntPtr ptr) where T : struct
		{
			StructureToPtrDirect(__makeref(structure), ptr);
		}
		
		/// <summary>
		/// Reads a structure from the unmanaged memory.
		/// </summary>
		/// <param name="ptr">A pointer to the memory where the structure is stored.</param>
		/// <param name="structure">Variable in which the structure will be stored.</param>
		/// <param name="size">The size of the structure.</param>
		[CLSCompliant(false)]
		public unsafe static void PtrToStructureDirect(IntPtr ptr, TypedReference structure, int size)
		{
			SafeBufferHacks.PtrToStructureNative((byte*)ptr, structure, unchecked((uint)size));
		}
		
		/// <summary>
		/// Reads a structure from the unmanaged memory.
		/// </summary>
		/// <param name="ptr">A pointer to the memory where the structure is stored.</param>
		/// <param name="structure">Variable in which the structure will be stored.</param>
		[CLSCompliant(false)]
		public static void PtrToStructureDirect(IntPtr ptr, TypedReference structure)
		{
			PtrToStructureDirect(ptr, structure, SizeOf(__reftype(structure)));
		}
		
		/// <summary>
		/// Reads a structure from the unmanaged memory.
		/// </summary>
		/// <param name="ptr">A pointer to the memory where the structure is stored.</param>
		/// <param name="structure">Variable in which the structure will be stored.</param>
		public static void PtrToStructure<T>(IntPtr ptr, out T structure) where T : struct
		{
			structure = default(T);
			PtrToStructureDirect(ptr, __makeref(structure));
		}
		
		/// <summary>
		/// Reads a structure from the unmanaged memory.
		/// </summary>
		/// <param name="ptr">A pointer to the memory where the structure is stored.</param>
		/// <returns>The structure in the memory.</returns>
		public static T PtrToStructure<T>(IntPtr ptr) where T : struct
		{
			T obj;
			PtrToStructure(ptr, out obj);
			return obj;
		}
		#endregion
		
		/// <summary>
		/// Returns the size of a structure.
		/// </summary>
		/// <param name="structure">The structure.</param>
		/// <returns>Its size.</returns>
		public static int SizeOf<T>(T structure)
		{
			return SizeOf<T>();
		}
		
		/// <summary>
		/// Returns the size of a type.
		/// </summary>
		/// <returns>The size.</returns>
		public static int SizeOf<T>()
		{
			return SizeOf(typeof(T));
		}
		
		/// <summary>
		/// Returns the size of a type.
		/// </summary>
		/// <param name="t">The type.</param>
		/// <returns>The size.</returns>
		public static int SizeOf(Type t)
		{
			return SizeOfHacks.SizeOfHelper(t, true);
		}
		
		public static int SizeOf(UnmanagedType t)
		{
			switch(t)
			{
				case UnmanagedType.Bool:
				case UnmanagedType.I4:
				case UnmanagedType.R4:
				case UnmanagedType.U4:
				case UnmanagedType.Error:
					return 4;
				case UnmanagedType.SysInt:
				case UnmanagedType.SysUInt:
				case UnmanagedType.FunctionPtr:
				case UnmanagedType.IDispatch:
				case UnmanagedType.IInspectable:
				case UnmanagedType.IUnknown:
				case UnmanagedType.LPArray:
				case UnmanagedType.LPStr:
				case UnmanagedType.LPStruct:
				case UnmanagedType.LPTStr:
				case UnmanagedType.LPWStr:
				case UnmanagedType.Interface:
				case UnmanagedType.AnsiBStr:
				case UnmanagedType.BStr:
				case UnmanagedType.SafeArray:
				case UnmanagedType.TBStr:
				case UnmanagedType.VBByRefStr:
					return IntPtr.Size;
				case UnmanagedType.I8:
				case UnmanagedType.R8:
				case UnmanagedType.U8:
				case UnmanagedType.Currency:
					return 8;
				case UnmanagedType.I1:
				case UnmanagedType.U1:
					return 1;
				case UnmanagedType.I2:
				case UnmanagedType.U2:
				case UnmanagedType.VariantBool:
					return 2;
				case UnmanagedType.Struct:
					return 16;
				case UnmanagedType.AsAny:
				case UnmanagedType.ByValArray:
				case UnmanagedType.ByValTStr:
				case UnmanagedType.CustomMarshaler:
				case UnmanagedType.HString:
				default:
					return 0;
			}
		}
				
		[Obsolete("Use System.RuntimeMethodHandle.GetFunctionPointer.")]
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
		
		[CLSCompliant(false)]
		public static void CopyBlock(IntPtr destination, IntPtr source, uint size)
		{
			cpblk(destination, source, size);
		}
		
		public static void CopyBlock(IntPtr destination, IntPtr source, int size)
		{
			cpblk(destination, source, unchecked((uint)size));
		}
		
		[CLSCompliant(false)]
		public static unsafe void CopyBlock(void* destination, void* source, uint size)
		{
			cpblk((IntPtr)destination, (IntPtr)source, size);
		}
		
		[CLSCompliant(false)]
		public static void InitBlock(IntPtr destination, byte initValue, uint size)
		{
			initblk(destination, initValue, size);
		}
		
		public static void InitBlock(IntPtr destination, byte initValue, int size)
		{
			initblk(destination, initValue, unchecked((uint)size));
		}
		
		[CLSCompliant(false)]
		public static unsafe void InitBlock(void* destination, byte initValue, uint size)
		{
			initblk((IntPtr)destination, initValue, size);
		}
		
		/// <summary>
		/// Pins an object on the heap, so its address will not be changed by GC.
		/// </summary>
		/// <param name="o">The object to pin.</param>
		/// <param name="act">The action during which the object will stay pinned.</param>
		public static void Pin(object o, Action<object> act)
		{
			PinHelper.Pin(o, act);
		}
		
		/// <summary>
		/// Pins an object on the heap, so its address will not be changed by GC.
		/// </summary>
		/// <param name="o">The object to pin.</param>
		/// <param name="func">The function during which the object will stay pinned.</param>
		/// <returns>The value returned from <paramref name="func"/>.</returns>
		public static TRet Pin<TRet>(object o, Func<object, TRet> func)
		{
			return PinHelper.WithRet<TRet>.Pin(o, func);
		}
		
		private static class PinHelper
		{
			private static readonly MethodInfo Invoke = typeof(Action<object>).GetMethod("Invoke");
			
			public delegate void Del(object o, Action<object> act);
			public static readonly Del Pin = LinqEmit.CreateDynamicMethod<Del>(
				Instruction.DeclareLocal(TypeOf<object>.TypeID, true),
				OpCodes.Ldarg_0,
				OpCodes.Stloc_0,
				OpCodes.Ldarg_1,
				OpCodes.Ldloc_0,
				new Instruction(OpCodes.Callvirt, Invoke),
				OpCodes.Ret
			);
			
			public static class WithRet<TRet>
			{
				private static readonly MethodInfo Invoke = typeof(Func<object,TRet>).GetMethod("Invoke");
				
				public delegate TRet Del(object o, Func<object,TRet> func);
				public static readonly Del Pin = LinqEmit.CreateDynamicMethod<Del>(
					Instruction.DeclareLocal(TypeOf<object>.TypeID, true),
					OpCodes.Ldarg_0,
					OpCodes.Stloc_0,
					OpCodes.Ldarg_1,
					OpCodes.Ldloc_0,
					new Instruction(OpCodes.Callvirt, Invoke),
					OpCodes.Ret
				);
			}
		}
		
		public static bool ContainsReferences<T>()
		{
			return ContainsReferencesCache<T>.Value;
		}
		
		public static bool ContainsReferences(Type type)
		{
			if(type.IsArray)
			{
				var elem = type.GetElementType();
				return !elem.IsValueType || ContainsReferences(elem);
			}
			if(type.IsPrimitive) return false;
			foreach(var fi in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
			{
				var ftype = fi.FieldType;
				if(!ftype.IsValueType || ContainsReferences(ftype))
				{
					return true;
				}
			}
			return false;
		}
		
		private static class ContainsReferencesCache<T>
		{
			public static readonly bool Value = ContainsReferences(typeof(T));
		}
		
		public static bool IsBlittable<T>()
		{
			return IsBlittableCache<T>.Value;
		}
		
		public static bool IsBlittable(Type type)
		{
			if(type.IsArray)
			{
				var elem = type.GetElementType();
				return !elem.IsValueType && IsBlittable(elem);
			}
	        try{
				object instance = FormatterServices.GetUninitializedObject(type);
				GCHandle.Alloc(instance, GCHandleType.Pinned).Free();
				return true;
	        }catch{
				return false;
			}
		}
		
		private static class IsBlittableCache<T>
		{
			public static readonly bool Value = IsBlittable(typeof(T));
		}
	}
}
