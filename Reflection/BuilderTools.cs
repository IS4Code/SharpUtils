/* Date: 13.12.2014, Time: 11:16 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using IllidanS4.SharpUtils.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.TypeSupport;

namespace IllidanS4.SharpUtils.Reflection
{
	public static class BuilderTools
	{
		private const BindingFlags sflags = BindingFlags.Static | BindingFlags.NonPublic;
		private const BindingFlags iflags = BindingFlags.Instance | BindingFlags.NonPublic;
		
		/// <summary>
		/// Creates a new field using extended field type.
		/// </summary>
		/// <param name="builder">The TypeBuilder where the field will be created.</param>
		/// <param name="name">The name of the field.</param>
		/// <param name="type">The extended field type.</param>
		/// <param name="attributes">The field attributes.</param>
		/// <returns>The created FieldBuilder.</returns>
		public static FieldBuilder DefineFieldExtended(this TypeBuilder builder, string name, Type type, FieldAttributes attributes)
		{
			TypeConstruct tc = type as TypeConstruct;
			if(tc == null) return builder.DefineField(name, type, attributes);
			var t = TypeOf<FieldBuilder>.TypeID;
			FieldBuilder fb = (FieldBuilder)FormatterServices.GetUninitializedObject(t);
			var mod = GetNativeModule((ModuleBuilder)builder.Module);
			SignatureHelper sig = SignatureHelper.GetFieldSigHelper(builder.Module);
			sig.AddElement(tc);
			byte[] sigd = sig.GetSignature();
			int tok = DefineFieldInternal.Invoke(mod, builder.TypeToken.Token, name, sigd, sigd.Length, attributes);
			InitField.Invoke(fb, name, builder, type.UnderlyingSystemType, attributes, tok, NewFieldToken(tok, type.UnderlyingSystemType));
			return fb;
		}
		
		public static FieldBuilder DefineField(this TypeBuilder builder, string name, FieldAttributes attributes, FieldSignature signature)
		{
			return DefineFieldExtended(builder, name, signature.FieldType, attributes);
		}
		
		public static MethodBuilder DefineMethod(this TypeBuilder builder, string name, MethodAttributes attributes, MethodSignature signature)
		{
			var t = TypeOf<MethodBuilder>.TypeID;
			var modb = (ModuleBuilder)builder.Module;
			var mod = GetNativeModule(modb);
			byte[] sig = signature.GetSignature(modb);
			int tok = DefineMethodInternal.Invoke(mod, builder.TypeToken.Token, name, sig, sig.Length, attributes);
			MethodBuilder mb = NewMethod.Invoke(
				name, attributes, signature.CallingConvention, signature.ReturnType,
				signature.ParameterTypes, modb, builder, false
			);
			mb.GetToken(); //returns 0 but circumvents the RSA error (?)
			SetToken.Invoke(mb, NewMethodToken(tok));
			GetMethodList.Invoke(builder).Add(mb);
			return mb;
		}
		
		public static TypeToken GetTokenFromTypeSpec(this ModuleBuilder builder, byte[] signature)
		{
			return NewTypeToken(GetTokFromTypeSpec(builder, signature, signature.Length));
		}
		
		public static byte[] GetSignature(this ModuleBuilder builder, ISignatureElement sig)
		{
			SignatureHelper sh = SignatureTools.GetSigHelper(builder);
			sh.AddElement(sig);
			return sh.GetSignature();
		}
		
		private delegate int TokTypeDel(ModuleBuilder builder, byte[] signature, int length);
		private static readonly TokTypeDel GetTokFromTypeSpec = Hacks.GenerateInvoker<TokTypeDel>(typeof(ModuleBuilder), "GetTokenFromTypeSpec", true);
		
		private delegate void InitFieldDelegate(FieldBuilder builder, string name, TypeBuilder type, Type fieldType, FieldAttributes attributes, int i, FieldToken token);
		private static readonly InitFieldDelegate InitField = Hacks.GenerateFieldSetter<InitFieldDelegate>(
			typeof(FieldBuilder), "m_fieldName", "m_typeBuilder",
			"m_fieldType", "m_Attributes", "m_fieldTok", "m_tkField"
		);
		
		private delegate FieldToken CreateFieldTokenDelegate(int field, Type fieldClass);
		private static readonly CreateFieldTokenDelegate NewFieldToken = Hacks.GenerateConstructor<CreateFieldTokenDelegate>(typeof(FieldToken), 0);
		
		private static readonly Func<int,MethodToken> NewMethodToken = Hacks.GenerateConstructor<Func<int,MethodToken>>(typeof(MethodToken), 0);

		
		private static readonly Func<int,TypeToken> NewTypeToken = Hacks.GenerateConstructor<Func<int,TypeToken>>(typeof(TypeToken), 0);
		
		private delegate Module GetNativeModuleDelegate(ModuleBuilder modb);
		private static readonly GetNativeModuleDelegate GetNativeModule = Hacks.GenerateInvoker<GetNativeModuleDelegate>(typeof(ModuleBuilder), "GetNativeHandle", true);

		
		private delegate int DefineMemberDelegate<TMemberAttributes>(Module module, int tkParent, string name, byte[] signature, int sigLength, TMemberAttributes attributes);
		
		private delegate int DefineFieldDelegate(Module module, int tkParent, string name, byte[] signature, int sigLength, FieldAttributes attributes);
		private static readonly DefineFieldDelegate DefineFieldInternal = Hacks.GenerateInvoker<DefineFieldDelegate>(typeof(TypeBuilder), "DefineField", false);

		private delegate int DefineMethodDelegate(Module module, int tkParent, string name, byte[] signature, int sigLength, MethodAttributes attributes);
		private static readonly DefineMethodDelegate DefineMethodInternal = Hacks.GenerateInvoker<DefineMethodDelegate>(typeof(TypeBuilder), "DefineMethod", false);
		
		private delegate IList<MethodBuilder> tbmlistdel(TypeBuilder tb);
		private static readonly tbmlistdel GetMethodList = Hacks.GenerateFieldGetter<tbmlistdel>(typeof(TypeBuilder), "m_listMethods");
		
		private delegate MethodBuilder NewMethodDelegate(
			string name, MethodAttributes attributes,
			CallingConventions callingConvention, Type returnType, Type[] parameterTypes,
			ModuleBuilder mod, TypeBuilder type, bool bIsGlobalMethod
		);
		private static readonly NewMethodDelegate NewMethod = Hacks.GenerateConstructor<NewMethodDelegate>(typeof(MethodBuilder), 0);
		
		private delegate void tokdel(MethodBuilder method, MethodToken token);
		private static readonly tokdel SetToken = (tokdel)Delegate.CreateDelegate(typeof(tokdel), typeof(MethodBuilder).GetMethod("SetToken", iflags));
		
		private delegate void mimpdel(Module module, int tkMethod, MethodImplAttributes MethodImplAttributes);
		private static readonly mimpdel SetMethodImpl = Hacks.GenerateInvoker<mimpdel>(typeof(TypeBuilder), "SetMethodImpl");
	}
}
