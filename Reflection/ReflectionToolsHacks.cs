/* Date: 3.4.2015, Time: 17:09 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;

namespace IllidanS4.SharpUtils.Reflection
{
	using FCsbIDict = Func<CallSiteBinder,IDictionary<Type,object>>;
	using FTypeArgs = Func<InvokeMemberBinder,IEnumerable<Type>>;
	using FTArrT = Func<Type[],Type>;
	using FOT = Func<object,Type>;
	using FCaFlags = Func<CSharpArgumentInfo,CSharpArgumentInfoFlags>;
	using FCaName = Func<CSharpArgumentInfo,string>;
	
	partial class ReflectionTools
	{
		static readonly Assembly CSharpAssembly = typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly;
		static readonly FCsbIDict GetCache = Hacks.GetFieldGetter<FCsbIDict>(typeof(CallSiteBinder), "Cache");
		static readonly FTypeArgs GetTypeArgs = Hacks.GetPropertyGetter<FTypeArgs>(CSharpAssembly.GetType("Microsoft.CSharp.RuntimeBinder.ICSharpInvokeOrInvokeMemberBinder"), "TypeArguments");
		static readonly FCaFlags GetArgFlags = Hacks.GetPropertyGetter<FCaFlags>(typeof(CSharpArgumentInfo), "Flags");
		static readonly FCaName GetArgName = Hacks.GetPropertyGetter<FCaName>(typeof(CSharpArgumentInfo), "Name");
		static readonly FTArrT MakeNewCustomDelegate = Hacks.GetInvoker<FTArrT>(Types.DelegateHelpers, "MakeNewCustomDelegate", false);
		static readonly FTArrT MakeNewDelegate = Hacks.GetInvoker<FTArrT>(Types.DelegateHelpers, "MakeNewDelegate", false);
		
		unsafe delegate object NewSignature(void* sigptr, int siglength, Type declaringType);
		static readonly NewSignature SignatureCreator = Hacks.GetConstructor<NewSignature>(Types.Signature, 3);
		static readonly FOT GetSignatureType = Hacks.GetPropertyGetter<FOT>(Types.Signature, "FieldType");
		
		class GetSignatureHacks
		{
			public static readonly Func<Module,object> GetMetadataImport = Hacks.GetPropertyGetter<Func<Module,object>>(Types.RuntimeModule, "MetadataImport");
			public static readonly Func<object,int,object> GetSigOfFieldDef = Hacks.GetInvoker<Func<object,int,object>>(Types.MetadataImport, "GetSigOfFieldDef", true);
			public static readonly Func<object,int,object> GetSigOfMethodDef = Hacks.GetInvoker<Func<object,int,object>>(Types.MetadataImport, "GetSigOfMethodDef", true);
			public static readonly Func<object,IntPtr> GetSignaturePtr = Hacks.GetPropertyGetter<Func<object,IntPtr>>(Types.ConstArray, "Signature");
			public static readonly Func<object,int> GetSignatureLength = Hacks.GetPropertyGetter<Func<object,int>>(Types.ConstArray, "Length");
		}
	}
}
