/* Date: 6.4.2015, Time: 19:51 */
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection.TypeSupport
{
	public class PartialGenericType : GenericType
	{
		public override CorElementType CorElementType{
			get{
				return CorElementType.GenericInst;
			}
		}
		
		public PartialGenericType(Type genericTypeDef, IDictionary<int, Type> typeArgs) : base(genericTypeDef, MapTypeArgs(genericTypeDef, typeArgs))
		{
			
		}
		
		protected PartialGenericType(Type genericTypeDef, params Type[] typeArgs) : base(genericTypeDef, typeArgs)
		{
			
		}
		
		public override Type MakeGenericType(params Type[] typeArguments)
		{
			bool partial = false;
			Type[] args = UnderlyingSystemType.GetGenericArguments();
			int j = 0;
			for(int i = 0; i < args.Length; i++)
			{
				if(args[i].IsGenericParameter)
				{
					if(j >= typeArguments.Length)
					{
						partial = true;
					}else{
						args[i] = typeArguments[j++];
					}
				}
			}
			Type typeDef = GetGenericTypeDefinition();
			if(partial)
			{
				return new PartialGenericType(typeDef, args);
			}else{
				return typeDef.MakeGenericType(args);
			}
		}
		
		private static Type[] MapTypeArgs(Type genericTypeDef, IDictionary<int, Type> typeArgs)
		{
			var args = genericTypeDef.GetGenericArguments();
			foreach(var pair in typeArgs)
			{
				args[pair.Key] = pair.Value;
			}
			return args;
		}
	}
}
