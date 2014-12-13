/* Date: 14.11.2014, Time: 15:11 */
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IllidanS4.SharpUtils.Interop
{
	[CLSCompliant(false)]
	public static class ReferenceHelper
	{
		public delegate TResult OutDelegate<TArg,TResult>(out TArg variable);
		public delegate TResult RefDelegate<TArg,TResult>(ref TArg variable);
		public delegate void OutDelegate<TArg>(out TArg variable);
		public delegate void RefDelegate<TArg>(ref TArg variable);
		
		static readonly AssemblyBuilder ab;
		static readonly ModuleBuilder mob;
		static readonly Type TypedReferenceType = typeof(TypedReference);
		
		static ReferenceHelper()
		{
			ab = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("ReferenceHelperAssembly"), AssemblyBuilderAccess.Run);
			mob = ab.DefineDynamicModule("ReferenceHelperAssembly.dll");
		}
		
		public static TResult PassReference<TArg,TResult>(TypedReference tref, RefDelegate<TArg,TResult> del)
		{
			return ReferenceBuilder<TArg,TResult>.PassRef(tref, del);
		}
		
		public static void PassReference<TArg>(TypedReference tref, RefDelegate<TArg> del)
		{
			ReferenceBuilder<TArg>.PassRef(tref, del);
		}
		
		public static TResult PassReference<TArg,TResult>(TypedReference tref, OutDelegate<TArg,TResult> del)
		{
			return PassReference<TArg,TResult>(tref, delegate(ref TArg arg){return del(out arg);});
		}
		
		public static void PassReference<TArg>(TypedReference tref, OutDelegate<TArg> del)
		{
			PassReference<TArg>(tref, delegate(ref TArg arg){del(out arg);});
		}
		
		static int mcounter = 0;
		private static Type BuildPassRef(Type deltype, Type argType, Type resultType)
		{
			TypeBuilder tb = mob.DefineType("ReferenceHelperType"+(mcounter++), TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);
			MethodBuilder mb = tb.DefineMethod(
				"PassRef",
				MethodAttributes.Public | MethodAttributes.Static,
				resultType,
				new[]{TypedReferenceType, deltype}
			);
			var il = mb.GetILGenerator();
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Refanyval, argType);
			il.Emit(OpCodes.Callvirt, deltype.GetMethod("Invoke"));
			il.Emit(OpCodes.Ret);
			return tb.CreateType();
		}
		
		private static class ReferenceBuilder<TArg,TResult>
		{
			public delegate TResult PassRefDelegate(TypedReference tref, RefDelegate<TArg,TResult> del);
			public static readonly PassRefDelegate PassRef;
			
			static ReferenceBuilder()
			{
				Type t = BuildPassRef(typeof(RefDelegate<TArg,TResult>), typeof(TArg), typeof(TResult));
				PassRef = (PassRefDelegate)t.GetMethod("PassRef").CreateDelegate(typeof(PassRefDelegate));
			}
		}
		
		private static class ReferenceBuilder<TArg>
		{
			public delegate void PassRefDelegate(TypedReference tref, RefDelegate<TArg> del);
			public static readonly PassRefDelegate PassRef;
			
			static ReferenceBuilder()
			{
				Type t = BuildPassRef(typeof(RefDelegate<TArg>), typeof(TArg), typeof(void));
				PassRef = (PassRefDelegate)t.GetMethod("PassRef").CreateDelegate(typeof(PassRefDelegate));
			}
		}
	}
}
