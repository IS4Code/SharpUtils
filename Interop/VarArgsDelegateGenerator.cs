using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace IllidanS4.SharpUtils.Interop
{
	[Obsolete]
	public static class VarArgsDelegateGenerator
	{
		static readonly CustomAttributeBuilder runtimeImpl;
		
		static VarArgsDelegateGenerator()
		{
			runtimeImpl = new CustomAttributeBuilder(TypeOf<MethodImplAttribute>.TypeID.GetConstructor(new[]{TypeOf<MethodImplOptions>.TypeID}), new object[]{MethodImplOptions.InternalCall});
		}
		
		public static Delegate FromMethod(MethodInfo method)
		{
			return FromMethod(method, null);
		}
		
		public static Delegate FromMethod(MethodInfo method, object target)
		{
			TypeBuilder tb = DynamicResources.DynamicModule.DefineType(method.Name+"Delegate", TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AnsiClass | TypeAttributes.AutoClass);
			tb.SetParent(TypeOf<MulticastDelegate>.TypeID);
			ConstructorBuilder cb = tb.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.HasThis, new[]{TypeOf<object>.TypeID, TypeOf<IntPtr>.TypeID});
			cb.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed | MethodImplAttributes.Synchronized | MethodImplAttributes.NoInlining);
			MethodBuilder mb = tb.DefineMethod("Invoke", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, CallingConventions.HasThis | (method.CallingConvention & CallingConventions.VarArgs), method.ReturnType, method.GetParameters().Select(p => p.ParameterType).ToArray());
			mb.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed | MethodImplAttributes.Synchronized | MethodImplAttributes.NoInlining);
			Type delType = tb.CreateType();
			return Delegate.CreateDelegate(delType, target, method);
		}
	}
}
