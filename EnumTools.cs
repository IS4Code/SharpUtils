/* Date: 28.12.2014, Time: 13:13 */
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using IllidanS4.SharpUtils.Interop;
using IllidanS4.SharpUtils.Reflection;

namespace IllidanS4.SharpUtils
{
	/// <summary>
	/// Use this class to convert between various enum types and their underlying
	/// primitive types.
	/// </summary>
	public sealed class EnumTools : EnumToolsBase<Enum>
	{
		private EnumTools(){}
	}
	
	/// <summary>
	/// The base class for <see cref="EnumTools"/> to limit the argument to <see cref="System.Enum"/>.
	/// Do not use this class directly.
	/// </summary>
	public abstract class EnumToolsBase<TEnumBase> where TEnumBase : class, IComparable, IFormattable
	{
		internal EnumToolsBase(){}
		
		/// <summary>
		/// Converts a primitive value to an enum value.
		/// </summary>
		/// <param name="value">The primitive value to convert.</param>
		/// <returns>The converted enum value.</returns>
		public static unsafe TEnum ToEnum<TEnum, TValue>(TValue value) where TEnum : struct, TEnumBase where TValue : struct, IComparable, IFormattable, IComparable<TValue>, IEquatable<TValue>
		{
			var tr = __makeref(value);
			TypedReferenceTools.ChangeType(&tr, typeof(TEnum));
			return __refvalue(tr, TEnum);
		}
		
		/// <summary>
		/// Converts an enum value to a primitive value.
		/// </summary>
		/// <param name="enm">The enum value to convert.</param>
		/// <returns>The converted primitive value.</returns>
		public static unsafe TValue ToValue<TEnum, TValue>(TEnum enm) where TEnum : struct, TEnumBase where TValue : struct, IComparable, IFormattable, IComparable<TValue>, IEquatable<TValue>
		{
			var tr = __makeref(enm);
			TypedReferenceTools.ChangeType(&tr, typeof(TValue));
			return __refvalue(tr, TValue);
		}
		
		/// <summary>
		/// Obtains the Int64 binary representation of any enum value.
		/// </summary>
		/// <param name="enm">The enum value to convert.</param>
		/// <returns>The binary representation.</returns>
		public static long ToValueBinary<TEnum>(TEnum enm) where TEnum : struct, TEnumBase
		{
			Type underlyingType = Enum.GetUnderlyingType(TypeOf<TEnum>.TypeID);
			unchecked{
				switch(Type.GetTypeCode(underlyingType))
				{
					case TypeCode.SByte:
						return ToValue<TEnum, sbyte>(enm);
					case TypeCode.Byte:
						return ToValue<TEnum, byte>(enm);
					case TypeCode.Int16:
						return ToValue<TEnum, short>(enm);
					case TypeCode.UInt16:
						return ToValue<TEnum, ushort>(enm);
					case TypeCode.Int32:
						return ToValue<TEnum, int>(enm);
					case TypeCode.UInt32:
						return ToValue<TEnum, uint>(enm);
					case TypeCode.Int64:
						return ToValue<TEnum, long>(enm);
					case TypeCode.UInt64:
						return (long)ToValue<TEnum, ulong>(enm);
					default:
						throw new ArgumentException(Extensions.GetResourceString("Arg_MustBeEnumBaseTypeOrEnum"), "value");
				}
			}
		}
		
		/// <summary>
		/// Creates an enum value from its Int64 binary representation.
		/// </summary>
		/// <param name="value">The binary representation.</param>
		/// <returns>The converted enum value.</returns>
		public static TEnum ToEnumBinary<TEnum>(long value) where TEnum : struct, TEnumBase
		{
			Type underlyingType = Enum.GetUnderlyingType(TypeOf<TEnum>.TypeID);
			unchecked{
				switch(Type.GetTypeCode(underlyingType))
				{
					case TypeCode.SByte:
						return ToEnum<TEnum, sbyte>((sbyte)value);
					case TypeCode.Byte:
						return ToEnum<TEnum, byte>((byte)value);
					case TypeCode.Int16:
						return ToEnum<TEnum, short>((short)value);
					case TypeCode.UInt16:
						return ToEnum<TEnum, ushort>((ushort)value);
					case TypeCode.Int32:
						return ToEnum<TEnum, int>((int)value);
					case TypeCode.UInt32:
						return ToEnum<TEnum, uint>((uint)value);
					case TypeCode.Int64:
						return ToEnum<TEnum, long>((long)value);
					case TypeCode.UInt64:
						return ToEnum<TEnum, ulong>((ulong)value);
					default:
						throw new ArgumentException(Extensions.GetResourceString("Arg_MustBeEnumBaseTypeOrEnum"), "value");
				}
			}
		}
		
