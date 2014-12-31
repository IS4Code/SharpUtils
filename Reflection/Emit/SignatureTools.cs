/* Date: 29.11.2014, Time: 23:14 */
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using IllidanS4.SharpUtils.Accessing;

namespace IllidanS4.SharpUtils.Reflection.Emit
{
	public static class SignatureTools
	{
		private const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		
		private static readonly Type sigType = typeof(SignatureHelper);
		private static readonly Action<SignatureHelper,int> Sig_AddData_int = (Action<SignatureHelper,int>)Delegate.CreateDelegate(typeof(Action<SignatureHelper,int>), sigType.GetMethod("AddData", flags, null, new[]{typeof(int)}, null));
		private static readonly Action<SignatureHelper,uint> Sig_AddData_uint = (Action<SignatureHelper,uint>)Delegate.CreateDelegate(typeof(Action<SignatureHelper,uint>), sigType.GetMethod("AddData", flags, null, new[]{typeof(uint)}, null));
		private static readonly Action<SignatureHelper,ulong> Sig_AddData_ulong = (Action<SignatureHelper,ulong>)Delegate.CreateDelegate(typeof(Action<SignatureHelper,ulong>), sigType.GetMethod("AddData", flags, null, new[]{typeof(ulong)}, null));
		private static readonly Action<SignatureHelper,byte> Sig_AddElementType = (Action<SignatureHelper,byte>)Delegate.CreateDelegate(typeof(Action<SignatureHelper,byte>), sigType.GetMethod("AddElementType", flags));
		private static readonly Action<SignatureHelper,int> Sig_AddToken = (Action<SignatureHelper,int>)Delegate.CreateDelegate(typeof(Action<SignatureHelper,int>), sigType.GetMethod("AddToken", flags, null, new[]{typeof(int)}, null));
		
		private static readonly Action<SignatureHelper,Module> Sig_Init = (Action<SignatureHelper,Module>)Delegate.CreateDelegate(typeof(Action<SignatureHelper,Module>), sigType.GetMethod("Init", flags, null, new[]{typeof(Module)}, null));
		
		
		public static void AddData(this SignatureHelper signature, byte data)
		{
			Sig_AddElementType(signature, data);
		}
		
		public static void AddElementType(this SignatureHelper signature, CorElementType elementType)
		{
			Sig_AddElementType(signature, (byte)elementType);
		}
		
		public static void AddElement(this SignatureHelper signature, ISignatureElement arg)
		{
			arg.AddSignature(signature);
		}
		
		public static void AddArgumentSignature(this SignatureHelper signature, Type arg)
		{
			ISignatureElement sig = arg as ISignatureElement;
			if(sig != null)
			{
				signature.AddElement(sig);
			}else{
				signature.AddArgument(arg.UnderlyingSystemType);
			}
		}
		
		public static void AddData(this SignatureHelper signature, int data)
		{
			Sig_AddData_int(signature, data);
		}
		
		public static void AddTypeToken(this SignatureHelper signature, TypeToken clsToken, CorElementType CorType)
		{
			Sig_AddElementType(signature, (byte)CorType);
			Sig_AddToken(signature, clsToken.Token);
		}
		
		private static readonly Func<SignatureHelper, ModuleBuilder> get_m_module = create_get_m_module();
		
		private static Func<SignatureHelper, ModuleBuilder> create_get_m_module()
		{
			var p1 = Expression.Parameter(typeof(SignatureHelper));
			var exp = Expression.Field(p1, typeof(SignatureHelper).GetField("m_module", flags));
			return Expression.Lambda<Func<SignatureHelper, ModuleBuilder>>(exp, p1).Compile();
		}
		
		public static void AddTypeTokenArgument(this SignatureHelper signature, Type clsArgument)
		{
			var corType = clsArgument.IsValueType ? CorElementType.ValueType : CorElementType.Class;
			var mod = get_m_module(signature);
			AddTypeToken(signature, mod.GetTypeToken(clsArgument), corType);
		}
		
		public static SignatureHelper GetSigHelper(ModuleBuilder mod = null)
		{
			var sig = (SignatureHelper)FormatterServices.GetUninitializedObject(sigType);
			Sig_Init(sig, mod);
			return sig;
		}
		
		public static SignatureHelper GetSigHelper(MdSigCallingConvention conv, ModuleBuilder mod = null)
		{
			var sig = (SignatureHelper)FormatterServices.GetUninitializedObject(sigType);
			Sig_Init(sig, mod);
			sig.AddData((byte)conv);
			return sig;
		}
		
		public static byte[] EncodeInteger(int i)
		{
			return EncodeInteger(unchecked((uint)i));
		}
		
		[CLSCompliant(false)]
		public static byte[] EncodeInteger(uint i)
		{
			if(i <= 0x7F) return new[]{(byte)i};
			else if(i <= 0x3FFF) return new[]{(byte)(((i&0xFF00)>>8)|0x80), (byte)(i&0xFF)};
			else return new[]{(byte)(((i&0xFF000000)>>24)|0xC0), (byte)((i&0xFF0000)>>16), (byte)((i&0xFF00)>>8), (byte)(i&0xFF)};
		}
	}
}
