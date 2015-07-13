/* Date: ‎20.12.‎2012, Time: ‏‎17:01 */
using System;
namespace IllidanS4.SharpUtils.Accessing
{
	public class DefinedWriteAccessor<T> : BasicWriteAccessor<T>, IDefinedWriteAccessor
	{
		public Action<T> Setter{get; private set;}
		
		public DefinedWriteAccessor(Action<T> setter)
		{
			Setter = setter;
		}
		
		public override T Item{
			set{
				Setter(value);
			}
		}
		
		MulticastDelegate IDefinedWriteAccessor.Setter{
			get{
				return Setter;
			}
		}
	}
	
	public interface IDefinedWriteAccessor : IWriteAccessor
	{
		MulticastDelegate Setter{get;}
	}
	
	public static class DefinedWriteAccessor
	{
		public static DefinedWriteAccessor<T> Create<T>(Action<T> setter)
		{
			return new DefinedWriteAccessor<T>(setter);
		}
	}
}