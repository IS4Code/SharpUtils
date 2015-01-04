/* Date: 31.12.2014, Time: 13:07 */
using System;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.TypeSupport;

namespace IllidanS4.SharpUtils.Reflection
{
	/// <summary>
	/// Used to distinguish between void and System.Void in signatures.
	/// </summary>
	public class VoidValueType : TypeConstruct
	{
		public static readonly VoidValueType Instance = new VoidValueType();
		public override CorElementType CorElementType{
			get{
				return CorElementType.ValueType;
			}
		}
		
		public VoidValueType() : base(typeof(void))
		{
			
		}
		
		protected override void AddSignature(SignatureHelper signature)
		{
			signature.AddTypeTokenArgument(typeof(void));
		}
	}
}