		private delegate void GetCachedValuesAndNamesDelegate(Type enumType, out ulong[] values, out string[] names, bool getValues, bool getNames);
		private static readonly GetCachedValuesAndNamesDelegate GetCachedValuesAndNames = Hacks.GetInvoker<GetCachedValuesAndNamesDelegate>(typeof(Enum), "GetCachedValuesAndNames", false);
		
		/// <summary>
		/// Converts a string to its respective enum value.
		/// </summary>
		/// <param name="value">The string to parse. Can contain a number or an enum field name.</param>
		/// <returns>The parsed enum value.</returns>
		public static TEnum Parse<TEnum>(string value) where TEnum : struct, TEnumBase
		{
			return Parse<TEnum>(value, false);
		}
		
		/// <summary>
		/// Converts a string to its respective enum value.
		/// </summary>
		/// <param name="value">The string to parse. Can contain a number or an enum field name.</param>
		/// <param name="ignoreCase">Whether to ignore case when parsing the enum name.</param>
		/// <returns>The parsed enum value.</returns>
		public static TEnum Parse<TEnum>(string value, bool ignoreCase) where TEnum : struct, TEnumBase
		{
			TEnum result;
			Exception exc = TryParseEnum<TEnum>(value, ignoreCase, true, out result);
			if(exc != null) throw exc;
			return result;
		}
		
		/// <summary>
		/// Attempts to convert a string to its respective enum value.
		/// </summary>
		/// <param name="value">The string to parse. Can contain a number or an enum field name.</param>
		/// <param name="result">The parsed enum value.</param>
		/// <returns>True if the conversion was successful, false otherwise.</returns>
		public static bool TryParse<TEnum>(string value, out TEnum result) where TEnum : struct, TEnumBase
		{
			return TryParse<TEnum>(value, false, out result);
		}
		
		/// <summary>
		/// Attempts to convert a string to its respective enum value.
		/// </summary>
		/// <param name="value">The string to parse. Can contain a number or an enum field name.</param>
		/// <param name="ignoreCase">Whether to ignore case when parsing the enum name.</param>
		/// <param name="result">The parsed enum value.</param>
		/// <returns>True if the conversion was successful, false otherwise.</returns>
		public static bool TryParse<TEnum>(string value, bool ignoreCase, out TEnum result) where TEnum : struct, TEnumBase
		{
			return TryParseEnum<TEnum>(value, ignoreCase, false, out result) == null;
		}
		
		private static Exception TryParseEnum<TEnum>(string value, bool ignoreCase, bool canThrow, out TEnum result) where TEnum : struct, TEnumBase
		{
			//basically rewrite of Enum.TryParseEnum
			if(value == null)
			{
				result = default(TEnum);
				return new ArgumentNullException("value");
			}
			value = value.Trim();
			if(value.Length == 0)
			{
				result = default(TEnum);
				return new ArgumentException(Extensions.GetResourceString("Arg_MustContainEnumInfo"));
			}
			
			Type enumType = TypeOf<TEnum>.TypeID;
			if(char.IsDigit(value[0]) || value[0] == '-' || value[0] == '+')
			{
				Type underlyingType = Enum.GetUnderlyingType(enumType);
				try
				{
					long value2 = Int64.Parse(value, CultureInfo.InvariantCulture);
					result = ToEnumBinary<TEnum>(value2);
					return null;
				}
				catch (FormatException)
				{
					
				}
				catch (Exception failure)
				{
					if(canThrow)
					{
						throw;
					}
					result = default(TEnum);
					return failure;
				}
			}
			ulong num = 0L;
			string[] array = value.Split(',');
			ulong[] array2;
			string[] array3;
			GetCachedValuesAndNames(enumType, out array2, out array3, true, true);
			for(int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Trim();
				bool flag = false;
				int j = 0;
				while (j < array3.Length)
				{
					if (ignoreCase)
					{
						if (string.Compare(array3[j], array[i], StringComparison.OrdinalIgnoreCase) == 0)
						{
							goto IL_152;
						}
					}
					else
					{
						if (array3[j].Equals(array[i]))
						{
							goto IL_152;
						}
					}
					j++;
					continue;
					IL_152:
					ulong num2 = array2[j];
					num |= num2;
					flag = true;
					break;
				}
				if (!flag)
				{
					result = default(TEnum);
					return new ArgumentException(Extensions.GetResourceString("Arg_EnumValueNotFound"), "value");
				}
			}
			try
			{
				result = ToEnumBinary<TEnum>(unchecked((long)num));
				return null;
			}
			catch(Exception failure2)
			{
				if(canThrow)
				{
					throw;
				}else{
					result = default(TEnum);
					return failure2;
				}
			}
		}
	}
}
