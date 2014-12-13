/* Date: 13.12.2014, Time: 11:16 */
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using IllidanS4.SharpUtils.Reflection.TypeSupport;

namespace IllidanS4.SharpUtils.Reflection.Emit
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
			int tok = TB_DefineField.Invoke(mod, builder.TypeToken.Token, name, sigd, sigd.Length, attributes);
			InitField.Invoke(fb, name, builder, type.UnderlyingSystemType, attributes, tok, CreateFieldToken(tok, type.UnderlyingSystemType));
			return fb;
		}
		
		private static readonly Action<FieldBuilder,string,TypeBuilder,Type,FieldAttributes,int,FieldToken> InitField = CreateInitFunc();
		private static Action<FieldBuilder,string,TypeBuilder,Type,FieldAttributes,int,FieldToken> CreateInitFunc()
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
			var lam = Expression.Lambda<Action<FieldBuilder,string,TypeBuilder,Type,FieldAttributes,int,FieldToken>>(exp, new[]{p1}.Concat(args));
			return lam.Compile();
		}
		
		private static readonly Func<int,Type,FieldToken> CreateFieldToken = CreateTokenFunc();
		private static Func<int,Type,FieldToken> CreateTokenFunc()
		{
			var ctor = typeof(FieldToken).GetConstructors(iflags)[0];
			var p1 = Expression.Parameter(typeof(int));
			var p2 = Expression.Parameter(typeof(Type));
			var exp = Expression.New(ctor, p1, p2);
			var lam = Expression.Lambda<Func<int,Type,FieldToken>>(exp, p1, p2);
			return lam.Compile();
		}
		
		private static readonly Func<ModuleBuilder, Module> GetNativeModule = CreateModuleFunc();
		private static Func<ModuleBuilder, Module> CreateModuleFunc()
		{
			MethodInfo mi = typeof(ModuleBuilder).GetMethod("GetNativeHandle", iflags);
			var p1 = Expression.Parameter(typeof(ModuleBuilder));
			var exp = Expression.Convert(Expression.Call(p1, mi), typeof(Module));
			var lam = Expression.Lambda<Func<ModuleBuilder, Module>>(exp, p1);
			return lam.Compile();
		}
		
		private static readonly Func<Module, int, string, byte[], int, FieldAttributes, int> TB_DefineField = CreateFieldFunc();
		private static Func<Module, int, string, byte[], int, FieldAttributes, int> CreateFieldFunc()
		{
			MethodInfo mi = typeof(TypeBuilder).GetMethod("DefineField", sflags);
			var p1 = Expression.Parameter(typeof(Module));
			var p2 = Expression.Parameter(typeof(int));
			var p3 = Expression.Parameter(typeof(string));
			var p4 = Expression.Parameter(typeof(byte[]));
			var p5 = Expression.Parameter(typeof(int));
			var p6 = Expression.Parameter(typeof(FieldAttributes));
			var exp = Expression.Call(mi, Expression.Convert(p1, Types.RuntimeModule), p2, p3, p4, p5, p6);
			var lam = Expression.Lambda<Func<Module, int, string, byte[], int, FieldAttributes, int>>(exp, p1, p2, p3, p4, p5, p6);
			return lam.Compile();
		}
	}
}
