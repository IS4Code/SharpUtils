using System;
using System.ComponentModel;

namespace IllidanS4.SharpUtils
{
	/// <summary>
	/// This is the opposite of <see cref="System.Nullable&lt;T&gt;"/>. Reference type enclosed in this structure should never be null.
	/// </summary>
	public struct NonNullable<T> : IEquatable<NonNullable<T>> where T : class
	{
		public T Value{
			get{
				if(value == null) value = Empty<T>.Value;
				if(value == null) throw new InvalidOperationException("Used NonNullable of "+TypeOf<T>.TypeID.Name+" instance has not yet been assigned to an object. Please assign an object to it before using it.");
				return value;
			}
			set{
				if(value == null) throw new ArgumentNullException("value");
				this.value = value;
			}
		}
		
		public NonNullable(T value) : this()
		{
			if(value == null) throw new ArgumentNullException("value");
			this.value = value;
		}
		
		public bool Equals(NonNullable<T> other)
		{
			return Value.Equals(other.Value);
		}
	
		public override bool Equals(object obj)
		{
			if(obj is NonNullable<T>)
			{
				return this.Equals((NonNullable<T>)obj);
			}else{
				return this.Value.Equals(obj);
			}
		}
		
		public override string ToString()
		{
			return Value.ToString();
		}
		
		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
		
		public static implicit operator NonNullable<T>(T value)
		{
			if(value is NonNullable<T>) return (NonNullable<T>)(object)value;
			return new NonNullable<T>(value);
		}
		
		public static implicit operator T(NonNullable<T> nonnullable)
		{
			return nonnullable.Value;
		}
		
		public static bool operator ==(NonNullable<T> lhs, NonNullable<T> rhs)
		{
			if(lhs.value == null || rhs.value == null) return false;
			return lhs.Value == rhs.Value;
		}
		
		public static bool operator !=(NonNullable<T> lhs, NonNullable<T> rhs)
		{
			return !(lhs == rhs);
		}
	
		[Description("Please don't change this.")]
		private T value;
	}
}