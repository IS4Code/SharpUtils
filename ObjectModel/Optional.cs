/* Date: 14.7.2015, Time: 0:44 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.ObjectModel
{
	/// <summary>
	/// Represents an optional value. Similar to <see cref="System.Nullable{T}"/> except that the generic type parameter is not constrained in any way.
	/// Conforms to ECMA TR/89.
	/// </summary>
	public struct Optional<T> : IComparable, IComparable<T>, IComparable<Optional<T>?>, IComparable<Optional<T>>, IEquatable<T>, IEquatable<Optional<T>?>, IEquatable<Optional<T>>, IEnumerable<T>
	{
		private readonly bool hasValue;
		private readonly T value;
		
		public Optional(T value)
		{
			this.value = value;
			hasValue = true;
		}
		
		public int CompareTo(object other)
		{
			if(other == null)
			{
				return hasValue ? 1 : 0;
			}else{
				return CompareTo((Optional<T>)other);
			}
		}
		
		public int CompareTo(Optional<T>? other)
		{
			if(other == null)
			{
				return hasValue ? 1 : 0;
			}else{
				return CompareTo(other.Value);
			}
		}
		
		public int CompareTo(Optional<T> other)
		{
			if(other.hasValue)
			{
				return CompareTo(other.value);
			}else{
				return hasValue ? 1 : 0;
			}
		}
		
		public int CompareTo(T other)
		{
			if(!hasValue)
			{
				return -1;
			}
			
			var comp = value as IComparable<T>;
			if(comp != null)
			{
				return comp.CompareTo(other);
			}
			var comp2 = value as IComparable;
			if(comp != null)
			{
				return comp.CompareTo(other);
			}
			throw new ArgumentException("Argument doesn't implement IComparable.");
		}
		
		public override bool Equals(object other)
		{
			if(other == null)
			{
				return !hasValue;
			}else if(other is Optional<T>)
			{
				return Equals((Optional<T>)other);
			}else if(other is T)
			{
				return Equals((T)other);
			}else{
				return false;
			}
		}
		
		public bool Equals(Optional<T>? other)
		{
			if(other == null)
			{
				return !hasValue;
			}
			return Equals(other.Value);
		}
		
		public bool Equals(Optional<T> other)
		{
			if(other.hasValue)
			{
				return Equals(other.value);
			}else{
				return !hasValue;
			}
		}
		
		public bool Equals(T other)
		{
			if(!hasValue)
			{
				return false;
			}
			
			var eq = value as IEquatable<T>;
			if(eq != null)
			{
				return eq.Equals(other);
			}
			return value.Equals(other);
		}
		
		public static Optional<T> FromNullable<T2>(T2? value) where T2 : struct, T
		{
			if(value == null)
			{
				return default(Optional<T>);
			}else{
				return new Optional<T>(value.Value);
			}
		}
		
		public static T2? ToNullable<T2>(Optional<T> value) where T2 : struct, T
		{
			if(!value.hasValue) return null;
			var val = value.value;
			return Extensions.FastCast<T, T2>(val);
		}
		
		public static T FromOptional(Optional<T> value)
		{
			return value.Value;
		}
		
		public static Optional<T> ToOptional(T value)
		{
			return new Optional<T>(value);
		}
		
		public override int GetHashCode()
		{
			if(!hasValue) return 0;
			return value.GetHashCode();
		}

		public T GetValueOrDefault()
		{
			return GetValueOrDefault(default(T));
		}
		
		public T GetValueOrDefault(T alternateDefaultValue)
		{
			if(hasValue) return value;
			return alternateDefaultValue;
		}
		
		public bool HasValue{
			get{
				return hasValue;
			}
		}
		
		public static bool operator ==(Optional<T> left, Optional<T> right)
		{
			return left.Equals(right);
		}
		
		public static bool operator !=(Optional<T> left, Optional<T> right)
		{
			return !left.Equals(right);
		}
		
		public static explicit operator T(Optional<T> value)
		{
			return FromOptional(value);
		}
		
		public static implicit operator Optional<T>(T value)
		{
			return ToOptional(value);
		}
		
		public override string ToString()
		{
			if(hasValue) return value.ToString();
			return String.Empty;
		}
		
		public T Value{
			get{
				if(hasValue) return value;
				throw new InvalidOperationException("This object doesn't contain any value.");
			}
		}
		
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			if(hasValue) yield return value;
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>)this).GetEnumerator();
		}
	}
}
