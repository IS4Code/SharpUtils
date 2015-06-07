/* Date: 24.4.2015, Time: 0:41 */
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using IllidanS4.SharpUtils.Reflection;
using IllidanS4.SharpUtils.Reflection.Emit;

namespace IllidanS4.SharpUtils.Interop
{
	[CLSCompliant(false)]
	public abstract class VarArgsDelegate : VarAgsDelegateBase<MulticastDelegate>
	{
		public abstract object Invoke(__arglist);
	}
	
	public abstract class VarAgsDelegateBase<TDelegateBase> where TDelegateBase : class, ICloneable, ISerializable
	{
		private static readonly MethodInfo InvokeMethod = typeof(VarArgsDelegate).GetMethod("Invoke");
		
		internal VarAgsDelegateBase()
		{
			
		}
		
		public TDelegate CreateInvoker<TDelegate>() where TDelegate : TDelegateBase
		{
			return (TDelegate)(object)this.CreateInvoker(TypeOf<TDelegate>.TypeID);
		}
		
		public Delegate CreateInvoker(Type delegateType)
		{
			MethodSignature sig = MethodSignature.FromDelegateType(delegateType);
			var pTypes = new Type[sig.ParameterTypes.Length+1];
			pTypes[0] = TypeOf<VarArgsDelegate>.TypeID;
			sig.ParameterTypes.CopyTo(pTypes, 1);
			DynamicMethod dyn = new DynamicMethod("", sig.ReturnType, pTypes, TypeOf<VarAgsDelegateBase<TDelegateBase>>.TypeID);
			var il = dyn.GetILGenerator();
			for(int i = 0; i < pTypes.Length; i++)
			{
				il.EmitLdarg(i);
			}
			il.EmitCall(OpCodes.Callvirt, InvokeMethod, sig.ParameterTypes);
			if(sig.ReturnType != typeof(void))
			{
				if(sig.ReturnType.IsValueType)
				{
					il.Emit(OpCodes.Unbox_Any, sig.ReturnType);
				}else if(sig.ReturnType != TypeOf<object>.TypeID)
				{
					il.Emit(OpCodes.Castclass, sig.ReturnType);
				}
			}else{
				il.Emit(OpCodes.Pop);
			}
			il.Emit(OpCodes.Ret);
			return dyn.CreateDelegate(delegateType, this);
		}
	}
}
