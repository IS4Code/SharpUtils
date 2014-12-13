using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Numerics
{
	[Serializable]
	public struct Number : IFormattable, IComparable, 
	IConvertible, IComparable<Number>, IEquatable<Number>
	{
		private dynamic value;
		
		public dynamic Value{
			get{
				return (value??(value=0));
			}
			private set{
				this.value = value??0;
			}
		}
		
		public Type NumberType{
			get{
				return Value.GetType();
			}
		}
		
		public bool IsWhole{
			get{
				return (this % 1).IsZero;
			}
		}
		
		public bool IsDecimal{
			get{
				return !(this % 1).IsZero;
			}
		}
		
		public bool IsNatural{
			get{
				return this.IsWhole && this.Value > (dynamic)Activator.CreateInstance(this.NumberType);
			}
		}
		
		public bool IsZero{
			get{
				return this == (dynamic)Activator.CreateInstance(this.NumberType);
			}
		}
		
		public bool IsComplex{
			get{
				return value is Complex || this.IsRational;
			}
		}
		
		public bool IsRational{
			get{
				return
					value is byte ||
					value is short ||
					value is int ||
					value is long ||
					value is sbyte ||
					value is ushort ||
					value is uint ||
					value is ulong ||
					value is float ||
					value is double ||
					value is decimal ||
					value is BigInteger;
			}
		}
		
		public Number(dynamic value)
		{
			this.value = value??0;
		}
		
		public Number(Type type)
		{
			this.value = Activator.CreateInstance(type);
		}
		
		public static Number Sqrt(Number arg)
		{
			double asDouble = (double)arg.Value;
			if(asDouble >= 0)
			{
				return new Number(Math.Sqrt(asDouble));
			}else{
				return new Number(Complex.Sqrt(new Complex(asDouble, 0)));
			}
		}
		
		public static Number operator +(Number larg, Number rarg)
		{
			ConvertEqual(ref larg, ref rarg);
			return new Number(larg.Value + rarg.Value);
		}
		
		public static Number operator -(Number larg, Number rarg)
		{
			ConvertEqual(ref larg, ref rarg);
			return new Number(larg.Value - rarg.Value);
		}
		
		public static Number operator *(Number larg, Number rarg)
		{
			ConvertEqual(ref larg, ref rarg);
			return new Number(larg.Value * rarg.Value);
		}
		
		public static Number operator /(Number larg, Number rarg)
		{
			ConvertEqual(ref larg, ref rarg);
			return new Number(larg.Value / rarg.Value);
		}
		
		public static bool operator ==(Number larg, Number rarg)
		{
			ConvertEqual(ref larg, ref rarg);
			return larg.Value == rarg.Value;
		}
		
		public static bool operator !=(Number larg, Number rarg)
		{
			ConvertEqual(ref larg, ref rarg);
			return larg.Value != rarg.Value;
		}
		
		public static bool operator <(Number larg, Number rarg)
		{
			ConvertEqual(ref larg, ref rarg);
			return larg.Value < rarg.Value;
		}
		
		public static bool operator >(Number larg, Number rarg)
		{
			ConvertEqual(ref larg, ref rarg);
			return larg.Value > rarg.Value;
		}
		
		public static bool operator <=(Number larg, Number rarg)
		{
			ConvertEqual(ref larg, ref rarg);
			return larg.Value <= rarg.Value;
		}
		
		public static bool operator >=(Number larg, Number rarg)
		{
			ConvertEqual(ref larg, ref rarg);
			return larg.Value >= rarg.Value;
		}
		
		public static Number operator %(Number larg, Number rarg)
		{
			ConvertEqual(ref larg, ref rarg);
			return larg.Value % rarg.Value;
		}
		
		public static implicit operator Number(byte value)
		{
			return new Number(value);
		}
		public static implicit operator Number(sbyte value)
		{
			return new Number(value);
		}
		public static implicit operator Number(short value)
		{
			return new Number(value);
		}
		public static implicit operator Number(ushort value)
		{
			return new Number(value);
		}
		public static implicit operator Number(int value)
		{
			return new Number(value);
		}
		public static implicit operator Number(uint value)
		{
			return new Number(value);
		}
		public static implicit operator Number(long value)
		{
			return new Number(value);
		}
		public static implicit operator Number(ulong value)
		{
			return new Number(value);
		}
		public static implicit operator Number(float value)
		{
			return new Number(value);
		}
		public static implicit operator Number(double value)
		{
			return new Number(value);
		}
		public static implicit operator Number(decimal value)
		{
			return new Number(value);
		}
		public static implicit operator Number(BigInteger value)
		{
			return new Number(value);
		}
		public static implicit operator Number(Complex value)
		{
			return new Number(value);
		}
		
		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
		
		#region Comparison
		
		public override bool Equals(object obj)
		{
			if(obj is Number)
			{
				return this == (Number)obj;
			}
			return false;
		}
		
		public bool Equals(Number other)
		{
			return this == other;
		}
		
		public int CompareTo(Number other)
		{
			if(this == other)
			{
				return 0;
			}else if(this < other)
			{
				return -1;
			}else if(this > other)
			{
				return 1;
			}
			throw new InvalidOperationException();
		}
		
		public int CompareTo(object obj)
		{
			if(obj is Number)
			{
				return CompareTo((Number)obj);
			}
			throw new InvalidOperationException();
		}
		
		#endregion
		
		#region Conversion
		
		private static void ConvertEqual(ref Number larg, ref Number rarg)
		{
			if(larg.Value is float)
			{
				if(rarg.Value is decimal)
				{
					if(larg.Value >= (float)Decimal.MaxValue)
					{
						rarg.Value = (float)rarg.Value;
					}else{
						larg.Value = (decimal)larg.Value;
					}
				}
			}else if(larg.Value is double)
			{
				if(rarg.Value is decimal)
				{
					if(larg.Value >= (double)Decimal.MaxValue)
					{
						rarg.Value = (double)rarg.Value;
					}else{
						larg.Value = (decimal)larg.Value;
					}
				}
			}else if(larg.Value is decimal)
			{
				if(rarg.Value is float)
				{
					if(rarg.Value >= (float)Decimal.MaxValue)
					{
						larg.Value = (float)larg.Value;
					}else{
						rarg.Value = (decimal)rarg.Value;
					}
				}else if(rarg.Value is double)
				{
					if(rarg.Value >= (double)Decimal.MaxValue)
					{
						larg.Value = (double)larg.Value;
					}else{
						rarg.Value = (decimal)rarg.Value;
					}
				}
			}
		}
		
		public byte[] ToByteArray()
		{
			if(Value is BigInteger)
			{
				return Value.ToByteArray();
			}
			try{
				return BitConverter.GetBytes(Value);
			}catch{
				int size = Marshal.SizeOf(NumberType);
				IntPtr dest = Marshal.AllocHGlobal(size);
				Marshal.StructureToPtr(Value, dest, true);
				byte[] bytes = new byte[size];
				Marshal.Copy(dest, bytes, 0, size);
				return bytes;
			}
			throw new InvalidOperationException();
		}
		
		public object ToType(Type type, IFormatProvider provider)
		{
			return Value.ToType(type, provider);
		}
		
		public string ToString(IFormatProvider provider)
		{
			return Value.ToString(provider);
		}
		
		public string ToString(string str, IFormatProvider provider)
		{
			return Value.ToString(str, provider);
		}
		
		public decimal ToDecimal(IFormatProvider provider)
		{
			return Value.ToDecimal(provider);
		}
		
		public double ToDouble(IFormatProvider provider)
		{
			return Value.ToDouble(provider);
		}
		
		public float ToSingle(IFormatProvider provider)
		{
			return Value.ToSingle(provider);
		}
		
		public ulong ToUInt64(IFormatProvider provider)
		{
			return Value.ToUInt64(provider);
		}
		
		public long ToInt64(IFormatProvider provider)
		{
			return Value.ToInt64(provider);
		}
		
		public uint ToUInt32(IFormatProvider provider)
		{
			return Value.ToUInt32(provider);
		}
		
		public int ToInt32(IFormatProvider provider)
		{
			return Value.ToInt32(provider);
		}
		
		public ushort ToUInt16(IFormatProvider provider)
		{
			return Value.ToUInt16(provider);
		}
		
		public short ToInt16(IFormatProvider provider)
		{
			return Value.ToInt16(provider);
		}
		
		public byte ToByte(IFormatProvider provider)
		{
			return Value.ToByte(provider);
		}
		
		public sbyte ToSByte(IFormatProvider provider)
		{
			return Value.ToSByte(provider);
		}
		
		public char ToChar(IFormatProvider provider)
		{
			return Value.ToChar(provider);
		}
		
		public bool ToBoolean(IFormatProvider provider)
		{
			return Value.ToBoolean(provider);
		}
		
		public DateTime ToDateTime(IFormatProvider provider)
		{
			return Value.ToDateTime(provider);
		}
		
		#endregion
		
		public TypeCode GetTypeCode()
		{
			return Type.GetTypeCode(Value.GetType());
		}
	}
}