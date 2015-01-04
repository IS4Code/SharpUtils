﻿/* Date: 4.1.2015, Time: 17:08 */
using System;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.TypeSupport;

namespace IllidanS4.SharpUtils.Reflection
{
	public class SignatureElementType : TypeConstruct
	{
		private readonly CorElementType elementType;
		public override CorElementType CorElementType{
			get{return elementType;}
		}
		
		public SignatureElementType(CorElementType elementType) : base(ReflectionTools.GetTypeFromElementType(elementType))
		{
			this.elementType = elementType;
		}
		
		protected override void AddSignature(SignatureHelper signature)
		{
			signature.AddElementType(elementType);
		}
	}
}
