/* ‎Date: 20.12.‎2012, Time: ‏‎17:01 */
using System;

namespace IllidanS4.SharpUtils.Accessing
{
	/// <summary>
	/// Read-write accessor that uses a getter and setter method.
	/// </summary>
	public class DefinedReadWriteAccessor<T> : BasicReadWriteAccessor<T>
	{
		Func<T> getter;
		Action<T> setter;
		
		public DefinedReadWriteAccessor(Func<T> getter, Action<T> setter)
		{
			this.getter = getter;
			this.setter = setter;
		}
		
		public override T Item{
			get{
				return getter();
			}
			set{
				setter(value);
			}
		}
	}
}