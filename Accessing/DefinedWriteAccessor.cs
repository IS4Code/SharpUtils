/* Date: ‎20.12.‎2012, Time: ‏‎17:01 */
using System;
namespace IllidanS4.SharpUtils.Accessing
{
	public class DefinedWriteAccessor<T> : IWriteAccessor<T>
	{
		Action<T> setter;
		
		public DefinedWriteAccessor(Action<T> setter)
		{
			this.setter = setter;
		}
		
		public T Value{
			set{
				setter(value);
			}
		}
	}
}