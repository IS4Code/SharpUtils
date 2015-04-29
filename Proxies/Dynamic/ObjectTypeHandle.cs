/* Date: 25.4.2015, Time: 1:48 */
using System;
using System.Runtime.Remoting;

namespace IllidanS4.SharpUtils.Proxies.Dynamic
{
	/// <summary>
	/// A remote handle to an object, exposing its type.
	/// </summary>
	public class ObjectTypeHandle : ObjectHandle
	{
		public ObjectTypeHandle(object o) : base(o)
		{
			
		}
		
		public Type Type{
			get{
				return this.Unwrap().GetType();
			}
		}
	}
}
