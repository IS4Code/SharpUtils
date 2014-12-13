using System;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils
{
	/// <summary>
	/// This struct is an union of some basic types. Each field points to the same location.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct VariadicUnion
	{
		[FieldOffset(0)]
		public bool AsBoolean;
		
		[FieldOffset(0)]
		public byte AsByte;
		
		[FieldOffset(0)]
		public short AsInt16;
		
		[FieldOffset(0)]
		public int AsInt32;
		
		[FieldOffset(0)]
		public long AsInt64;
		
		[FieldOffset(0)]
		[CLSCompliant(false)]
		public sbyte AsSByte;
		
		[FieldOffset(0)]
		[CLSCompliant(false)]
		public ushort AsUInt16;
		
		[FieldOffset(0)]
		[CLSCompliant(false)]
		public uint AsUInt32;
		
		[FieldOffset(0)]
		[CLSCompliant(false)]
		public ulong AsUInt64;
		
		[FieldOffset(0)]
		public float AsSingle;
		
		[FieldOffset(0)]
		public double AsDouble;
		
		[FieldOffset(0)]
		public decimal AsDecimal;
		
		[FieldOffset(0)]
		public char AsChar;
		
		[FieldOffset(0)]
		public DateTime AsDateTime;
		
		[FieldOffset(0)]
		public IntPtr AsIntPtr;
		
		[FieldOffset(0)]
		public Guid AsGuid;
	}
}