/* Date: 3.4.2015, Time: 17:29 */
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Reflection.Linq;

namespace IllidanS4.SharpUtils.Interop
{
	using ACpblk = Action<IntPtr,IntPtr,uint>;
	using AInblk = Action<IntPtr,byte,uint>;
	
	partial class InteropTools
	{
		class SafeBufferHacks
		{
			private static readonly Type SafeBufferType = typeof(SafeBuffer);
			public unsafe delegate void PtrToStruct(byte* ptr, TypedReference structure, uint sizeofT);
			public unsafe delegate void StructToPtr(TypedReference structure, byte* ptr, uint sizeofT);
			public static readonly PtrToStruct PtrToStructureNative = Hacks.GetInvoker<PtrToStruct>(SafeBufferType, "PtrToStructureNative", false);
			public static readonly StructToPtr StructureToPtrNative = Hacks.GetInvoker<StructToPtr>(SafeBufferType, "PtrToStructureNative", false);
		}
		
		class SizeOfHacks
		{
			public static readonly Func<Type,bool,int> SizeOfHelper = Hacks.GetInvoker<Func<Type,bool,int>>(typeof(Marshal), "SizeOfHelper", false);
		}
		
		private static ACpblk cpblk = LinqEmit.CreateDynamicMethod<ACpblk>(
			"Cpblk",
			OpCodes.Ldarg_0,
			OpCodes.Ldarg_1,
			OpCodes.Ldarg_2,
			OpCodes.Cpblk,
			OpCodes.Ret
		);
		
		private static AInblk initblk = LinqEmit.CreateDynamicMethod<AInblk>(
			"Initblk",
			OpCodes.Ldarg_0,
			OpCodes.Ldarg_1,
			OpCodes.Ldarg_2,
			OpCodes.Initblk,
			OpCodes.Ret
		);
	}
}
