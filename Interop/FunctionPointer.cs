/* Date: 10.4.2015, Time: 23:37 */
using System;
using System.Reflection;
using IllidanS4.SharpUtils.Reflection;
using IllidanS4.SharpUtils.Reflection.Emit;

namespace IllidanS4.SharpUtils.Interop
{
	public struct FunctionPointer : IPointer
	{
		readonly IntPtr ptr;
		readonly FunctionPointerType fnptrType;
		public Type Type{get{return fnptrType;}}
		
		public FunctionPointer(MethodBase method, params Type[] optionalParameterTypes) : this(method.MethodHandle.GetFunctionPointer(), new FunctionPointerType(MethodSignature.FromMethod(method, optionalParameterTypes)))
		{
			
		}
		
		public FunctionPointer(IntPtr ptr, FunctionPointerType fnptrType)
		{
			this.ptr = ptr;
			this.fnptrType = fnptrType;
		}
		
		[CLSCompliant(false)]
		public unsafe FunctionPointer(void* ptr, FunctionPointerType fnptrType) : this((IntPtr)ptr, fnptrType)
		{
			
		}
		
		public IntPtr ToIntPtr()
		{
			return ptr;
		}
		
		[CLSCompliant(false)]
		public unsafe void* ToPointer()
		{
			return (void*)ptr;
		}
		
		public bool IsNull{
			get{
				return ptr == IntPtr.Zero;
			}
		}
		
		/*public object Invoke(params object[] args)
		{
			Type retType = fnptrType.Signature.ReturnType;
			Type[] paramTypes = fnptrType.Signature.ParameterTypes;
			
			Delegate del = (Delegate)typeof(FnPtrInvoker<>).MakeGenericType(ReflectionTools.GetDelegateType(retType, paramTypes)).InvokeMember("Invoke", BindingFlags.GetField, null, null, null);
			object[] newargs = new object[args.Length+1];
			newargs[0] = ptr;
			args.CopyTo(newargs, 1);
			return del.DynamicInvoke(newargs);
		}*/
	}
}
