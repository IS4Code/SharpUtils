/* Date: 7.4.2015, Time: 16:46 */
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace IllidanS4.SharpUtils.Reflection
{
	public struct Operand<T> : IEquatable<Operand<T>>, IEquatable<T>, IConvertible, IOperand<Operand<T>>
	{
		public readonly T Value;
		
		public Operand(T value)
		{
			Value = value;
		}
		
		public static implicit operator Operand<T>(T value)
		{
			return new Operand<T>(value);
		}
		
		public static implicit operator T(Operand<T> value)
		{
			return value.Value;
		}
		
		public static readonly Func<T,T,bool> Equality;
		public static readonly Func<T,T,bool> Inequality;
		public static readonly Func<T,T,bool> GreaterThan;
		public static readonly Func<T,T,bool> LessThan;
		public static readonly Func<T,T,bool> GreaterThanOrEqual;
		public static readonly Func<T,T,bool> LessThanOrEqual;
		public static readonly Func<T,T,T> BitwiseAnd;
		public static readonly Func<T,T,T> BitwiseOr;
		public static readonly Func<T,T,T> Addition;
		public static readonly Func<T,T,T> AdditionChecked;
		public static readonly Func<T,T,T> Subtraction;
		public static readonly Func<T,T,T> SubtractionChecked;
		public static readonly Func<T,T,T> Division;
		public static readonly Func<T,T,T> Modulus;
		public static readonly Func<T,T,T> Multiply;
		public static readonly Func<T,T,T> MultiplyChecked;
		public static readonly Func<T,T,T> ExclusiveOr;
		public static readonly Func<T,int,T> LeftShift;
		public static readonly Func<T,int,T> RightShift;
		public static readonly Func<T,T> UnaryNegation;
		public static readonly Func<T,T> UnaryNegationChecked;
		public static readonly Func<T,T> UnaryPlus;
		public static readonly Func<T,T> LogicalNot;
		public static readonly Func<T,T> OnesComplement;
		public static readonly Func<T,T> Increment;
		public static readonly Func<T,T> Decrement;
		public static readonly Func<T,bool> False;
		public static readonly Func<T,bool> True;
		
		public static class Convert<TOther>
		{
			public static readonly Func<T,TOther> To;
			public static readonly Func<TOther,T> From;
			public static readonly Func<T,TOther> ToChecked;
			public static readonly Func<TOther,T> FromChecked;
			
			static Convert()
			{
				To = ConvertExpression<T,TOther>(Expression.Convert);
				From = ConvertExpression<TOther,T>(Expression.Convert);
				ToChecked = ConvertExpression<T,TOther>(Expression.ConvertChecked);
				FromChecked = ConvertExpression<TOther,T>(Expression.ConvertChecked);
			}
			
			private static Func<TFrom,TTo> ConvertExpression<TFrom,TTo>(Func<Expression,Type,Expression> convertType)
			{
				ParameterExpression p = Expression.Parameter(TypeOf<TFrom>.TypeID);
				return CompileExpression<Func<TFrom,TTo>,TTo>(()=>convertType(p, TypeOf<TTo>.TypeID), p);
			}
		}
		
		static Operand()
		{
			Equality = BinaryExpression<T,bool>(Expression.Equal);
			Inequality = BinaryExpression<T,bool>(Expression.NotEqual);
			GreaterThan = BinaryExpression<T,bool>(Expression.GreaterThan);
			LessThan = BinaryExpression<T,bool>(Expression.LessThan);
			GreaterThanOrEqual = BinaryExpression<T,bool>(Expression.GreaterThanOrEqual);
			LessThanOrEqual = BinaryExpression<T,bool>(Expression.LessThanOrEqual);
			
			BitwiseAnd = BinaryExpression<T,T>(Expression.And);
			BitwiseOr = BinaryExpression<T,T>(Expression.Or);
			Addition = BinaryExpression<T,T>(Expression.Add);
			AdditionChecked = BinaryExpression<T,T>(Expression.AddChecked);
			Subtraction = BinaryExpression<T,T>(Expression.Subtract);
			SubtractionChecked = BinaryExpression<T,T>(Expression.SubtractChecked);
			Division = BinaryExpression<T,T>(Expression.Divide);
			Modulus = BinaryExpression<T,T>(Expression.Modulo);
			Multiply = BinaryExpression<T,T>(Expression.Multiply);
			MultiplyChecked = BinaryExpression<T,T>(Expression.MultiplyChecked);
			ExclusiveOr = BinaryExpression<T,T>(Expression.ExclusiveOr);
			
			LeftShift = BinaryExpression<int,T>(Expression.LeftShift);
			RightShift = BinaryExpression<int,T>(Expression.RightShift);
			
			UnaryNegation = UnaryExpression<T>(Expression.Negate);
			UnaryNegationChecked = UnaryExpression<T>(Expression.NegateChecked);
			UnaryPlus = UnaryExpression<T>(Expression.UnaryPlus);
			LogicalNot = UnaryExpression<T>(Expression.Not);
			OnesComplement = UnaryExpression<T>(Expression.OnesComplement);
			Increment = UnaryExpression<T>(Expression.Increment);
			Decrement = UnaryExpression<T>(Expression.Decrement);
			
			False = UnaryExpression<bool>(Expression.IsFalse);
			True = UnaryExpression<bool>(Expression.IsTrue);
		}
		
		private static Func<T,TParam2,TReturn> BinaryExpression<TParam2,TReturn>(Func<Expression,Expression,BinaryExpression> expressionType)
		{
			ParameterExpression p1 = Expression.Parameter(TypeOf<T>.TypeID);
			ParameterExpression p2 = Expression.Parameter(TypeOf<TParam2>.TypeID);
			return CompileExpression<Func<T,TParam2,TReturn>, TReturn>(()=>expressionType(p1, p2), p1, p2);
		}
		
		private static Func<T,TReturn> UnaryExpression<TReturn>(Func<Expression,UnaryExpression> expressionType)
		{
			ParameterExpression p1 = Expression.Parameter(TypeOf<T>.TypeID);
			return CompileExpression<Func<T,TReturn>, TReturn>(()=>expressionType(p1), p1);
		}
		
		private static TFunc CompileExpression<TFunc,TReturn>(Func<Expression> expression, params ParameterExpression[] parameters)
		{
			Type tRet = TypeOf<TReturn>.TypeID;
			Expression exp;
			try{
				exp = expression();
				if(exp.Type != tRet)
				{
					exp = Expression.Block(Expression.Throw(Expression.Constant(new NotImplementedException())), Expression.Default(tRet));
				}
			}catch(InvalidOperationException e)
			{
				exp = Expression.Block(Expression.Throw(Expression.Constant(e)), Expression.Default(tRet));
			}
			return Expression.Lambda<TFunc>(exp, parameters).Compile();
		}
		
		#region Equals and GetHashCode implementation
		
		public override bool Equals(object obj)
		{
			return ((obj is Operand<T>) && Equals((Operand<T>)obj)) || ((obj is T) && Equals((T)obj));
		}
 		
		public bool Equals(Operand<T> other)
		{
			return Value.Equals(other.Value);
		}
 		
		public bool Equals(T other)
		{
			return Value.Equals(other);
		}
		
		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
		
		public override string ToString()
		{
			return Value.ToString();
		}
		
		#endregion

		#region Operators
		
		public static bool operator ==(Operand<T> left, Operand<T> right)
		{
			return Equality(left.Value, right.Value);
		}
		
		public static bool operator !=(Operand<T> left, Operand<T> right)
		{
			return Inequality(left.Value, right.Value);
		}
		
		public static bool operator >(Operand<T> left, Operand<T> right)
		{
			return GreaterThan(left.Value, right.Value);
		}
		
		public static bool operator <(Operand<T> left, Operand<T> right)
		{
			return LessThan(left.Value, right.Value);
		}
		
		public static bool operator >=(Operand<T> left, Operand<T> right)
		{
			return GreaterThanOrEqual(left.Value, right.Value);
		}
		
		public static bool operator <=(Operand<T> left, Operand<T> right)
		{
			return LessThanOrEqual(left.Value, right.Value);
		}
		
		public static Operand<T> operator &(Operand<T> left, Operand<T> right)
		{
			return new Operand<T>(BitwiseAnd(left.Value, right.Value));
		}
		
		public static Operand<T> operator |(Operand<T> left, Operand<T> right)
		{
			return new Operand<T>(BitwiseOr(left.Value, right.Value));
		}
		
		public static Operand<T> operator +(Operand<T> left, Operand<T> right)
		{
			return new Operand<T>(Addition(left.Value, right.Value));
		}
		
		public static Operand<T> operator -(Operand<T> left, Operand<T> right)
		{
			return new Operand<T>(Subtraction(left.Value, right.Value));
		}
		
		public static Operand<T> operator /(Operand<T> left, Operand<T> right)
		{
			return new Operand<T>(Division(left.Value, right.Value));
		}
		
		public static Operand<T> operator %(Operand<T> left, Operand<T> right)
		{
			return new Operand<T>(Modulus(left.Value, right.Value));
		}
		
		public static Operand<T> operator *(Operand<T> left, Operand<T> right)
		{
			return new Operand<T>(Multiply(left.Value, right.Value));
		}
		
		public static Operand<T> operator <<(Operand<T> left, int right)
		{
			return new Operand<T>(LeftShift(left.Value, right));
		}
		
		public static Operand<T> operator >>(Operand<T> left, int right)
		{
			return new Operand<T>(RightShift(left.Value, right));
		}
		
		public static Operand<T> operator ^(Operand<T> left, Operand<T> right)
		{
			return new Operand<T>(ExclusiveOr(left.Value, right.Value));
		}
		
		public static Operand<T> operator -(Operand<T> value)
		{
			return new Operand<T>(UnaryNegation(value.Value));
		}
		
		public static Operand<T> operator +(Operand<T> value)
		{
			return new Operand<T>(UnaryPlus(value.Value));
		}
		
		public static Operand<T> operator !(Operand<T> value)
		{
			return new Operand<T>(LogicalNot(value.Value));
		}
		
		public static Operand<T> operator ~(Operand<T> value)
		{
			return new Operand<T>(OnesComplement(value.Value));
		}
		
		public static bool operator false(Operand<T> value)
		{
			return False(value.Value);
		}
		
		public static bool operator true(Operand<T> value)
		{
			return True(value.Value);
		}
		
		public static Operand<T> operator ++(Operand<T> value)
		{
			return new Operand<T>(Increment(value.Value));
		}
		
		public static Operand<T> operator --(Operand<T> value)
		{
			return new Operand<T>(Decrement(value.Value));
		}
		
		#endregion
		
		#region IConvertible
		
		TypeCode IConvertible.GetTypeCode()
		{
			return Type.GetTypeCode(TypeOf<T>.TypeID);
		}
		
		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert<bool>.ToChecked(Value);
		}
		
		char IConvertible.ToChar(IFormatProvider provider)
		{
			return Convert<char>.ToChecked(Value);
		}
		
		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert<sbyte>.ToChecked(Value);
		}
		
		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert<byte>.ToChecked(Value);
		}
		
		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert<short>.ToChecked(Value);
		}
		
		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert<ushort>.ToChecked(Value);
		}
		
		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert<int>.ToChecked(Value);
		}
		
		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert<uint>.ToChecked(Value);
		}
		
		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert<long>.ToChecked(Value);
		}
		
		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert<ulong>.ToChecked(Value);
		}
		
		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return Convert<float>.ToChecked(Value);
		}
		
		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert<double>.ToChecked(Value);
		}
		
		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert<decimal>.ToChecked(Value);
		}
		
		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return Convert<DateTime>.ToChecked(Value);
		}
		
		string IConvertible.ToString(IFormatProvider provider)
		{
			return Convert<string>.ToChecked(Value);
		}
		
		public Operand<TTo> ConvertTo<TTo>()
		{
			return new Operand<TTo>(Convert<TTo>.ToChecked(Value));
		}
		public static Operand<T> ConvertFrom<TFrom>(TFrom value)
		{
			return new Operand<T>(Convert<TFrom>.FromChecked(value));
		}
		
		private static readonly MethodInfo ToTypeMethod = typeof(Operand<T>).GetMethod("ToTypeInternal", BindingFlags.NonPublic | BindingFlags.Instance);
		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			return ToTypeMethod.MakeGenericMethod(conversionType).Invoke(this, new object[]{provider});
		}
		
		private object ToTypeInternal<TTo>(IFormatProvider provider)
		{
			return Convert<TTo>.ToChecked(Value);
		}
		
		#endregion
		
		#region IOperand
		
		bool IOperand<Operand<T>>.Equal(Operand<T> other)
		{
			return this == other;
		}
		bool IOperand<Operand<T>>.NotEqual(Operand<T> other)
		{
			return this != other;
		}
		bool IOperand<Operand<T>>.GreaterThan(Operand<T> other)
		{
			return this > other;
		}
		bool IOperand<Operand<T>>.LessThan(Operand<T> other)
		{
			return this < other;
		}
		bool IOperand<Operand<T>>.GreaterThanOrEqual(Operand<T> other)
		{
			return this >= other;
		}
		bool IOperand<Operand<T>>.LessThanOrEqual(Operand<T> other)
		{
			return this <= other;
		}
		Operand<T> IOperand<Operand<T>>.BitwiseAnd(Operand<T> other)
		{
			return this & other;
		}
		Operand<T> IOperand<Operand<T>>.BitwiseOr(Operand<T> other)
		{
			return this | other;
		}
		Operand<T> IOperand<Operand<T>>.Add(Operand<T> other)
		{
			return Addition(this, other);
		}
		Operand<T> IOperand<Operand<T>>.AddChecked(Operand<T> other)
		{
			return AdditionChecked(this, other);
		}
		Operand<T> IOperand<Operand<T>>.Subtract(Operand<T> other)
		{
			return Subtraction(this, other);
		}
		Operand<T> IOperand<Operand<T>>.SubtractChecked(Operand<T> other)
		{
			return SubtractionChecked(this, other);
		}
		Operand<T> IOperand<Operand<T>>.Divide(Operand<T> other)
		{
			return this / other;
		}
		Operand<T> IOperand<Operand<T>>.Modulo(Operand<T> other)
		{
			return this % other;
		}
		Operand<T> IOperand<Operand<T>>.Multiply(Operand<T> other)
		{
			return Multiply(this, other);
		}
		Operand<T> IOperand<Operand<T>>.MultiplyChecked(Operand<T> other)
		{
			return MultiplyChecked(this, other);
		}
		Operand<T> IOperand<Operand<T>>.ExclusiveOr(Operand<T> other)
		{
			return this ^ other;
		}
		Operand<T> IOperand<Operand<T>>.LeftShift(int shift)
		{
			return this << shift;
		}
		Operand<T> IOperand<Operand<T>>.RightShift(int shift)
		{
			return this >> shift;
		}
		Operand<T> IOperand<Operand<T>>.UnaryMinus()
		{
			return UnaryNegation(this);
		}
		Operand<T> IOperand<Operand<T>>.UnaryMinusChecked()
		{
			return UnaryNegationChecked(this);
		}
		Operand<T> IOperand<Operand<T>>.UnaryPlus()
		{
			return +this;
		}
		Operand<T> IOperand<Operand<T>>.Not()
		{
			return !this;
		}
		Operand<T> IOperand<Operand<T>>.OnesComplement()
		{
			return ~this;
		}
		Operand<T> IOperand<Operand<T>>.Increment()
		{
			return ++this;
		}
		Operand<T> IOperand<Operand<T>>.Decrement()
		{
			return --this;
		}
		bool IOperand<Operand<T>>.IsFalse()
		{
			return False(this);
		}
		bool IOperand<Operand<T>>.IsTrue()
		{
			return True(this);
		}
		
		#endregion
	}
}
