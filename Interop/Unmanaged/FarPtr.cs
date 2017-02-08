/* Date: 8.1.2017, Time: 21:51 */
using System;

namespace IllidanS4.SharpUtils.Interop.Unmanaged
{
	/// <summary>
	/// Represents a pointer to another memory.
	/// </summary>
	[Serializable]
	public struct FarPtr : IEquatable<FarPtr>, IComparable<FarPtr>, IComparable
	{
		internal readonly MemoryContext memory;
		internal readonly long value;
		
		public FarPtr(MemoryContext context, long address)
		{
			if(address >= 2L<<(context.PointerSize*8-1)) throw new ArgumentOutOfRangeException("address", "The address lies outside the target memory addressing space.");
			memory = context;
			value = address;
		}
		
		public int ToInt32()
		{
			return checked((int)value);
		}
		public long ToInt64()
		{
			return value;
		}
		public IntPtr ToIntPtr()
		{
			return (IntPtr)value;
		}
		
		public override bool Equals(object obj)
		{
			return (obj is FarPtr) && Equals((FarPtr)obj);
		}
		
		public bool Equals(FarPtr other)
		{
			return Object.Equals(this.memory, other.memory) && this.value == other.value;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (memory != null)
					hashCode += 1000000007 * memory.GetHashCode();
				hashCode += 1000000009 * value.GetHashCode();
			}
			return hashCode;
		}
		
		public static bool operator ==(FarPtr lhs, FarPtr rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(FarPtr lhs, FarPtr rhs)
		{
			return !(lhs == rhs);
		}
		
		public static FarPtr operator +(FarPtr ptr, int offset)
		{
			return new FarPtr(ptr.memory, checked(ptr.value+offset));
		}
		
		public static FarPtr operator +(FarPtr ptr, long offset)
		{
			return new FarPtr(ptr.memory, checked(ptr.value+offset));
		}
		
		public static FarPtr operator -(FarPtr ptr, int offset)
		{
			return new FarPtr(ptr.memory, checked(ptr.value-offset));
		}
		
		public static FarPtr operator -(FarPtr ptr, long offset)
		{
			return new FarPtr(ptr.memory, checked(ptr.value-offset));
		}
		
		public static long operator -(FarPtr a, FarPtr b)
		{
			a.CheckSameMemory(ref b);
			return a.value-b.value;
		}
		
		int IComparable.CompareTo(object other)
		{
			if(other is FarPtr)
			{
				return CompareTo((FarPtr)other);
			}else{
				throw new ArgumentException();
			}
		}
		
		public int CompareTo(FarPtr other)
		{
			if(memory.Equals(other.memory))
			{
				return value.CompareTo(other.value);
			}else{
				throw new ArgumentException("This FarPtr instance comes from another memory context.");
			}
		}
		
		internal void CheckSameMemory(ref FarPtr other)
		{
			if(!memory.Equals(other.memory))
			{
				throw new InvalidOperationException();
			}
		}
	}
}
