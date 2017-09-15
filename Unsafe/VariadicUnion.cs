using System;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Unsafe
{
	/// <summary>
	/// This struct is an union of some basic types. Each field points to the same location.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct VariadicUnion
	{
		/// <summary>
		/// The bool value of the union.
		/// </summary>
		[FieldOffset(0)]
		public bool AsBoolean;
		
		/// <summary>
		/// The byte value of the union.
		/// </summary>
		[FieldOffset(0)]
		public byte AsByte;
		
		/// <summary>
		/// The int16 value of the union.
		/// </summary>
		[FieldOffset(0)]
		public short AsInt16;
		
		/// <summary>
		/// The int32 value of the union.
		/// </summary>
		[FieldOffset(0)]
		public int AsInt32;
		
		/// <summary>
		/// The int64 value of the union.
		/// </summary>
		[FieldOffset(0)]
		public long AsInt64;
		
		/// <summary>
		/// The sbyte value of the union.
		/// </summary>
		[FieldOffset(0)]
		[CLSCompliant(false)]
		public sbyte AsSByte;
		
		/// <summary>
		/// The uint16 value of the union.
		/// </summary>
		[FieldOffset(0)]
		[CLSCompliant(false)]
		public ushort AsUInt16;
		
		/// <summary>
		/// The uint32 value of the union.
		/// </summary>
		[FieldOffset(0)]
		[CLSCompliant(false)]
		public uint AsUInt32;
		
		/// <summary>
		/// The uint64 value of the union.
		/// </summary>
		[FieldOffset(0)]
		[CLSCompliant(false)]
		public ulong AsUInt64;
		
		/// <summary>
		/// The single value of the union.
		/// </summary>
		[FieldOffset(0)]
		public float AsSingle;
		
		/// <summary>
		/// The double value of the union.
		/// </summary>
		[FieldOffset(0)]
		public double AsDouble;
		
		/// <summary>
		/// The decimal value of the union.
		/// </summary>
		[FieldOffset(0)]
		public decimal AsDecimal;
		
		/// <summary>
		/// The char value of the union.
		/// </summary>
		[FieldOffset(0)]
		public char AsChar;
		
		/// <summary>
		/// The DateTime value of the union.
		/// </summary>
		[FieldOffset(0)]
		public DateTime AsDateTime;
		
		/// <summary>
		/// The IntPtr value of the union.
		/// </summary>
		[FieldOffset(0)]
		public IntPtr AsIntPtr;
		
		/// <summary>
		/// The Guid value of the union.
		/// </summary>
		[FieldOffset(0)]
		public Guid AsGuid;
	}
}