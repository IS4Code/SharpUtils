/* Date: 6.9.2014, Time: 12:46 */
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace IllidanS4.SharpUtils.Reflection
{
	public struct Property<T> : IEquatable<Property<T>>, IEquatable<PropertyInfo>
	{
		private readonly PropertyInfo property;
		private static readonly Type propType = typeof(T);
		
		public PropertyInfo Value{
			get{
				return property;
			}
		}
		
		public Property(PropertyInfo property)
		{
			if(property == null)
			{
				throw new ArgumentNullException("expr");
			}
			if(property.PropertyType != propType)
			{
				throw new ArgumentException("Property is not of type "+propType.ToString()+".");
			}
			this.property = property;
		}
		
		public Property(Expression<Func<T>> expr) : this(expr.Body as MemberExpression)
		{
			if(expr == null)
			{
				throw new ArgumentNullException("expr");
			}
		}
		
		public Property(MemberExpression expr) : this(expr.Member as PropertyInfo)
		{
			if(expr == null)
			{
				throw new ArgumentNullException("expr");
			}
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return ((obj is Property<T>) && Equals((Property<T>)obj)) || ((obj is PropertyInfo) && Equals((PropertyInfo)obj));
		}
		
		public bool Equals(Property<T> other)
		{
			return true;
		}
		
		public bool Equals(PropertyInfo other)
		{
			return true;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			return hashCode;
		}
		
		public static bool operator ==(Property<T> lhs, Property<T> rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(Property<T> lhs, Property<T> rhs)
		{
			return !(lhs == rhs);
		}
		
		public static implicit operator PropertyInfo(Property<T> obj)
		{
			return obj.Value;
		}
		
		public static implicit operator Property<T>(PropertyInfo obj)
		{
			return new Property<T>(obj);
		}
		
		public static implicit operator Property<T>(Expression<Func<T>> expr)
		{
			return new Property<T>(expr);
		}
		#endregion
	}
}
