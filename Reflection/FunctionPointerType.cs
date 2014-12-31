/* Date: 12.12.2014, Time: 14:22 */
using System;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.TypeSupport;

namespace IllidanS4.SharpUtils.Reflection
{
	/// <summary>
	/// Description of FunctionPointerType.
	/// </summary>
	public class FunctionPointerType : TypeConstruct
	{
		public MethodCallSite Signature{get; private set;}
		
		public FunctionPointerType() : this(new MethodCallSite())
		{
			
		}
		
		public FunctionPointerType(MethodCallSite signature) : base(Types.Generated.FnPtr)
		{
			if(signature == null) throw new ArgumentNullException("signature");
			Signature = signature;
		}
		
		protected override void AddSignature(SignatureHelper signature)
		{
			signature.AddElementType(CorElementType.FnPtr);
			signature.AddElement(Signature);
		}
		
		public override string ToString()
		{
			return Signature.ToString()+"*";
		}

		public override string FullName{
			get{
				return this.ToString();
			}
		}
		
		public override string Name{
			get{
				return this.ToString();
			}
		}
	}
}
