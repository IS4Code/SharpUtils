using System;
using System.ComponentModel;

namespace IllidanS4.SharpUtils
{
	/// <summary>
	/// This is the opposite of <see cref="System.Nullable{T}"/>. Reference type enclosed in this structure should never be null.
	/// </summary>
	public struct NonNullable<T> : IEquatable<NonNullable<T>> where T : class
	{
		/// <summary>
		/// The non-null value of this object.
		/// </summary>
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
		
		/// <summary>
		/// Creates a non-nullable value from a specific value.
		/// </summary>
		/// <param name="value">The non-null value.</param>
		/// <exception cref="System.ArgumentNullException">When the argument is null.</exception>
		public NonNullable(T value) : this()
		{
			if(value == null) throw new ArgumentNullException("value");
			this.value = value;
		}
		
		/// <summary>
		/// Returns true if the value of this object equals another one's.
		/// </summary>
		public bool Equals(NonNullable<T> other)
		{
			return Value.Equals(other.Value);
		}
	
		/// <summary>
		/// Returns true if the value of this object equals another one's.
		/// </summary>
		public override bool Equals(object obj)
		{
			if(obj is NonNullable<T>)
			{
				return this.Equals((NonNullable<T>)obj);
			}else{
				return this.Value.Equals(obj);
			}
		}
		
		/// <summary>
		/// Returns the string representation of the value.
		/// </summary>
		public override string ToString()
		{
			return Value.ToString();
		}
		
		/// <summary>
		/// Returns the hash of the value.
		/// </summary>
		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
		
		/// <summary>
		/// Converts a non-null value to its non-nullable representation.
		/// </summary>
		public static implicit operator NonNullable<T>(T value)
		{
			if(value is NonNullable<T>) return (NonNullable<T>)(object)value;
			return new NonNullable<T>(value);
		}
		
		/// <summary>
		/// Converts a its non-nullable representation to its value.
		/// </summary>
		public static implicit operator T(NonNullable<T> nonnullable)
		{
			return nonnullable.Value;
		}
		
		/// <summary>
		/// Returns true if two non-nullable values are equal.
		/// </summary>
		public static bool operator ==(NonNullable<T> lhs, NonNullable<T> rhs)
		{
			if(lhs.value == null || rhs.value == null) return false;
			return lhs.Value == rhs.Value;
		}
		
		/// <summary>
		/// Returns true if two non-nullable values are not equal.
		/// </summary>
		public static bool operator !=(NonNullable<T> lhs, NonNullable<T> rhs)
		{
			return !(lhs == rhs);
		}
	
		[Description("Please don't change this.")]
		private T value;
	}
}