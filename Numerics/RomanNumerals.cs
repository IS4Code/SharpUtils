using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace IllidanS4.SharpUtils.Numerics
{
	public class RomanNumerals : IFormatProvider, ICustomFormatter
	{
		public static readonly RomanNumerals Default = new RomanNumerals();
		
		public object GetFormat(Type t)
		{
			if(t == TypeOf<ICustomFormatter>.TypeID || t == TypeOf<RomanNumerals>.TypeID)
			{
				return this;
			}
			return null;
		}
		
		public string Format(string format, object arg, IFormatProvider formatProvider)
		{
			switch(Type.GetTypeCode(arg.GetType()))
			{
				case TypeCode.Byte:
					return Format((byte)arg);
				case TypeCode.Int16:
					return Format((short)arg);
				case TypeCode.Int32:
					return Format((int)arg);
				case TypeCode.Int64:
					return Format((long)arg);
				case TypeCode.SByte:
					return Format(Convert.ToInt16(arg));
				case TypeCode.UInt16:
					return Format(Convert.ToInt32(arg));
				case TypeCode.UInt32:
					return Format(Convert.ToInt64(arg));
			}
			return null;
		}
		
		static SymbolStoreDict orders = new SymbolStoreDict
		{
			{1000, 'M'},
			{500, 'D'},
			{100, 'C'},
			{50, 'L'},
			{10, 'X'},
			{5, 'V'},
			{1, 'I'}
		};
		
		public int Parse(string str)
		{
			int sum = 0;
			for(int i = 0; i < str.Length; i++)
			{
				char ch = str[i];
				if(orders.Contains(ch))
				{
					if(i+1 < str.Length)
					{
						char nextch = str[i+1];
						int cur = orders[ch];
						int next = orders[nextch];
						if(cur < next)
						{
							sum += next-cur;
							i++;
							continue;
						}
					}
					
					sum += orders[ch];
				}else{
					throw new FormatException();
				}
			}
			
			return sum;
		}
		
		public static string Format(int i)
		{
			if(i <= 0 || i >= 4000)throw new ArgumentOutOfRangeException("i", "Argument should be in range 1 - 3999");
			StringBuilder builder = new StringBuilder();
			
			foreach(int order in OrderIterator())
			{
				CheckDivide(builder, ref i, order);
				foreach(int suborder in OrderIterator(order))
				{
					if(CheckSubstract(builder, ref i, suborder, order)) break;
				}
			}
			
			return builder.ToString();
		}
		
		public static string Format(short i)
		{
			if(i <= 0 || i >= 4000)throw new ArgumentOutOfRangeException("i", "Argument should be in range 1 - 3999");
			StringBuilder builder = new StringBuilder();
			foreach(int order in OrderIterator())
			{
				CheckDivide(builder, ref i, order);
				foreach(int suborder in OrderIterator(order))
				{
					if(CheckSubstract(builder, ref i, suborder, order)) break;
				}
			}
			return builder.ToString();
		}
		
		public static string Format(long i)
		{
			if(i <= 0 || i >= 4000)throw new ArgumentOutOfRangeException("i", "Argument should be in range 1 - 3999");
			StringBuilder builder = new StringBuilder();
			foreach(int order in OrderIterator())
			{
				CheckDivide(builder, ref i, order);
				foreach(int suborder in OrderIterator(order))
				{
					if(CheckSubstract(builder, ref i, suborder, order)) break;
				}
			}
			return builder.ToString();
		}
		
		public static string Format(byte i)
		{
			StringBuilder builder = new StringBuilder();
			foreach(int order in OrderIterator())
			{
				CheckDivide(builder, ref i, order);
				foreach(int suborder in OrderIterator(order))
				{
					if(CheckSubstract(builder, ref i, suborder, order)) break;
				}
			}
			return builder.ToString();
		}
		
		private static void CheckDivide(StringBuilder builder, ref int i, int div)
		{
			builder.Append(orders[div], i/div);
			i = i%div;
		}
		
		private static bool CheckSubstract(StringBuilder builder, ref int i, int mem, int div)
		{
			if(i+mem >= div)
			{
				builder.Append(orders[mem]);
				builder.Append(orders[div]);
				i = (i+mem)%div;
				return true;
			}
			return false;
		}
		
		private static void CheckDivide(StringBuilder builder, ref short i, int div)
		{
			builder.Append(orders[div], i/div);
			i = Convert.ToInt16(i%div);
		}
		
		private static bool CheckSubstract(StringBuilder builder, ref short i, int mem, int div)
		{
			if(i+mem >= div)
			{
				builder.Append(orders[mem]);
				builder.Append(orders[div]);
				i = Convert.ToInt16((i+mem)%div);
				return true;
			}
			return false;
		}
		
		private static void CheckDivide(StringBuilder builder, ref byte i, int div)
		{
			builder.Append(orders[div], i/div);
			i = Convert.ToByte(i%div);
		}
		
		private static bool CheckSubstract(StringBuilder builder, ref byte i, int mem, int div)
		{
			if(i+mem >= div)
			{
				builder.Append(orders[mem]);
				builder.Append(orders[div]);
				i = Convert.ToByte((i+mem)%div);
				return true;
			}
			return false;
		}
		
		private static void CheckDivide(StringBuilder builder, ref long i, int div)
		{
			builder.Append(orders[div], Convert.ToInt32(i/div));
			i = i%div;
		}
		
		private static bool CheckSubstract(StringBuilder builder, ref long i, int mem, int div)
		{
			if(i+mem >= div)
			{
				builder.Append(orders[mem]);
				builder.Append(orders[div]);
				i = (i+mem)%div;
				return true;
			}
			return false;
		}
		
		private static IEnumerable<int> OrderIterator()
		{
			yield return 1000;
			yield return 500;
			yield return 100;
			yield return 50;
			yield return 10;
			yield return 5;
			yield return 1;
		}
		
		private static IEnumerable<int> OrderIterator(int current)
		{
			switch(current)
			{
				case 1000: case 500:
					yield return 1;
					yield return 5;
					yield return 10;
					yield return 50;
					yield return 100;
					break;
				case 100: case 50:
					yield return 1;
					yield return 5;
					yield return 10;
					break;
				case 10: case 5:
					yield return 1;
					break;
			}
		}
		
		private class SymbolStoreDict : IEnumerable<KeyValuePair<int,char>>, IEnumerable<KeyValuePair<char,int>>
		{
			List<KeyValuePair<int,char>> values;
			
			public SymbolStoreDict()
			{
				values = new List<KeyValuePair<int,char>>();
			}
			
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
			
			public IEnumerator<KeyValuePair<int,char>> GetEnumerator()
			{
				return values.GetEnumerator();
			}
			
			IEnumerator<KeyValuePair<char,int>> IEnumerable<KeyValuePair<char,int>>.GetEnumerator()
			{
				foreach(var pair in values)
				{
					yield return new KeyValuePair<char,int>(pair.Value, pair.Key);
				}
			}
			
			public void Add(KeyValuePair<int,char> item)
			{
				values.Add(item);
			}
			
			public void Add(KeyValuePair<char,int> item)
			{
				values.Add(new KeyValuePair<int,char>(item.Value, item.Key));
			}
			
			public void Add(int key, char value)
			{
				Add(new KeyValuePair<int,char>(key, value));
			}
			
			public void Add(char key, int value)
			{
				Add(value, key);
			}
			
			public bool Contains(int i)
			{
				foreach(var p in values)
				{
					if(p.Key == i) return true;
				}
				return false;
			}
			
			public bool Contains(char ch)
			{
				foreach(var p in values)
				{
					if(p.Value == ch) return true;
				}
				return false;
			}
			
			public char this[int key]
			{
				get{
					foreach(var pair in values)
					{
						if(pair.Key == key)
						{
							return pair.Value;
						}
					}
					return '\0';
				}
			}
			
			public int this[char key]
			{
				get{
					foreach(var pair in values)
					{
						if(pair.Value == key)
						{
							return pair.Key;
						}
					}
					return 0;
				}
			}
		}
	}
}