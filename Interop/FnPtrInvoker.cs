/* Date: 11.1.2015, Time: 12:38 */
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection;

namespace IllidanS4.SharpUtils.Interop
{
	/// <summary>
	/// Allows dynamic invoking of function pointers with the specified signature. TDelegate must be a delegate type with IntPtr, pointing to the function, as the first parameter.
	/// </summary>
	public static class FnPtrInvoker<TDelegate> where TDelegate : class
	{
		public static readonly TDelegate Invoke;
		
		static FnPtrInvoker()
		{
			Type tDel = typeof(TDelegate);
			var msig = ReflectionTools.GetDelegateSignature(tDel);
			var ptypes = msig.ParameterTypes;
			if(ptypes[0] != typeof(IntPtr))
				throw new ArgumentException("Delegate must have IntPtr as the first parameter.");
			DynamicMethod dyn = new DynamicMethod("Invoker", msig.ReturnType, ptypes, typeof(FnPtrInvoker<TDelegate>), true);
			var il = dyn.GetILGenerator();
			for(int i = 1; i < ptypes.Length; i++)
			{
				il.EmitLdarg(i);
			}
			il.Emit(OpCodes.Ldarg_0);
			Type[] newptypes = ptypes.Skip(1).ToArray();
			if(msig.IsUnmanaged)
			{
				il.EmitCalli(OpCodes.Calli, msig.UnmanagedCallingConvention, msig.ReturnType, newptypes);
			}else{
				il.EmitCalli(OpCodes.Calli, msig.CallingConvention, msig.ReturnType, newptypes, msig.OptionalParameterTypes);
			}
			il.Emit(OpCodes.Ret);
			
			Invoke = (TDelegate)(object)dyn.CreateDelegate(tDel);
		}
	}
}
