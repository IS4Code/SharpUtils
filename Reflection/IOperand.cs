/* Date: 16.4.2015, Time: 15:46 */
using System;

namespace IllidanS4.SharpUtils.Reflection
{
	public interface IOperand<TOperand> where TOperand : IOperand<TOperand>
	{
		bool Equal(TOperand other);
		bool NotEqual(TOperand other);
		bool GreaterThan(TOperand other);
		bool LessThan(TOperand other);
		bool GreaterThanOrEqual(TOperand other);
		bool LessThanOrEqual(TOperand other);
		TOperand BitwiseAnd(TOperand other);
		TOperand BitwiseOr(TOperand other);
		TOperand Add(TOperand other);
		TOperand AddChecked(TOperand other);
		TOperand Subtract(TOperand other);
		TOperand SubtractChecked(TOperand other);
		TOperand Divide(TOperand other);
		TOperand Modulo(TOperand other);
		TOperand Multiply(TOperand other);
		TOperand MultiplyChecked(TOperand other);
		TOperand ExclusiveOr(TOperand other);
		TOperand LeftShift(int shift);
		TOperand RightShift(int shift);
		TOperand UnaryMinus();
		TOperand UnaryMinusChecked();
		TOperand UnaryPlus();
		TOperand Not();
		TOperand OnesComplement();
		TOperand Increment();
		TOperand Decrement();
		bool IsFalse();
		bool IsTrue();
	}
}
