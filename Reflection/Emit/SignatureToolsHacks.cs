/* Date: 3.4.2015, Time: 18:01 */
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection.Emit
{
	using AInt = Action<SignatureHelper,int>;
	using AUInt = Action<SignatureHelper,uint>;
	using AULong = Action<SignatureHelper,ulong>;
	using AEl = Action<SignatureHelper,byte>;
	using ATok = Action<SignatureHelper,int>;
	using AMod = Action<SignatureHelper,Module>;
	
	partial class SignatureTools
	{
		private static readonly Type sigType = typeof(SignatureHelper);
		private static readonly AInt Sig_AddData_int = Hacks.GetInvoker<AInt>(sigType, "AddData", true);
		private static readonly AUInt Sig_AddData_uint = Hacks.GetInvoker<AUInt>(sigType, "AddData", true);
		private static readonly AULong Sig_AddData_ulong = Hacks.GetInvoker<AULong>(sigType, "AddData", true);
		private static readonly AEl Sig_AddElementType = Hacks.GetInvoker<AEl>(sigType, "AddElementType", true);
		private static readonly ATok Sig_AddToken = Hacks.GetInvoker<ATok>(sigType, "AddToken", true);
		
		private static readonly AMod Sig_Init = Hacks.GetInvoker<AMod>(sigType, "Init", true);
		
	}
}
