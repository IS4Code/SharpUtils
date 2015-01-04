/* Date: 14.11.2014, Time: 12:27 */
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using IllidanS4.SharpUtils.Reflection;
using IllidanS4.SharpUtils.Unsafe;

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
		/// Stores a structure in the unmanaged memory.
		/// </summary>
		/// <param name="structure">A reference to the structure to be stored in the memory.</param>
		/// <param name="ptr">The pointer to the memory location.</param>
		/// <param name="size">The size of the structure.</param>
		public unsafe static void StructureToPtrDirect(TypedReference structure, IntPtr ptr, int size)
		{
			StructureToPtrNative(structure, (byte*)ptr, unchecked((uint)size));
		}
		
		/// <summary>
		/// Stores a structure in the unmanaged memory.
		/// </summary>
		/// <param name="structure">A reference to the structure to be stored in the memory.</param>
		/// <param name="ptr">The pointer to the memory location.</param>
		public static void StructureToPtrDirect(TypedReference structure, IntPtr ptr)
		{
			StructureToPtrDirect(structure, ptr, SizeOf(__reftype(structure)));
		}
		
		/// <summary>
		/// Stores a structure in the unmanaged memory.
		/// </summary>
		/// <param name="structure">The sturecture to be stored in the memory.</param>
		/// <param name="ptr">The pointer to the memory location.</param>
		public static void StructureToPtr<T>(ref T structure, IntPtr ptr)
		{
			StructureToPtrDirect(__makeref(structure), ptr);
		}
		
		/// <summary>
		/// Stores a structure in the unmanaged memory.
		/// </summary>
		/// <param name="structure">The sturcture to be stored in the memory.</param>
		/// <param name="ptr">The pointer to the memory location.</param>
		public static void StructureToPtr<T>(T structure, IntPtr ptr)
		{
			StructureToPtrDirect(__makeref(structure), ptr);
		}
		
		/// <summary>
		/// Reads a structure from the unmanaged memory.
		/// </summary>
		/// <param name="ptr">A pointer to the memory where the structure is stored.</param>
		/// <param name="structure">Variable in which the structure will be stored.</param>
		/// <param name="size">The size of the structure.</param>
		public unsafe static void PtrToStructureDirect(IntPtr ptr, TypedReference structure, int size)
		{
			PtrToStructureNative((byte*)ptr, structure, unchecked((uint)size));
		}
		
		/// <summary>
		/// Reads a structure from the unmanaged memory.
		/// </summary>
		/// <param name="ptr">A pointer to the memory where the structure is stored.</param>
		/// <param name="structure">Variable in which the structure will be stored.</param>
		public static void PtrToStructureDirect(IntPtr ptr, TypedReference structure)
		{
			PtrToStructureDirect(ptr, structure, SizeOf(__reftype(structure)));
		}
		
		/// <summary>
		/// Reads a structure from the unmanaged memory.
		/// </summary>
		/// <param name="ptr">A pointer to the memory where the structure is stored.</param>
		/// <param name="structure">Variable in which the structure will be stored.</param>
		public static void PtrToStructure<T>(IntPtr ptr, out T structure)
		{
			structure = default(T);
			PtrToStructureDirect(ptr, __makeref(structure));
		}
		
		/// <summary>
		/// Reads a structure from the unmanaged memory.
		/// </summary>
		/// <param name="ptr">A pointer to the memory where the structure is stored.</param>
		/// <returns>The structure in the memory.</returns>
		public static T PtrToStructure<T>(IntPtr ptr)
		{
			T obj;
			PtrToStructure(ptr, out obj);
			return obj;
		}
		
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
			return SizeOfHelper_f(t, true);
		}
		#endregion
		
		/// <summary>
		/// Compares two references for equality.
		/// </summary>
		/// <param name="a">The first reference.</param>
		/// <param name="b">The second reference.</param>
		/// <returns>true if they are equal (point to the same location).</returns>
		public static bool Equals<T>(ref T a, ref T b)
		{
			return __makeref(a).Equals(__makeref(b));
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
		
		private static Action<IntPtr,IntPtr,uint> cpblk = Create_cpblk();
		private static Action<IntPtr,IntPtr,uint> Create_cpblk()
		{
			DynamicMethod dyn = new DynamicMethod("Cpblk", null, new[]{typeof(IntPtr), typeof(IntPtr), typeof(uint)}, typeof(InteropTools), true);
			var il = dyn.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Cpblk);
			il.Emit(OpCodes.Ret);
			return (Action<IntPtr,IntPtr,uint>)dyn.CreateDelegate(typeof(Action<IntPtr,IntPtr,uint>));
		}
		
		private static Action<IntPtr,byte,uint> initblk = Create_initblk();
		private static Action<IntPtr,byte,uint> Create_initblk()
		{
			DynamicMethod dyn = new DynamicMethod("Initblk", null, new[]{typeof(IntPtr), typeof(byte), typeof(uint)}, typeof(InteropTools), true);
			var il = dyn.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Initblk);
			il.Emit(OpCodes.Ret);
			return (Action<IntPtr,byte,uint>)dyn.CreateDelegate(typeof(Action<IntPtr,byte,uint>));
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
	}
}
