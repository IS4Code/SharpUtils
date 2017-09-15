/* Date: 20.4.2017, Time: 16:50 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils;
using IllidanS4.SharpUtils.Reflection;

namespace IllidanS4.SharpUtils.Interop
{
	public abstract partial class NativeLibrary : IDisposable
	{
		IntPtr hmod;
		
		public NativeLibrary(IntPtr module)
		{
			hmod = module;
		}
		
		protected TDel FindFunction<TDel>(RuntimeMethodHandle method) where TDel : class
		{
			if(hmod == IntPtr.Zero) return null;
			var mi = (MethodInfo)MethodBase.GetMethodFromHandle(method);
			IntPtr proc = FindEntryPoint(mi);
			if(proc == IntPtr.Zero) throw new Win32Exception();
			
			return Marshal.GetDelegateForFunctionPointer<TDel>(proc);
		}
		
		private IntPtr FindEntryPoint(MethodInfo mi)
		{
			var import = mi.GetCustomAttribute(typeof(LibImportAttribute)) as LibImportAttribute;
			
			string entryPoint = import.EntryPoint;
			if(entryPoint == null) entryPoint = mi.Name;
			
			CharSet charSet = import.CharSet;
			if(charSet == CharSet.Auto) charSet = CharSet.Unicode;
			
			bool exact = import.ExactSpelling;
			if(import.EntryPoint != null) exact = true;
			
			IntPtr proc = Native.GetProcAddress(hmod, entryPoint);
			if(exact)
			{
				return proc;
			}
			if(proc == IntPtr.Zero || charSet != CharSet.Ansi)
			{
				if(charSet == CharSet.Ansi)
				{
					proc = Native.GetProcAddress(hmod, entryPoint+"A");
				}else{
					var procUni = Native.GetProcAddress(hmod, entryPoint+"W");
					if(procUni != IntPtr.Zero)
					{
						proc = procUni;
					}
				}
				
				if(proc != IntPtr.Zero)
				{
					return proc;
				}
			}
			
			if(proc == IntPtr.Zero)
			{
				int bytes = 0;
				foreach(var param in mi.GetParameters())
				{
					var attr = param.GetCustomAttribute<MarshalAsAttribute>();
					int nativeSize = 0;
					if(attr != null)
					{
						nativeSize = InteropTools.SizeOf(attr.Value);
						if(nativeSize == 0 && attr.SizeConst > 0)
						{
							nativeSize = InteropTools.SizeOf(attr.ArraySubType)*attr.SizeConst;
						}
					}
					if(nativeSize == 0)
					{
						nativeSize = Marshal.SizeOf(param.ParameterType);
					}
					bytes += nativeSize;
				}
				proc = Native.GetProcAddress(hmod, entryPoint);
				
				if(proc == IntPtr.Zero)
				{
					proc = Native.GetProcAddress(hmod, "_"+entryPoint+"@"+bytes);
					
					if(proc == IntPtr.Zero)
					{
						proc = Native.GetProcAddress(hmod, "_"+entryPoint);
					}
				}
			}
			return proc;
		}
		
		/*protected TDel FindFunction<TDel>(string name, bool exact) where TDel : class
		{
			if(hmod == IntPtr.Zero) return null;
			IntPtr proc = Native.GetProcAddress(hmod, name);
			if(proc == IntPtr.Zero)
			{
				if(exact) throw new Win32Exception();
				proc = FindProcMangled(name);
			}
			
			return Marshal.GetDelegateForFunctionPointer<TDel>(proc);
		}*/
		
		public static T Create<T>(string lib) where T : NativeLibrary
		{
			return NativeFactory<T>.Create(lib);
		}
		
		public void Dispose()
		{
			Dispose(true);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			GC.SuppressFinalize(this);
			if(hmod != IntPtr.Zero)
			{
				Native.FreeLibrary(hmod);
				hmod = IntPtr.Zero;
			}
		}
		
		~NativeLibrary()
		{
			Dispose(false);
		}
		
		static readonly MethodInfo FindFunctionMethod =
			typeof(NativeLibrary)
			.GetMethod("FindFunction", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		static class NativeFactory<T> where T : NativeLibrary
		{
			static readonly Type libraryType;
			
			static NativeFactory()
			{
				Type iType = typeof(T);
				
				TypeBuilder tb = DynamicResources.DefineDynamicType(TypeAttributes.Public | TypeAttributes.Sealed);
				tb.SetParent(iType);
				
				
				var ctorArgs = new[]{typeof(IntPtr)};
				
				var baseCtor = iType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, ctorArgs, null);
				
				var ctor = tb.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.HasThis, ctorArgs);
				var cil = ctor.GetILGenerator();
				cil.Emit(OpCodes.Ldarg_0);
				cil.Emit(OpCodes.Ldarg_1);
				cil.Emit(OpCodes.Call, baseCtor);
				
				var procArray = tb.DefineField("m_procs", typeof(MulticastDelegate[]), FieldAttributes.Private | FieldAttributes.InitOnly);
				var methods = iType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				
				cil.Emit(OpCodes.Ldarg_0);
				cil.Emit(OpCodes.Ldc_I4, methods.Length);
				cil.Emit(OpCodes.Newarr, typeof(MulticastDelegate));
				cil.Emit(OpCodes.Stfld, procArray);
				
				int index = 0;
				foreach(var mi in methods)
				{
					if(!mi.IsAbstract) continue;
					var import = mi.GetCustomAttribute(typeof(LibImportAttribute)) as LibImportAttribute;
					if(import == null) continue;
					
					
					var delType = DynamicResources.DefineDynamicType(TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AnsiClass | TypeAttributes.AutoClass);
					delType.SetParent(typeof(MulticastDelegate));
					var cb = delType.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, new[]{typeof(object), typeof(IntPtr)});
					cb.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);
					var args = mi.GetParameters();
					var argTypes = args.Select(p => p.ParameterType).ToArray();
					MethodBuilder delInvoke = delType.DefineMethod("Invoke", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, CallingConventions.HasThis | mi.CallingConvention, mi.ReturnType, null);
					delInvoke.SetParameters(argTypes);
					delInvoke.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);
					for(int i = 0; i < args.Length; i++)
					{
						var arg = args[i];
						
						var marshal = arg.GetCustomAttribute<MarshalAsAttribute>();
						var guid = arg.GetCustomAttribute<GuidAttribute>();
						
						var attributes =
							(arg.IsIn ? ParameterAttributes.In : 0) |
							(arg.IsLcid ? ParameterAttributes.Lcid : 0) |
							(arg.IsOptional ? ParameterAttributes.Optional : 0) |
							(arg.IsOut ? ParameterAttributes.Out : 0) |
							(arg.IsRetval ? ParameterAttributes.Retval : 0) |
							(arg.HasDefaultValue ? ParameterAttributes.HasDefault : 0) |
							(marshal != null ? ParameterAttributes.HasFieldMarshal : 0);
						var builder = delInvoke.DefineParameter(i, attributes, arg.Name);
						
						if(marshal != null)
						{
							Guid iid = guid != null ? Guid.Parse(guid.Value) : Guid.Empty;
							
							new MarshalInfo(marshal, iid).Apply(builder);
						}
					}
					import.Apply(delType);
					
					delType.CreateType();
					/*Type del = delType.CreateType();
					var delInvoke = del.getme*/
					
					string entryPoint = import.EntryPoint;
					if(entryPoint == null) entryPoint = mi.Name;
					
					var mb = tb.DefineMethod(mi.Name, mi.Attributes & (~MethodAttributes.Abstract), mi.ReturnType, argTypes);
					tb.DefineMethodOverride(mb, mi);
					var il = mb.GetILGenerator();
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldfld, procArray);
					if(index <= 255)
					{
						il.Emit(OpCodes.Ldc_I4_S, (byte)index);
					}else{
						il.Emit(OpCodes.Ldc_I4, index);
					}
					il.Emit(OpCodes.Ldelem, typeof(MulticastDelegate));
					il.Emit(OpCodes.Castclass, delType);
					
					for(int i = 0; i < args.Length; i++)
					{
						il.EmitLdarg(i+1);
					}
					il.Emit(OpCodes.Tailcall);
					il.Emit(OpCodes.Callvirt, delInvoke);
					il.Emit(OpCodes.Ret);
					
					cil.Emit(OpCodes.Ldarg_0);
					cil.Emit(OpCodes.Ldfld, procArray);
					if(index <= 255)
					{
						cil.Emit(OpCodes.Ldc_I4_S, (byte)index);
					}else{
						cil.Emit(OpCodes.Ldc_I4, index);
					}
					
					cil.Emit(OpCodes.Ldarg_0);
					cil.Emit(OpCodes.Ldstr, entryPoint);
					cil.Emit(import.ExactSpelling ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
					cil.Emit(OpCodes.Callvirt, FindFunctionMethod.MakeGenericMethod(delType));
					
					cil.Emit(OpCodes.Stelem, delType);
					
					index++;
				}
				
				cil.Emit(OpCodes.Ret);
				
				libraryType = tb.CreateType();
			}
			
			public static T Create(string lib)
			{
				IntPtr mod = Native.LoadLibrary(lib);
				return (T)Activator.CreateInstance(libraryType, mod);
			}
		}
		
		private class Native
		{
			public const string Lib = "kernel32.dll";
			
			[DllImport(Lib, SetLastError=true, CharSet=CharSet.Ansi)]
			public static extern IntPtr LoadLibrary(string lpFileName);
			
			[DllImport(Lib, SetLastError=true, CharSet=CharSet.Ansi)]
			public static extern bool FreeLibrary(IntPtr hModule);
			
			[DllImport(Lib, SetLastError=true, CharSet=CharSet.Ansi)]
			public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
			
			[DllImport(Lib, SetLastError=true, CharSet=CharSet.Ansi)]
			public static extern IntPtr GetProcAddress(IntPtr hModule, IntPtr procName);
		}
	}
}
