/* Date: 27.3.2015, Time: 13:52 */
using System;

namespace IllidanS4.SharpUtils.Unsafe
{
	/// <summary>
	/// A memory union.
	/// </summary>
	public interface IMemoryUnion
	{
		/// <summary>
		/// The size of the union.
		/// </summary>
		int Size{get;}
		/// <summary>
		/// The type of the union.
		/// </summary>
		Type DataType{get;}
	}
	
	/// <summary>
	/// A memory union of 2 values.
	/// </summary>
	public interface IMemoryUnion<T1, T2> : IMemoryUnion
	{
		/// <summary>
		/// The 1st value.
		/// </summary>
		T1 Field1{get; set;}
		/// <summary>
		/// The 2nd value.
		/// </summary>
		T2 Field2{get; set;}
	}
	
	/// <summary>
	/// A memory union of 3 values.
	/// </summary>
	public interface IMemoryUnion<T1, T2, T3> : IMemoryUnion<IMemoryUnion<T1, T2>, T3>
	{
		/// <summary>
		/// The 1st value.
		/// </summary>
		new T1 Field1{get; set;}
		/// <summary>
		/// The 2nd value.
		/// </summary>
		new T2 Field2{get; set;}
		/// <summary>
		/// The 3rd value.
		/// </summary>
		T3 Field3{get; set;}
	}
	
	/// <summary>
	/// A memory union of 4 values.
	/// </summary>
	public interface IMemoryUnion<T1, T2, T3, T4> : IMemoryUnion<IMemoryUnion<T1, T2, T3>, T4>
	{
		/// <summary>
		/// The 1st value.
		/// </summary>
		new T1 Field1{get; set;}
		/// <summary>
		/// The 2nd value.
		/// </summary>
		new T2 Field2{get; set;}
		/// <summary>
		/// The 3rd value.
		/// </summary>
		T3 Field3{get; set;}
		/// <summary>
		/// The 4th value.
		/// </summary>
		T4 Field4{get; set;}
	}
	
	/// <summary>
	/// A memory union of 5 values.
	/// </summary>
	public interface IMemoryUnion<T1, T2, T3, T4, T5> : IMemoryUnion<IMemoryUnion<T1, T2, T3, T4>, T5>
	{
		/// <summary>
		/// The 1st value.
		/// </summary>
		new T1 Field1{get; set;}
		/// <summary>
		/// The 2nd value.
		/// </summary>
		new T2 Field2{get; set;}
		/// <summary>
		/// The 3rd value.
		/// </summary>
		T3 Field3{get; set;}
		/// <summary>
		/// The 4th value.
		/// </summary>
		T4 Field4{get; set;}
		/// <summary>
		/// The 5th value.
		/// </summary>
		T5 Field5{get; set;}
	}
	
	/// <summary>
	/// A memory union of 6 values.
	/// </summary>
	public interface IMemoryUnion<T1, T2, T3, T4, T5, T6> : IMemoryUnion<IMemoryUnion<T1, T2, T3, T4, T5>, T6>
	{
		/// <summary>
		/// The 1st value.
		/// </summary>
		new T1 Field1{get; set;}
		/// <summary>
		/// The 2nd value.
		/// </summary>
		new T2 Field2{get; set;}
		/// <summary>
		/// The 3rd value.
		/// </summary>
		T3 Field3{get; set;}
		/// <summary>
		/// The 4th value.
		/// </summary>
		T4 Field4{get; set;}
		/// <summary>
		/// The 5th value.
		/// </summary>
		T5 Field5{get; set;}
		/// <summary>
		/// The 6th value.
		/// </summary>
		T6 Field6{get; set;}
	}
	
	/// <summary>
	/// A memory union of 7 values.
	/// </summary>
	public interface IMemoryUnion<T1, T2, T3, T4, T5, T6, T7> : IMemoryUnion<IMemoryUnion<T1, T2, T3, T4, T5, T6>, T7>
	{
		/// <summary>
		/// The 1st value.
		/// </summary>
		new T1 Field1{get; set;}
		/// <summary>
		/// The 2nd value.
		/// </summary>
		new T2 Field2{get; set;}
		/// <summary>
		/// The 3rd value.
		/// </summary>
		T3 Field3{get; set;}
		/// <summary>
		/// The 4th value.
		/// </summary>
		T4 Field4{get; set;}
		/// <summary>
		/// The 5th value.
		/// </summary>
		T5 Field5{get; set;}
		/// <summary>
		/// The 6th value.
		/// </summary>
		T6 Field6{get; set;}
		/// <summary>
		/// The 7th value.
		/// </summary>
		T7 Field7{get; set;}
	}
	
	/// <summary>
	/// A memory union of 8 values.
	/// </summary>
	public interface IMemoryUnion<T1, T2, T3, T4, T5, T6, T7, T8> : IMemoryUnion<IMemoryUnion<T1, T2, T3, T4, T5, T6, T7>, T8>
	{
		/// <summary>
		/// The 1st value.
		/// </summary>
		new T1 Field1{get; set;}
		/// <summary>
		/// The 2nd value.
		/// </summary>
		new T2 Field2{get; set;}
		/// <summary>
		/// The 3rd value.
		/// </summary>
		T3 Field3{get; set;}
		/// <summary>
		/// The 4th value.
		/// </summary>
		T4 Field4{get; set;}
		/// <summary>
		/// The 5th value.
		/// </summary>
		T5 Field5{get; set;}
		/// <summary>
		/// The 6th value.
		/// </summary>
		T6 Field6{get; set;}
		/// <summary>
		/// The 7th value.
		/// </summary>
		T7 Field7{get; set;}
		/// <summary>
		/// The 8th value.
		/// </summary>
		T8 Field8{get; set;}
	}
}
