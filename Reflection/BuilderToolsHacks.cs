/* Date: 3.4.2015, Time: 17:35 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection
{
	using FMTok = Func<int,MethodToken>;
	using FNatMod = Func<ModuleBuilder,Module>;
	using FTTok = Func<int,TypeToken>;
	using FFTok = Func<int,Type,FieldToken>;
	using FGetTok = Func<Module,byte[],int,int>;
	using AFbInit = Action<FieldBuilder,string,TypeBuilder,Type,FieldAttributes,int,FieldToken>;
	using FMbList = Func<TypeBuilder,IList<MethodBuilder>>;
	using FNewMth = Func<string,MethodAttributes,CallingConventions,Type,Type[],ModuleBuilder,TypeBuilder,bool,MethodBuilder>;
	using ATokSet = Action<MethodBuilder,MethodToken>;
	using AImpSet = Action<Module,int,MethodImplAttributes>;
	using FDefFld = Func<Module,int,string,byte[],int,FieldAttributes,int>;
	using FDefMth = Func<Module,int,string,byte[],int,MethodAttributes,int>;
	using AEnsC = Action<ILGenerator,int>;
	using AIntE = Action<ILGenerator,OpCode>;
	using ARcFx = Action<ILGenerator>;
	using APutI4 = Action<ILGenerator,int>;
	using FDnMMd = Func<Module>;
	using FIlMb = Func<ILGenerator,MethodInfo>;
	using FIlTok = Func<object,byte[],int>;
	using FDynSc = Func<ILGenerator,object>;
	
	partial class BuilderTools
	{
		private static readonly Type TModB = typeof(ModuleBuilder);
		private static readonly Type TMthB = typeof(MethodBuilder);
		private static readonly Type TFldB = typeof(FieldBuilder);
		private static readonly Type TTypB = typeof(TypeBuilder);
		private static readonly Type TIlGn = typeof(ILGenerator);
		private static readonly Type TDynM = typeof(DynamicMethod);
		private static readonly Type TDIlGn = Type.GetType("System.Reflection.Emit.DynamicILGenerator");
		
		private static readonly FGetTok GetTokFromTypeSpec = Hacks.GetInvoker<FGetTok>(TModB, "GetTokenFromTypeSpec", true);
		
		private static readonly AFbInit InitField = Hacks.GetFieldSetter<AFbInit>(TFldB, "m_fieldName", "m_typeBuilder", "m_fieldType", "m_Attributes", "m_fieldTok", "m_tkField");
		
		private static readonly FFTok NewFieldToken = Hacks.GetConstructor<FFTok>(typeof(FieldToken), 0);
		
		private static readonly FMTok NewMethodToken = Hacks.GetConstructor<FMTok>(typeof(MethodToken), 0);

		private static readonly FTTok NewTypeToken = Hacks.GetConstructor<FTTok>(typeof(TypeToken), 0);
		
		private static readonly FNatMod GetNativeModule = Hacks.GetInvoker<FNatMod>(TModB, "GetNativeHandle", true);

		//private delegate int DefineMemberDelegate<TMemberAttributes>(Module module, int tkParent, string name, byte[] signature, int sigLength, TMemberAttributes attributes);
		
		private static readonly FDefFld DefineFieldInternal = Hacks.GetInvoker<FDefFld>(TTypB, "DefineField", false);

		private static readonly FDefMth DefineMethodInternal = Hacks.GetInvoker<FDefMth>(TTypB, "DefineMethod", false);
		
		private static readonly FMbList GetMethodList = Hacks.GetFieldGetter<FMbList>(TTypB, "m_listMethods");
		
		private static readonly FNewMth NewMethod = Hacks.GetConstructor<FNewMth>(TMthB, 0);
		
		private static readonly ATokSet SetToken = Hacks.GetInvoker<ATokSet>(TMthB, "SetToken", true);
		
		private static readonly AImpSet SetMethodImpl = Hacks.GetInvoker<AImpSet>(TTypB, "SetMethodImpl", false);
		
		private static readonly AEnsC EnsureCapacity = Hacks.GetInvoker<AEnsC>(TIlGn, "EnsureCapacity", true);
		
		private static readonly AIntE InternalEmit = Hacks.GetInvoker<AIntE>(TIlGn, "InternalEmit", true);
		
		private static readonly ARcFx RecordTokenFixup = Hacks.GetInvoker<ARcFx>(TIlGn, "RecordTokenFixup", true);
		
		private static readonly APutI4 PutInteger4 = Hacks.GetInvoker<APutI4>(TIlGn, "PutInteger4", true);
		
		private static readonly FDnMMd GetDynamicMethodsModule = Hacks.GetInvoker<FDnMMd>(TDynM, "GetDynamicMethodsModule", false);
		
		private static readonly FIlMb GetMethodBuilder = Hacks.GetFieldGetter<FIlMb>(TIlGn, "m_methodBuilder");
		
		private static readonly FIlTok GetTokenFor = Hacks.GetInvoker<FIlTok>(Type.GetType("System.Reflection.Emit.DynamicScope"), "GetTokenFor", true);
		
		private static readonly FDynSc GetDynamicScope = Hacks.GetFieldGetter<FDynSc>(TDIlGn, "m_scope");
	}
}
