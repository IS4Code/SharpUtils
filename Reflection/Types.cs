/* Date: 29.11.2014, Time: 19:33 */
using System;
using System.Linq.Expressions;
using System.Reflection;
using IllidanS4.SharpUtils.Accessing;
using IllidanS4.SharpUtils.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection
{
	/// <summary>
	/// A collection of some useful types.
	/// </summary>
	public static class Types
	{
		public static readonly Module CommonLanguageRuntimeLibrary = typeof(object).Module;
		public static readonly Module Core = typeof(Expression).Module;
		
		public static readonly Type Signature = CommonLanguageRuntimeLibrary.GetType("System.Signature");
		public static readonly Type RuntimeType = CommonLanguageRuntimeLibrary.GetType("System.RuntimeType");
		public static readonly Type RuntimeModule = CommonLanguageRuntimeLibrary.GetType("System.Reflection.RuntimeModule");
		public static readonly Type MetadataImport = CommonLanguageRuntimeLibrary.GetType("System.Reflection.MetadataImport");
		public static readonly Type ConstArray = CommonLanguageRuntimeLibrary.GetType("System.Reflection.ConstArray");
		public static readonly Type DelegateHelpers = Core.GetType("System.Linq.Expressions.Compiler.DelegateHelpers");
    	public static readonly Type Void = typeof(void);
		
		/// <summary>
		/// Types that are generated from a signature.
		/// </summary>
		public static class Generated
		{
			public static readonly Type FnPtr = ReflectionTools.GetTypeFromFieldSignature(new byte[]{6, 27, 0, 0, 1});
			public static readonly Type FnPtrPointer = ReflectionTools.GetTypeFromFieldSignature(new byte[]{6, 0x0F, 27, 0, 0, 1});
			public static readonly Type FnPtrByRef = ReflectionTools.GetTypeFromFieldSignature(new byte[]{6, 0x10, 27, 0, 0, 1});
			public static readonly Type FnPtrSZArray = ReflectionTools.GetTypeFromFieldSignature(new byte[]{6, 0x1D, 27, 0, 0, 1});
			public static readonly MDFnPtrArrayCreator FnPtrMDArray = MDFnPtrArrayCreator.Instance;
			
			public sealed class MDFnPtrArrayCreator : IIndexGet<int,Type>, IIndexGet<uint,Type>
			{
				internal static readonly MDFnPtrArrayCreator Instance = new MDFnPtrArrayCreator();
				private static readonly byte[] sigstart = new byte[]{6, 20, 27, 0, 0, 1};
				
				public Type this[int rank]
				{
					get{
						return this[unchecked((uint)rank)];
					}
				}
				
				[CLSCompliant(false)]
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
}
