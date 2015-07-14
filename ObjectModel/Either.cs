/* Date: 14.7.2015, Time: 1:20 */
using System;

namespace IllidanS4.SharpUtils.ObjectModel
{
	/// <summary>
	/// Contains a value of type TFirst or a value of type TSecond.
	/// Implementation of ECMA TR/89.
	/// </summary>
	public struct Either<TFirst, TSecond> : IEquatable<Either<TFirst, TSecond>>
	{
		readonly bool isSecond;
		readonly TFirst first;
		readonly TSecond second;
		
		public Either(TFirst first) : this()
		{
			this.first = first;
			isSecond = false;
		}
		
		public Either(TSecond second) : this()
		{
			this.second = second;
			isSecond = true;
		}
		
		public override bool Equals(object other)
		{
			if(other == null)
			{
				return false;
			}else if(other is Either<TFirst, TSecond>)
			{
				return Equals((Either<TFirst, TSecond>)other);
			}else{
				return false;
			}
		}
		
		public bool Equals(Either<TFirst, TSecond> other)
		{
			if(!isSecond && !other.isSecond)
			{
				var eq = first as IEquatable<TFirst>;
				if(eq != null)
				{
					return eq.Equals(other.first);
				}
				return first.Equals(other.first);
			}else if(isSecond && other.isSecond)
			{
				var eq = second as IEquatable<TFirst>;
				if(eq != null)
				{
					return eq.Equals(other.first);
				}
				return second.Equals(other.second);
			}else{
				return false;
			}
		}
		
		public TFirst First{
			get{
				if(!isSecond) return first;
				throw new InvalidOperationException("The object doesn't contain this value.");
			}
		}
		
		public TSecond Second{
			get{
				if(isSecond) return second;
				throw new InvalidOperationException("The object doesn't contain this value.");
			}
		}
		
		public bool IsFirst{
			get{
				return !isSecond;
			}
		}
		
		public bool IsSecond{
			get{
				return !isSecond;
			}
		}
		
		public override int GetHashCode()
		{
			if(isSecond)
			{
				return second.GetHashCode();
			}else{
				return first.GetHashCode();
			}
		}
		
		public override string ToString()
		{
			if(isSecond)
			{
				return second.ToString();
			}else{
				return first.ToString();
			}
		}
		
		public static bool operator ==(Either<TFirst, TSecond> left, Either<TFirst, TSecond> right)
		{
			return left.Equals(right);
		}
		
		public static bool operator !=(Either<TFirst, TSecond> left, Either<TFirst, TSecond> right)
		{
			return !left.Equals(right);
		}
		
		public static Either<TFirst, TSecond> MakeFirst(TFirst first)
		{
			return new Either<TFirst, TSecond>(first);
		}
		
		public static Either<TFirst, TSecond> MakeSecond(TSecond second)
		{
			return new Either<TFirst, TSecond>(second);
		}
		
		public static explicit operator TFirst(Either<TFirst, TSecond> value)
		{
			return value.First;
		}
		
		public static explicit operator TSecond(Either<TFirst, TSecond> value)
		{
			return value.Second;
		}
		
		public static implicit operator Either<TFirst, TSecond>(TFirst first)
		{
			return MakeFirst(first);
		}
		
		public static implicit operator Either<TFirst, TSecond>(TSecond second)
		{
			return MakeSecond(second);
		}
	}
}
