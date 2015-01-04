/* Date: 3.12.2014, Time: 16:09 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using IllidanS4.SharpUtils.Reflection.Emit;

namespace IllidanS4.SharpUtils.Reflection.TypeSupport
{
	public class GenericType : TypeConstruct
	{
		private readonly Type[] typeArgs;
		private readonly Type typeDef;
		public override CorElementType CorElementType{
			get{
				return CorElementType.GenericInst;
			}
		}
		
		public GenericType(Type genericTypeDef, params Type[] typeArguments) : this(genericTypeDef, (IEnumerable<Type>)typeArguments)
		{
			
		}
		
		public GenericType(Type genericTypeDef, IEnumerable<Type> typeArguments) :
			base(
				genericTypeDef.UnderlyingSystemType.MakeGenericType(
					typeArguments.Select(t => t.UnderlyingSystemType).ToArray()
				)
			)
		{
			typeDef = genericTypeDef;
			typeArgs = typeArguments.ToArray();
		}
		
		public override Type[] GenericTypeArguments{
			get{
				return GetGenericArguments();
			}
		}
		
		public override Type GetGenericTypeDefinition()
		{
			return typeDef;
		}
		
		public override bool IsGenericType{
			get{
				return true;
			}
		}
		
		public override Type[] GetGenericArguments()
		{
			return (Type[])typeArgs.Clone();
		}
		
		public override string Name{
			get{
				return typeDef.Name;
			}
		}
		
		public override string Namespace{
			get{
				return typeDef.Namespace;
			}
		}
		
		public override string ToString()
		{
			return typeDef.FullName+"["+String.Join(",", typeArgs.Select(t => t.FullName))+"]";
		}
		
		public override string FullName{
			get{
				return ToString();
			}
		}
		
		protected override void AddSignature(SignatureHelper signature)
		{
			signature.AddElementType(CorElementType.GenericInst);
			signature.AddArgumentSignature(typeDef);
			signature.AddData(typeArgs.Length);
			foreach(var arg in typeArgs)
			{
				signature.AddArgumentSignature(arg);
			}
		}
	}
}
