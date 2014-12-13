/* Date: 3.12.2014, Time: 17:04 */
using System;

namespace IllidanS4.SharpUtils.Reflection.TypeSupport
{
	public abstract class TypeAppendConstruct : TypeConstruct
	{
		public TypeAppendConstruct(Type elementType, Type delegatingType) : base(elementType, delegatingType)
		{
			
		}
		
		protected abstract string Append(string name);
		
		public override string ToString()
		{
			return Append(base.ToString());
		}
		
		public override string FullName{
			get{
				return Append(base.FullName);
			}
		}
	}
}
