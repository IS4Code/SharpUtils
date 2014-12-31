/* Date: 31.12.2014, Time: 13:07 */
using System;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.TypeSupport;

namespace IllidanS4.SharpUtils.Reflection
{
	public class VoidValueType : TypeConstruct
	{
		public static readonly VoidValueType Instance = new VoidValueType();
		
		public VoidValueType() : base(typeof(void))
		{
			
		}
		
		protected override void AddSignature(SignatureHelper signature)
		{
			signature.AddTypeTokenArgument(typeof(void));
		}
	}
}
