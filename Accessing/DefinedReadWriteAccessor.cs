/* ‎Date: 20.12.‎2012, Time: ‏‎17:01 */
using System;

namespace IllidanS4.SharpUtils.Accessing
{
	/// <summary>
	/// Read-write accessor that uses a getter and setter method.
	/// </summary>
	public class DefinedReadWriteAccessor<T> : BasicReadWriteAccessor<T>, IDefinedReadWriteAccessor
	{
		public Func<T> Getter{get; private set;}
		public Action<T> Setter{get; private set;}
		
		public DefinedReadWriteAccessor(Func<T> getter, Action<T> setter)
		{
			Getter = getter;
			Setter = setter;
		}
		
		public override T Item{
			get{
				return Getter();
			}
			set{
				Setter(value);
			}
		}
		
		MulticastDelegate IDefinedWriteAccessor.Setter{
			get{
				return Setter;
			}
		}
		
		MulticastDelegate IDefinedReadAccessor.Getter{
			get{
				return Getter;
			}
		}
	}
	
	public interface IDefinedReadWriteAccessor : IDefinedReadAccessor, IDefinedWriteAccessor, IReadWriteAccessor
	{
		
	}
	
	public static class DefinedReadWriteAccessor
	{
		public static DefinedReadWriteAccessor<T> Create<T>(Func<T> getter, Action<T> setter)
		{
			return new DefinedReadWriteAccessor<T>(getter, setter);
		}
	}
}