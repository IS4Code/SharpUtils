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
		private static readonly InitFieldDelegate InitField = CreateInitFunc();
		private static InitFieldDelegate CreateInitFunc()
		{
			Type t = typeof(FieldBuilder);
			FieldInfo[] fields = new FieldInfo[]{
				t.GetField("m_fieldName", iflags),
				t.GetField("m_typeBuilder", iflags),
				t.GetField("m_fieldType", iflags),
				t.GetField("m_Attributes", iflags),
				t.GetField("m_fieldTok", iflags),
				t.GetField("m_tkField", iflags),
			};
			var p1 = Expression.Parameter(typeof(FieldBuilder));
			var args = fields.Select(f => Expression.Parameter(f.FieldType)).ToArray();
			var fexps = fields.Select(f => Expression.Field(p1, f));
			var fsets = fexps.Select((e,i) => Expression.Assign(e, args[i]));
			var exp = Expression.Block(fsets);
			var lam = Expression.Lambda<InitFieldDelegate>(exp, new[]{p1}.Concat(args));
			return lam.Compile();
		}
		
		private delegate FieldToken CreateFieldTokenDelegate(int field, Type fieldClass);
		private static readonly CreateFieldTokenDelegate NewFieldToken = CreateFieldTokenFunc();
		private static CreateFieldTokenDelegate CreateFieldTokenFunc()
		{
			var ctor = typeof(FieldToken).GetConstructors(iflags)[0];
			var p1 = Expression.Parameter(typeof(int));
			var p2 = Expression.Parameter(typeof(Type));
			var exp = Expression.New(ctor, p1, p2);
			var lam = Expression.Lambda<CreateFieldTokenDelegate>(exp, p1, p2);
			return lam.Compile();
		}
		
		private delegate MethodToken CreateMethodTokenDelegate(int str);
		private static readonly CreateMethodTokenDelegate NewMethodToken = CreateMethodTokenFunc();
		private static CreateMethodTokenDelegate CreateMethodTokenFunc()
		{
			var ctor = typeof(MethodToken).GetConstructors(iflags)[0];
			var p1 = Expression.Parameter(typeof(int));
			var exp = Expression.New(ctor, p1);
			var lam = Expression.Lambda<CreateMethodTokenDelegate>(exp, p1);
			return lam.Compile();
		}
		
		private delegate TypeToken CreateTypeTokenDelegate(int tok);
		private static readonly CreateTypeTokenDelegate NewTypeToken = Hacks.GenerateConstructor<CreateTypeTokenDelegate>(typeof(TypeToken), 0);
		
		private delegate Module GetNativeModuleDelegate(ModuleBuilder modb);
		private static readonly GetNativeModuleDelegate GetNativeModule = CreateModuleFunc();
		private static GetNativeModuleDelegate CreateModuleFunc()
		{
			MethodInfo mi = typeof(ModuleBuilder).GetMethod("GetNativeHandle", iflags);
			var p1 = Expression.Parameter(typeof(ModuleBuilder));
			var exp = Expression.Convert(Expression.Call(p1, mi), typeof(Module));
			var lam = Expression.Lambda<GetNativeModuleDelegate>(exp, p1);
			return lam.Compile();
		}
		
		private delegate int DefineMemberDelegate<TMemberAttributes>(Module module, int tkParent, string name, byte[] signature, int sigLength, TMemberAttributes attributes);
		
		private delegate int DefineFieldDelegate(Module module, int tkParent, string name, byte[] signature, int sigLength, FieldAttributes attributes);
		private static readonly DefineFieldDelegate DefineFieldInternal = CreateFieldFunc();
		private static DefineFieldDelegate CreateFieldFunc()
		{
			MethodInfo mi = typeof(TypeBuilder).GetMethod("DefineField", sflags);
			var p1 = Expression.Parameter(typeof(Module));
			var p2 = Expression.Parameter(typeof(int));
			var p3 = Expression.Parameter(typeof(string));
			var p4 = Expression.Parameter(typeof(byte[]));
			var p5 = Expression.Parameter(typeof(int));
			var p6 = Expression.Parameter(typeof(FieldAttributes));
			var exp = Expression.Call(mi, Expression.Convert(p1, Types.RuntimeModule), p2, p3, p4, p5, p6);
			var lam = Expression.Lambda<DefineFieldDelegate>(exp, p1, p2, p3, p4, p5, p6);
			return lam.Compile();
		}
		
		private delegate int DefineMethodDelegate(Module module, int tkParent, string name, byte[] signature, int sigLength, MethodAttributes attributes);
		private static readonly DefineMethodDelegate DefineMethodInternal = CreateMethodFunc();
		private static DefineMethodDelegate CreateMethodFunc()
		{
			MethodInfo mi = typeof(TypeBuilder).GetMethod("DefineMethod", sflags);
			var p1 = Expression.Parameter(typeof(Module));
			var p2 = Expression.Parameter(typeof(int));
			var p3 = Expression.Parameter(typeof(string));
			var p4 = Expression.Parameter(typeof(byte[]));
			var p5 = Expression.Parameter(typeof(int));
			var p6 = Expression.Parameter(typeof(MethodAttributes));
			var exp = Expression.Call(mi, Expression.Convert(p1, Types.RuntimeModule), p2, p3, p4, p5, p6);
			var lam = Expression.Lambda<DefineMethodDelegate>(exp, p1, p2, p3, p4, p5, p6);
			return lam.Compile();
		}
		
		/*private delegate void AddMethodDelegate(TypeBuilder builder, MethodBuilder method);
		private static readonly AddMethodDelegate AddMethod = CreateAddMethod();
		private static AddMethodDelegate CreateAddMethod()
		{
			var p1 = Expression.Parameter(typeof(TypeBuilder));
			var p2 = Expression.Parameter(typeof(MethodBuilder));
			var exp = Expression.Call(
				Expression.Field(p1, typeof(TypeBuilder).GetField("m_listMethods", iflags)),
				"Add",
				null,
				p2
			);
			var lam = Expression.Lambda<AddMethodDelegate>(exp, p1, p2);
			return lam.Compile();
		}*/
		
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
