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
	public static partial class BuilderTools
	{
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
	}
}
