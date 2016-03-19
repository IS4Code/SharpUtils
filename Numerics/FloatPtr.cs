/* Date: 1.9.2014, Time: 12:22 */
using System;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Numerics
{
	/// <summary>
	/// Native-size floating point number. Note that this shouldn't increase the computational speed at all,
	/// since the CLR uses its own float type (F) to do all computations, and then convert the result to Single or Double.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct FloatPtr : IEquatable<FloatPtr>, IConvertible
	{
		public static readonly FloatPtr Zero = default(FloatPtr);
		
		[FieldOffset(0)]
		readonly IntPtr value;
		
		public IntPtr MemoryValue{
			get{
				return value;
			}
		}
		
		/// <summary>
		/// Always check for IntPtr size.
		/// </summary>
		private float SingleValue{
			get{
				return IntPtrToSingle(value);
			}
		}
		
		/// <summary>
		/// Always check for IntPtr size.
		/// </summary>
		private double DoubleValue{
			get{
				return IntPtrToDouble(value);
			}
		}
		
		public object Value{
			get{
				switch(IntPtr.Size)
				{
					case 4:
						return SingleValue;
					case 8:
						return DoubleValue;
					default:
						throw GetError();
				}
			}
		}
		
		public static int Size{
			get{
				return IntPtr.Size;
			}
		}
		
		public static TypeCode InterpretType{
			get{
				switch(IntPtr.Size)
				{
					case 4:
						return TypeCode.Single;
					case 8:
						return TypeCode.Double;
					default:
						throw GetError();
				}
			}
		}
		
		private FloatPtr(IntPtr memoryValue)
		{
			value = memoryValue;
		}
		
		public FloatPtr(double value) : this()
		{
			switch(IntPtr.Size)
			{
				case 4:
					this = new FloatPtr((float)value);
					break;
				case 8:
					this = new FloatPtr(DoubleToIntPtr(value));
					break;
				default:
					throw GetError();
			}
		}
		
		public FloatPtr(float value) : this()
		{
			switch(IntPtr.Size)
			{
				case 4:
					this = new FloatPtr(SingleToIntPtr(value));
					break;
				case 8:
					this = new FloatPtr((double)value);
					break;
				default:
					throw GetError();
			}
		}
		
		public override string ToString()
		{
			switch(IntPtr.Size)
			{
				case 4:
					return SingleValue.ToString();
				case 8:
					return DoubleValue.ToString();
				default:
					throw GetError();
			}
		}
		
		public static FloatPtr Parse(string s)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return Single.Parse(s);
				case 8:
					return Double.Parse(s);
				default:
					throw GetError();
			}
		}
		
		#region Operations
		#region Arithmetic
		public static FloatPtr operator +(FloatPtr a, FloatPtr b)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return new FloatPtr(a.SingleValue+b.SingleValue);
				case 8:
					return new FloatPtr(a.DoubleValue+b.DoubleValue);
				default:
					throw GetError();
			}
		}
		
		public static FloatPtr operator ++(FloatPtr a)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return new FloatPtr(a.SingleValue+1f);
				case 8:
					return new FloatPtr(a.DoubleValue+1d);
				default:
					throw GetError();
			}
		}
		
		public static FloatPtr operator --(FloatPtr a)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return new FloatPtr(a.SingleValue-1f);
				case 8:
					return new FloatPtr(a.DoubleValue-1d);
				default:
					throw GetError();
			}
		}
		
		public static FloatPtr operator -(FloatPtr a, FloatPtr b)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return new FloatPtr(a.SingleValue-b.SingleValue);
				case 8:
					return new FloatPtr(a.DoubleValue-b.DoubleValue);
				default:
					throw GetError();
			}
		}
		
		public static FloatPtr operator *(FloatPtr a, FloatPtr b)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return new FloatPtr(a.SingleValue*b.SingleValue);
				case 8:
					return new FloatPtr(a.DoubleValue*b.DoubleValue);
				default:
					throw GetError();
			}
		}
		
		public static FloatPtr operator /(FloatPtr a, FloatPtr b)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return new FloatPtr(a.SingleValue/b.SingleValue);
				case 8:
					return new FloatPtr(a.DoubleValue/b.DoubleValue);
				default:
					throw GetError();
			}
		}
		#endregion
		
		#region Relations
		public static bool operator ==(FloatPtr lhs, FloatPtr rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(FloatPtr lhs, FloatPtr rhs)
		{
			return !(lhs == rhs);
		}
		
		public static bool operator ==(FloatPtr a, double b)
		{
			return (double)a==b;
		}
		
		public static bool operator !=(FloatPtr a, double b)
		{
			return (double)a!=b;
		}
		
		public static bool operator ==(FloatPtr a, float b)
		{
			return a==(FloatPtr)b;
		}
		
		public static bool operator !=(FloatPtr a, float b)
		{
			return a!=(FloatPtr)b;
		}
		#endregion
		
		#region Explicit
		public static explicit operator double(FloatPtr val)
		{
			return ((IConvertible)val).ToDouble(null);
		}
		
		public static explicit operator float(FloatPtr val)
		{
			return ((IConvertible)val).ToSingle(null);
		}
		
		public static explicit operator byte(FloatPtr val)
		{
			return ((IConvertible)val).ToByte(null);
		}
		
		[CLSCompliant(false)]
		public static explicit operator sbyte(FloatPtr val)
		{
			return ((IConvertible)val).ToSByte(null);
		}
		
		public static explicit operator short(FloatPtr val)
		{
			return ((IConvertible)val).ToInt16(null);
		}
		
		[CLSCompliant(false)]
		public static explicit operator ushort(FloatPtr val)
		{
			return ((IConvertible)val).ToUInt16(null);
		}
		
		public static explicit operator int(FloatPtr val)
		{
			return ((IConvertible)val).ToInt32(null);
		}
		
		[CLSCompliant(false)]
		public static explicit operator uint(FloatPtr val)
		{
			return ((IConvertible)val).ToUInt32(null);
		}
		
		public static explicit operator long(FloatPtr val)
		{
			return ((IConvertible)val).ToInt64(null);
		}
		
		[CLSCompliant(false)]
		public static explicit operator ulong(FloatPtr val)
		{
			return ((IConvertible)val).ToUInt64(null);
		}
		
		public static explicit operator decimal(FloatPtr val)
		{
			return ((IConvertible)val).ToDecimal(null);
		}
		#endregion
		
		#region Implicit
		public static implicit operator FloatPtr(byte val)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return new FloatPtr((float)val);
				case 8:
					return new FloatPtr((double)val);
				default:
					throw GetError();
			}
		}
		
		[CLSCompliant(false)]
		public static implicit operator FloatPtr(sbyte val)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return new FloatPtr((float)val);
				case 8:
					return new FloatPtr((double)val);
				default:
					throw GetError();
			}
		}
		
		public static implicit operator FloatPtr(short val)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return new FloatPtr((float)val);
				case 8:
					return new FloatPtr((double)val);
				default:
					throw GetError();
			}
		}
		
		[CLSCompliant(false)]
		public static implicit operator FloatPtr(ushort val)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return new FloatPtr((float)val);
				case 8:
					return new FloatPtr((double)val);
				default:
					throw GetError();
			}
		}
		
		public static implicit operator FloatPtr(int val)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return new FloatPtr((float)val);
				case 8:
					return new FloatPtr((double)val);
				default:
					throw GetError();
			}
		}
		
		[CLSCompliant(false)]
		public static implicit operator FloatPtr(uint val)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return new FloatPtr((float)val);
				case 8:
					return new FloatPtr((double)val);
				default:
					throw GetError();
			}
		}
		
		public static implicit operator FloatPtr(long val)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return new FloatPtr((float)val);
				case 8:
					return new FloatPtr((double)val);
				default:
					throw GetError();
			}
		}
		
		[CLSCompliant(false)]
		public static implicit operator FloatPtr(ulong val)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return new FloatPtr((float)val);
				case 8:
					return new FloatPtr((double)val);
				default:
					throw GetError();
			}
		}
		
		public static implicit operator FloatPtr(decimal val)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return new FloatPtr((float)val);
				case 8:
					return new FloatPtr((double)val);
				default:
					throw GetError();
			}
		}
		
		public static implicit operator FloatPtr(float val)
		{
			return new FloatPtr(val);
		}
		
		public static implicit operator FloatPtr(double val)
		{
			return new FloatPtr(val);
		}
		#endregion
		
		#region Memory
		public static IntPtr FloatPtrToIntPtrBits(FloatPtr f)
		{
			return f.MemoryValue;
		}
		
		public static FloatPtr IntPtrBitsToFloatPtr(IntPtr ptr)
		{
			return new FloatPtr(ptr);
		}
		#endregion
		#endregion
		
		#region Properties
		public static FloatPtr PositiveInfinity{
			get{
				switch(IntPtr.Size)
				{
					case 4:
						return SingleToFloatPtr(Single.PositiveInfinity);
					case 8:
						return DoubleToFloatPtr(Double.PositiveInfinity);
					default:
						throw GetError();
				}
			}
		}
		
		public static FloatPtr NegativeInfinity{
			get{
				switch(IntPtr.Size)
				{
					case 4:
						return SingleToFloatPtr(Single.NegativeInfinity);
					case 8:
						return DoubleToFloatPtr(Double.NegativeInfinity);
					default:
						throw GetError();
				}
			}
		}
		
		public static FloatPtr NaN{
			get{
				switch(IntPtr.Size)
				{
					case 4:
						return SingleToFloatPtr(Single.NaN);
					case 8:
						return DoubleToFloatPtr(Double.NaN);
					default:
						throw GetError();
				}
			}
		}
		
		public static FloatPtr Epsilon{
			get{
				switch(IntPtr.Size)
				{
					case 4:
						return SingleToFloatPtr(Single.Epsilon);
					case 8:
						return DoubleToFloatPtr(Double.Epsilon);
					default:
						throw GetError();
				}
			}
		}
		
		public static FloatPtr MinValue{
			get{
				switch(IntPtr.Size)
				{
					case 4:
						return SingleToFloatPtr(Single.MinValue);
					case 8:
						return DoubleToFloatPtr(Double.MinValue);
					default:
						throw GetError();
				}
			}
		}
		
		public static FloatPtr MaxValue{
			get{
				switch(IntPtr.Size)
				{
					case 4:
						return SingleToFloatPtr(Single.MaxValue);
					case 8:
						return DoubleToFloatPtr(Double.MaxValue);
					default:
						throw GetError();
				}
			}
		}
		
		public static bool IsPositiveInfinity(FloatPtr f)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return Single.IsPositiveInfinity(f.SingleValue);
				case 8:
					return Double.IsPositiveInfinity(f.DoubleValue);
				default:
					throw GetError();
			}
		}
		
		public static bool IsNegativeInfinity(FloatPtr f)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return Single.IsNegativeInfinity(f.SingleValue);
				case 8:
					return Double.IsNegativeInfinity(f.DoubleValue);
				default:
					throw GetError();
			}
		}
		public static bool IsInfinity(FloatPtr f)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return Single.IsInfinity(f.SingleValue);
				case 8:
					return Double.IsInfinity(f.DoubleValue);
				default:
					throw GetError();
			}
		}
		public static bool IsNaN(FloatPtr f)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return Single.IsNaN(f.SingleValue);
				case 8:
					return Double.IsNaN(f.DoubleValue);
				default:
					throw GetError();
			}
		}
		#endregion
		
		#region Helpers
		private static InvalidOperationException GetError()
		{
			return new InvalidOperationException("Unknown native integer size.");
		}
		
		private unsafe static float IntPtrToSingle(IntPtr ptr)
		{
			return *(float*)(&ptr);
		}
		
		private unsafe static double IntPtrToDouble(IntPtr ptr)
		{
			return *(double*)(&ptr);
		}
		
		private unsafe static IntPtr DoubleToIntPtr(double val)
		{
			return *(IntPtr*)(&val);
		}
		
		private unsafe static IntPtr SingleToIntPtr(float val)
		{
			return *(IntPtr*)(&val);
		}
		
		/// <summary>
		/// Always check for IntPtr size.
		/// </summary>
		private unsafe static FloatPtr SingleToFloatPtr(float val)
		{
			return *(FloatPtr*)(&val);
		}
		
		/// <summary>
		/// Always check for IntPtr size.
		/// </summary>
		private unsafe static FloatPtr DoubleToFloatPtr(double val)
		{
			return *(FloatPtr*)(&val);
		}
		#endregion
		
		#region IConvertible
		object IConvertible.ToType(Type type, IFormatProvider provider)
		{
			return Convert.ChangeType(Value, type);
		}
		
		int IConvertible.ToInt32(IFormatProvider provider)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return (int)SingleValue;
				case 8:
					return (int)DoubleValue;
				default:
					throw GetError();
			}
		}
		
		string IConvertible.ToString(IFormatProvider provider)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return SingleValue.ToString(provider);
				case 8:
					return DoubleValue.ToString(provider);
				default:
					throw GetError();
			}
		}
		
		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return Convert.ToDateTime(Value);
		}
		
		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return (decimal)SingleValue;
				case 8:
					return (decimal)DoubleValue;
				default:
					throw GetError();
			}
		}
		
		double IConvertible.ToDouble(IFormatProvider provider)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return (double)SingleValue;
				case 8:
					return DoubleValue;
				default:
					throw GetError();
			}
		}
		
		float IConvertible.ToSingle(IFormatProvider provider)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return SingleValue;
				case 8:
					return (float)DoubleValue;
				default:
					throw GetError();
			}
		}
		
		long IConvertible.ToInt64(IFormatProvider provider)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return (long)SingleValue;
				case 8:
					return (long)DoubleValue;
				default:
					throw GetError();
			}
		}
		
		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return (ulong)SingleValue;
				case 8:
					return (ulong)DoubleValue;
				default:
					throw GetError();
			}
		}
		
		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return (uint)SingleValue;
				case 8:
					return (uint)DoubleValue;
				default:
					throw GetError();
			}
		}
		
		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return (ushort)SingleValue;
				case 8:
					return (ushort)DoubleValue;
				default:
					throw GetError();
			}
		}
		
		short IConvertible.ToInt16(IFormatProvider provider)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return (short)SingleValue;
				case 8:
					return (short)DoubleValue;
				default:
					throw GetError();
			}
		}
		
		byte IConvertible.ToByte(IFormatProvider provider)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return (byte)SingleValue;
				case 8:
					return (byte)DoubleValue;
				default:
					throw GetError();
			}
		}
		
		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			switch(IntPtr.Size)
			{
				case 4:
					return (sbyte)SingleValue;
				case 8:
					return (sbyte)DoubleValue;
				default:
					throw GetError();
			}
		}
		
		char IConvertible.ToChar(IFormatProvider provider)
		{
			return Convert.ToChar(Value);
		}
		
		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(Value);
		}
		
		TypeCode IConvertible.GetTypeCode()
		{
			return InterpretType;
		}
		#endregion
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return (obj is FloatPtr) && Equals((FloatPtr)obj);
		}
		
		public bool Equals(FloatPtr other)
		{
			return this.value == other.value;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * value.GetHashCode();
			}
			return hashCode;
		}
		#endregion
	}
}
