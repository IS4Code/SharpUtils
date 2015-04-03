/* Date: 3.4.2015, Time: 17:09 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace IllidanS4.SharpUtils.Reflection
{
	using FCsbIDict = Func<CallSiteBinder,IDictionary<Type,object>>;
	using FTArrT = Func<Type[],Type>;
	using FOT = Func<object,Type>;
	
	partial class ReflectionTools
	{
		static readonly FCsbIDict GetCache = Hacks.GetFieldGetter<FCsbIDict>(typeof(CallSiteBinder), "Cache");
    	static readonly FTArrT MakeNewCustomDelegate = Hacks.GetInvoker<FTArrT>(Types.DelegateHelpers, "MakeNewCustomDelegate", false);
		static readonly FTArrT MakeNewDelegate = Hacks.GetInvoker<FTArrT>(Types.DelegateHelpers, "MakeNewDelegate", false);
		
		unsafe delegate object NewSignature(void* sigptr, int siglength, Type declaringType);
		static readonly NewSignature SignatureCreator = Hacks.GetConstructor<NewSignature>(Types.Signature, 3);
		static readonly FOT GetSignatureType = Hacks.GetPropertyGetter<FOT>(Types.Signature, "FieldType");
		
		class GetSignatureHacks
		{
			public static readonly Func<Module,object> GetMetadataImport = Hacks.GetPropertyGetter<Func<Module,object>>(Types.RuntimeModule, "MetadataImport");
			public static readonly Func<object,int,object> GetSigOfFieldDef = Hacks.GetInvoker<Func<object,int,object>>(Types.MetadataImport, "GetSigOfFieldDef", true);
			public static readonly Func<object,IntPtr> GetSignaturePtr = Hacks.GetPropertyGetter<Func<object,IntPtr>>(Types.ConstArray, "Signature");
			public static readonly Func<object,int> GetSignatureLength = Hacks.GetPropertyGetter<Func<object,int>>(Types.ConstArray, "Length");
		}
	}
}
