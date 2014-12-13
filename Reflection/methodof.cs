/* Date: 6.9.2014, Time: 12:17 */
using System;
using System.Reflection;

namespace IllidanS4.SharpUtils.Reflection
{
	public struct methodof<T> : IEquatable<methodof<T>>, IEquatable<MethodInfo> where T : class
	{
		private readonly T method;
		
		public MethodInfo Value{
			get{
				return ((Delegate)(object)method).Method;
			}
		}
		
		static methodof()
		{
			Type t = typeof(T);
			if(typeof(Delegate).IsAssignableFrom(t))
			{
				throw new TypeLoadException("Generic argument must be a delegate type.");
			}
		}
		
		public methodof(T method)
		{
			this.method = method;
		}
		
		public static implicit operator MethodInfo(methodof<T> m)
		{
			return m.Value;
		}
		
		public static implicit operator T(methodof<T> m)
		{
			return m.method;
		}
		
		public static implicit operator methodof<T>(T m)
		{
			return new methodof<T>(m);
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return ((obj is methodof<T>) && Equals((methodof<T>)obj)) || ((obj is T) && Equals((T)obj)) || ((obj is MethodInfo) && Equals((MethodInfo)obj));
		}
		
		public bool Equals(methodof<T> other)
		{
			return Object.Equals(this.method, other.method);
		}
		
		public bool Equals(T other)
		{
			return Object.Equals(this.method, other);
		}
		
		public bool Equals(MethodInfo other)
		{
			return Object.Equals((MethodInfo)this, other);
		}
		
		public override int GetHashCode()
		{
			return method.GetHashCode();
		}
		
		public static bool operator ==(methodof<T> lhs, methodof<T> rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(methodof<T> lhs, methodof<T> rhs)
		{
			return !(lhs == rhs);
		}
		#endregion

	}
}
