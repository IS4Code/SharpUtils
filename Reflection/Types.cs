/* Date: 29.11.2014, Time: 19:33 */
using System;
using System.Reflection;
using IllidanS4.SharpUtils.Accessing;
using IllidanS4.SharpUtils.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection
{
	public static class Types
	{
		public static readonly Module CommonLanguageRuntimeLibrary = typeof(object).Module;
		
		public static readonly Type RuntimeType = CommonLanguageRuntimeLibrary.GetType("System.RuntimeType");
		public static readonly Type RuntimeModule = CommonLanguageRuntimeLibrary.GetType("System.Reflection.RuntimeModule");
		
		public static readonly Type FnPtr = ReflectionTools.GetTypeFromFieldSignature(new byte[]{6, 27, 0, 0, 1});
		public static readonly Type FnPtrPointer = ReflectionTools.GetTypeFromFieldSignature(new byte[]{6, 0x0F, 27, 0, 0, 1});
		public static readonly Type FnPtrByRef = ReflectionTools.GetTypeFromFieldSignature(new byte[]{6, 0x10, 27, 0, 0, 1});
		public static readonly Type FnPtrSZArray = ReflectionTools.GetTypeFromFieldSignature(new byte[]{6, 0x1D, 27, 0, 0, 1});
		public static readonly IIndexableGetter<int,Type> FnPtrMDArray = MDFnPtrArray.Instance;
		
		private sealed class MDFnPtrArray : IIndexableGetter<int,Type>, IIndexableGetter<uint,Type>
		{
			internal static readonly MDFnPtrArray Instance = new MDFnPtrArray();
			private static readonly byte[] sigstart = new byte[]{6, 20, 27, 0, 0, 1};
			
			public Type this[int rank]
			{
				get{
					return this[unchecked((uint)rank)];
				}
			}
			
			public Type this[uint rank]
			{
				get{
					byte[] brank = SignatureTools.EncodeInteger(rank);
					byte[] sig = new byte[8+brank.Length];
					sigstart.CopyTo(sig, 0);
					brank.CopyTo(sig, 6);
					return ReflectionTools.GetTypeFromFieldSignature(sig);
				}
			}
		}
	}
}
