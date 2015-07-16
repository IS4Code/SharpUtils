/* Date: 16.7.2015, Time: 1:08 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IllidanS4.SharpUtils.Accessing;

namespace IllidanS4.SharpUtils.Text
{
	public struct StringChunk : IComparable, ICloneable, IConvertible, IComparable<string>, IEnumerable<char>, IEnumerable, IEquatable<string>, IEquatable<StringChunk>, IComparable<StringChunk>, IIndexGet<int, char>
	{
		readonly string source;
		readonly int offset;
		readonly int length;
		
		public StringChunk(string value) : this()
		{
			if(value == null) throw new ArgumentNullException("value");
			source = value;
			length = value.Length;
		}
		
		public StringChunk(char[] value) : this(new string(value))
		{
			
		}
		
		private StringChunk(string source, int offset, int length)
		{
			this.source = source;
			this.offset = offset;
			this.length = length;
		}
		
		public override string ToString()
		{
			if(source == null) return "";
			return source.Substring(offset, length);
		}
		
		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
		
		#region Implementation
		
		public char this[int index]{
			get{
				return source[offset+index];
			}
		}
		
		public int Length{
			get{
				return length;
			}
		}
		
		public StringChunk Substring(int startIndex)
		{
			return Substring(startIndex, length-startIndex);
		}
		
		public StringChunk Substring(int startIndex, int length)
		{
			if(startIndex > this.length) throw new ArgumentOutOfRangeException("startIndex");
			if(startIndex+length > this.length) throw new ArgumentOutOfRangeException("length");
			if(length == 0) return default(StringChunk);
			return new StringChunk(source, offset+startIndex, length);
		}
		
		public bool Contains(string value)
		{
			if(value == null) throw new ArgumentNullException("value");
			return Contains((StringChunk)value);
		}
		
		public bool Contains(StringChunk value)
		{
			return IndexOf(value) != -1;
		}
		
		public int IndexOf(string value)
		{
			if(value == null) throw new ArgumentNullException("value");
			return IndexOf((StringChunk)value);
		}
		
		public int IndexOf(StringChunk value)
		{
			if(value.length > this.length) return -1;
			var e1 = this.GetEnumerator();
			var e2 = value.GetEnumerator();
			int len = this.length - value.length + 1;
			for(int i = 0; i < len; i++)
			{
				if(EqualsChunk(ref e1, ref e2, value.length))
				{
					return i;
				}else{
					e2.Reset();
					e1.Position = i+1;
				}
			}
			return -1;
		}
		
		public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
		{
			for(int i = 0; i < count; i++)
			{
				destination[destinationIndex+i] = this[sourceIndex+i];
			}
		}
		
		#region Split helpers
		
		public struct SingleSplitEnumerable : IEnumerable<StringChunk>
		{
			StringChunk.Enumerator source;
			readonly char separator;
			
			internal SingleSplitEnumerable(ref StringChunk chunk, char separator)
			{
				source = chunk.GetEnumerator();
				this.separator = separator;
			}
			
			public Enumerator GetEnumerator()
			{
				return new Enumerator(ref source, separator);
			}
			
			public struct Enumerator : IEnumerator<StringChunk>
			{
				bool disposed;
				StringChunk.Enumerator source;
				readonly char separator;
				StringChunk current;
				
				public Enumerator(ref StringChunk.Enumerator source, char separator) : this()
				{
					this.source = source;
					this.separator = separator;
				}
				
				
				public StringChunk Current{
					get{
						ThrowIfDisposed();
						return current;
					}
				}
				
				object IEnumerator.Current{
					get{
						return Current;
					}
				}
				
				public bool MoveNext()
				{
					ThrowIfDisposed();
					int start = source.Position+1;
					int len = source.Source.length+1;
					for(int i = start; i < len; i++)
					{
						if(!source.MoveNext())
						{
							current = source.Source.Substring(start, i-start);
							return true;
						}
						if(source.Current == separator)
						{
							current = source.Source.Substring(start, i-start);
							return true;
						}
					}
					return false;
				}
				
				public void Reset()
				{
					ThrowIfDisposed();
					source.Reset();
				}
				
				public void Dispose()
				{
					disposed = true;
					source = default(StringChunk.Enumerator);
				}
			
				private void ThrowIfDisposed()
				{
					if(disposed) throw new ObjectDisposedException(TypeOf<Enumerator>.TypeID.FullName);
				}
			}
			
			IEnumerator<StringChunk> IEnumerable<StringChunk>.GetEnumerator()
			{
				return GetEnumerator();
			}
			
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
		
		public struct MultiSplitEnumerable : IEnumerable<StringChunk>
		{
			StringChunk.Enumerator source;
			readonly char[] separators;
			
			internal MultiSplitEnumerable(ref StringChunk chunk, char[] separators)
			{
				source = chunk.GetEnumerator();
				this.separators = separators;
			}
			
			public Enumerator GetEnumerator()
			{
				return new Enumerator(ref source, separators);
			}
			
			public struct Enumerator : IEnumerator<StringChunk>
			{
				bool disposed;
				StringChunk.Enumerator source;
				readonly char[] separators;
				StringChunk current;
				
				public Enumerator(ref StringChunk.Enumerator source, char[] separators) : this()
				{
					this.source = source;
					this.separators = separators;
				}
				
				
				public StringChunk Current{
					get{
						ThrowIfDisposed();
						return current;
					}
				}
				
				object IEnumerator.Current{
					get{
						return Current;
					}
				}
				
				public bool MoveNext()
				{
					ThrowIfDisposed();
					int start = source.Position+1;
					int len = source.Source.length+1;
					for(int i = start; i < len; i++)
					{
						if(!source.MoveNext())
						{
							current = source.Source.Substring(start, i-start);
							return true;
						}
						if(separators.Contains(source.Current))
						{
							current = source.Source.Substring(start, i-start);
							return true;
						}
					}
					return false;
				}
				
				public void Reset()
				{
					ThrowIfDisposed();
					source.Reset();
				}
				
				public void Dispose()
				{
					disposed = true;
					source = default(StringChunk.Enumerator);
				}
			
				private void ThrowIfDisposed()
				{
					if(disposed) throw new ObjectDisposedException(TypeOf<Enumerator>.TypeID.FullName);
				}
			}
			
			IEnumerator<StringChunk> IEnumerable<StringChunk>.GetEnumerator()
			{
				return GetEnumerator();
			}
			
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
		
		public struct SingleStringSplitEnumerable : IEnumerable<StringChunk>
		{
			StringChunk.Enumerator source;
			StringChunk separator;
			
			internal SingleStringSplitEnumerable(ref StringChunk chunk, ref StringChunk separator)
			{
				source = chunk.GetEnumerator();
				this.separator = separator;
			}
			
			public Enumerator GetEnumerator()
			{
				return new Enumerator(ref source, ref separator);
			}
			
			public struct Enumerator : IEnumerator<StringChunk>
			{
				bool disposed;
				StringChunk.Enumerator source;
				StringChunk.Enumerator separator;
				StringChunk current;
				
				public Enumerator(ref StringChunk.Enumerator source, ref StringChunk separator) : this()
				{
					this.source = source;
					this.separator = separator.GetEnumerator();
				}
				
				
				public StringChunk Current{
					get{
						ThrowIfDisposed();
						return current;
					}
				}
				
				object IEnumerator.Current{
					get{
						return Current;
					}
				}
				
				public bool MoveNext()
				{
					ThrowIfDisposed();
					int start = source.Position+1;
					int len = source.Source.length+1;
					for(int i = start; i < len; i++)
					{
						if(!source.MoveNext())
						{
							current = source.Source.Substring(start, i-start);
							return true;
						}
						source.Position = i-1;
						separator.Reset();
						int slen = separator.Source.length;
						if(EqualsChunk(ref source, ref separator, slen))
						{
							i += slen;
							current = source.Source.Substring(start, i-start-slen);
							return true;
						}else{
							source.Position = i-slen+2;
						}
					}
					return false;
				}
				
				public void Reset()
				{
					ThrowIfDisposed();
					source.Reset();
				}
				
				public void Dispose()
				{
					disposed = true;
					source = default(StringChunk.Enumerator);
				}
			
				private void ThrowIfDisposed()
				{
					if(disposed) throw new ObjectDisposedException(TypeOf<Enumerator>.TypeID.FullName);
				}
			}
			
			IEnumerator<StringChunk> IEnumerable<StringChunk>.GetEnumerator()
			{
				return GetEnumerator();
			}
			
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
		
		public struct MultiStringSplitEnumerable : IEnumerable<StringChunk>
		{
			StringChunk.Enumerator source;
			StringChunk.Enumerator[] separators;
			
			internal MultiStringSplitEnumerable(ref StringChunk chunk, StringChunk.Enumerator[] separators)
			{
				source = chunk.GetEnumerator();
				this.separators = separators;
			}
			
			public Enumerator GetEnumerator()
			{
				return new Enumerator(ref source, separators);
			}
			
			public struct Enumerator : IEnumerator<StringChunk>
			{
				bool disposed;
				StringChunk.Enumerator source;
				StringChunk.Enumerator[] separators;
				StringChunk current;
				
				public Enumerator(ref StringChunk.Enumerator source, StringChunk.Enumerator[] separators) : this()
				{
					this.source = source;
					this.separators = separators;
				}
				
				
				public StringChunk Current{
					get{
						ThrowIfDisposed();
						return current;
					}
				}
				
				object IEnumerator.Current{
					get{
						return Current;
					}
				}
				
				public bool MoveNext()
				{
					ThrowIfDisposed();
					int start = source.Position+1;
					int len = source.Source.length+1;
					for(int i = start; i < len; i++)
					{
						if(!source.MoveNext())
						{
							current = source.Source.Substring(start, i-start);
							return true;
						}
						for(int j = 0; j < separators.Length; j++)
						{
							source.Position = i-1;
							separators[j].Reset();
							int slen = separators[j].Source.length;
							if(EqualsChunk(ref source, ref separators[j], slen))
							{
								i += slen;
								current = source.Source.Substring(start, i-start-slen);
								return true;
							}else{
								source.Position = i-slen+2;
							}
						}
					}
					return false;
				}
				
				public void Reset()
				{
					ThrowIfDisposed();
					source.Reset();
				}
				
				public void Dispose()
				{
					disposed = true;
					source = default(StringChunk.Enumerator);
				}
			
				private void ThrowIfDisposed()
				{
					if(disposed) throw new ObjectDisposedException(TypeOf<Enumerator>.TypeID.FullName);
				}
			}
			
			IEnumerator<StringChunk> IEnumerable<StringChunk>.GetEnumerator()
			{
				return GetEnumerator();
			}
			
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
		
		#endregion
		
		public SingleSplitEnumerable Split(char separator)
		{
			return new SingleSplitEnumerable(ref this, separator);
		}
		
		public MultiSplitEnumerable Split(params char[] separators)
		{
			if(separators == null) throw new ArgumentNullException("separators");
			return new MultiSplitEnumerable(ref this, separators);
		}
		
		public SingleStringSplitEnumerable Split(string separator)
		{
			if(separator == null) throw new ArgumentNullException("separators");
			return Split((StringChunk)separator);
		}
		
		public SingleStringSplitEnumerable Split(StringChunk separator)
		{
			return new SingleStringSplitEnumerable(ref this, ref separator);
		}
		
		public MultiStringSplitEnumerable Split(params string[] separators)
		{
			if(separators == null) throw new ArgumentNullException("separators");
			StringChunk[] arr = new StringChunk[separators.Length];
			for(int i = 0; i < separators.Length; i++)
			{
				arr[i] = (StringChunk)separators[i];
			}
			return Split(arr);
		}
		
		public MultiStringSplitEnumerable Split(params StringChunk[] separators)
		{
			if(separators == null) throw new ArgumentNullException("separators");
			Enumerator[] arr = new Enumerator[separators.Length];
			for(int i = 0; i < separators.Length; i++)
			{
				arr[i] = separators[i].GetEnumerator();
			}
			return new MultiStringSplitEnumerable(ref this, arr);
		}
		
		public bool StartsWith(string value)
		{
			if(value == null) throw new ArgumentNullException("value");
			return StartsWith((StringChunk)value);
		}
		
		public bool StartsWith(StringChunk value)
		{
			if(value.length > this.length) return false;
			return EqualsChunk(this.GetEnumerator(), value.GetEnumerator(), value.length);
		}
		
		public bool EndsWith(string value)
		{
			if(value == null) throw new ArgumentNullException("value");
			return EndsWith((StringChunk)value);
		}
		
		public bool EndsWith(StringChunk value)
		{
			if(value.length > this.length) return false;
			var enum1 = this.GetEnumerator();
			var enum2 = value.GetEnumerator();
			enum1.Position = this.length-value.length-1;
			return EqualsChunk(ref enum1, ref enum2, value.length);
		}
		
		public char[] ToCharArray()
		{
			char[] arr = new char[length];
			CopyTo(0, arr, 0, length);
			return arr;
		}
		
		#region Helpers
		
		private static bool EqualsChunk(Enumerator enum1, Enumerator enum2, int length = -1)
		{
			return EqualsChunk(ref enum1, ref enum2, length);
		}
		
		private static bool EqualsChunk(ref Enumerator enum1, ref Enumerator enum2, int length = -1)
		{
			if(length == -1) length = Int32.MaxValue;
			for(int i = 0; i < length; i++)
			{
				bool move1 = enum1.MoveNext();
				bool move2 = enum2.MoveNext();
				if(move1 == move2)
				{
					if(move1)
					{
						if(enum1.Current != enum2.Current) return false;
					}else{
						return true;
					}
				}else{
					return false;
				}
			}
			return true;
		}
		
		private static int CompareChunk(Enumerator enum1, Enumerator enum2, int length = -1)
		{
			return CompareChunk(ref enum1, ref enum2, length);
		}
		
		private static int CompareChunk(ref Enumerator enum1, ref Enumerator enum2, int length = -1)
		{
			if(length == -1) length = Int32.MaxValue;
			for(int i = 0; i < length; i++)
			{
				bool move1 = enum1.MoveNext();
				bool move2 = enum2.MoveNext();
				if(move1 == move2)
				{
					if(move1)
					{
						int result = enum1.Current.CompareTo(enum2.Current);
						if(result != 0) return result;
					}else{
						return 0;
					}
				}else{
					return move1 ? 1 : -1;
				}
			}
			return 0;
		}
		
		#endregion
		
		#endregion
		
		#region IEquatable
		
		public override bool Equals(object obj)
		{
			if(obj is StringChunk)
			{
				return Equals((StringChunk)obj);
			}else{
				string str = obj as string;
				if(str != null)
				{
					return Equals(str);
				}else{
					return false;
				}
			}
		}
		
		public bool Equals(StringChunk other)
		{
			return EqualsChunk(this.GetEnumerator(), other.GetEnumerator());
		}
		
		public bool Equals(string other)
		{
			if(other == null) return false;
			return Equals((StringChunk)other);
		}
		
		#endregion
		
		#region IEnumerable
		
		public Enumerator GetEnumerator()
		{
			return new Enumerator(ref this);
		}
		
		public struct Enumerator : IEnumerator<char>
		{
			internal StringChunk Source;
			int offset;
			bool disposed;
			
			internal Enumerator(ref StringChunk source) : this()
			{
				this.Source = source;
				this.offset = -1;
			}
			
			public char Current{
				get{
					ThrowIfDisposed();
					if(offset >= Source.length) throw new InvalidOperationException();
					return Source[offset];
				}
			}
			
			object IEnumerator.Current{
				get{
					return Current;
				}
			}
			
			internal int Position{
				get{
					return offset;
				}
				set{
					offset = value;
				}
			}
			
			public void Dispose()
			{
				disposed = true;
				Source = default(StringChunk);
			}
			
			public bool MoveNext()
			{
				ThrowIfDisposed();
				if(++offset < Source.length)
				{
					return true;
				}else{
					return false;
				}
			}
			
			public void Reset()
			{
				ThrowIfDisposed();
				offset = -1;
			}
			
			private void ThrowIfDisposed()
			{
				if(disposed) throw new ObjectDisposedException(TypeOf<Enumerator>.TypeID.FullName);
			}
		}
		
		IEnumerator<char> IEnumerable<char>.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		#endregion
		
		#region ICloneable
		
		object ICloneable.Clone()
		{
			return this;
		}
		
		#endregion
		
		#region IConvertible
		
		private IConvertible Convertible{
			get{
				return (IConvertible)ToString();
			}
		}
		
		TypeCode IConvertible.GetTypeCode()
		{
			return TypeCode.String;
		}
		
		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convertible.ToBoolean(provider);
		}
		
		char IConvertible.ToChar(IFormatProvider provider)
		{
			return Convertible.ToChar(provider);
		}
		
		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convertible.ToSByte(provider);
		}
		
		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convertible.ToByte(provider);
		}
		
		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convertible.ToInt16(provider);
		}
		
		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convertible.ToUInt16(provider);
		}
		
		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convertible.ToInt32(provider);
		}
		
		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convertible.ToUInt32(provider);
		}
		
		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convertible.ToInt64(provider);
		}
		
		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convertible.ToUInt64(provider);
		}
		
		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return Convertible.ToSingle(provider);
		}
		
		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convertible.ToDouble(provider);
		}
		
		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convertible.ToDecimal(provider);
		}
		
		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return Convertible.ToDateTime(provider);
		}
		
		string IConvertible.ToString(IFormatProvider provider)
		{
			return ToString();
		}
		
		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			return Convertible.ToType(conversionType, provider);
		}
		
		#endregion
		
		#region IComparable
		
		public int CompareTo(string other)
		{
			if(other == null) throw new ArgumentNullException("other");
			return ToString().CompareTo(other);
		}
		
		public int CompareTo(StringChunk other)
		{
			return CompareChunk(this.GetEnumerator(), other.GetEnumerator());
		}
		
		public int CompareTo(object obj)
		{
			if(obj is StringChunk)
			{
				return CompareTo((StringChunk)obj);
			}else{
				string str = obj as string;
				if(str != null)
				{
					return CompareTo(str);
				}else{
					throw new ArgumentException("Argument must be a string or StringChunk.", "obj");
				}
			}
		}
		
		#endregion
		
		#region Operators
		
		public static implicit operator StringChunk(string value)
		{
			return new StringChunk(value);
		}
		
		public static explicit operator string(StringChunk chunk)
		{
			return chunk.ToString();
		}
		
		public static bool operator ==(StringChunk lhs, StringChunk rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(StringChunk lhs, StringChunk rhs)
		{
			return !(lhs == rhs);
		}
		
		#endregion
	}
}
