/* Date: 6.9.2014, TDelegateime: 12:17 */
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace IllidanS4.SharpUtils.Reflection
{
	public struct methodof<TDelegate> : IEquatable<methodof<TDelegate>>, IEquatable<MethodInfo> where TDelegate : class
	{
		public MethodBase Value{
			get; private set;
		}
		
		static methodof()
		{
			Type t = typeof(TDelegate);
			if(!typeof(Delegate).IsAssignableFrom(t))
			{
				throw new TypeLoadException("Generic argument must be a delegate type.");
			}
		}
		
		public methodof(TDelegate method) : this()
		{
			Value = ((Delegate)(object)method).Method;
		}
		
		public methodof(Expression<TDelegate> expr) : this()
		{
			NewExpression ctor = expr.Body as NewExpression;
			if(ctor != null)
			{
				Value = ctor.Constructor;
			}else{
				MethodCallExpression call = expr.Body as MethodCallExpression;
				if(call == null) throw new ArgumentException("Expression must be MethodCallExpression or NewExpression.", "expr");
				Value = call.Method;
			}
		}
		
		public static implicit operator MethodInfo(methodof<TDelegate> m)
		{
			return m.Value as MethodInfo;
		}
		
		public static implicit operator ConstructorInfo(methodof<TDelegate> m)
		{
			return m.Value as ConstructorInfo;
		}
		
		public static implicit operator MethodBase(methodof<TDelegate> m)
		{
			return m.Value;
		}
		
		public static implicit operator TDelegate(methodof<TDelegate> m)
		{
			MethodInfo mi = m.Value as MethodInfo;
			if(mi != null)
			{
				return (TDelegate)(object)Delegate.CreateDelegate(typeof(TDelegate), mi);
			}else{
				return null;
			}
		}
		
		/*public static implicit operator methodof<TDelegate>(TDelegate m)
		{
			return new methodof<TDelegate>(m);
		}*/
		
		public static implicit operator methodof<TDelegate>(Expression<TDelegate> expr)
		{
			return new methodof<TDelegate>(expr);
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return ((obj is methodof<TDelegate>) && Equals((methodof<TDelegate>)obj)) || ((obj is TDelegate) && Equals((TDelegate)obj)) || ((obj is MethodInfo) && Equals((MethodInfo)obj));
		}
		
		public bool Equals(methodof<TDelegate> other)
		{
			return Object.Equals(this.Value, other.Value);
		}
		
		public bool Equals(MethodInfo other)
		{
			return Object.Equals((MethodInfo)this, other);
		}
		
		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
		
		public static bool operator ==(methodof<TDelegate> lhs, methodof<TDelegate> rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(methodof<TDelegate> lhs, methodof<TDelegate> rhs)
		{
			return !(lhs == rhs);
		}
		#endregion

	}
}
