/* Date: 27.3.2015, Time: 13:52 */
using System;

namespace IllidanS4.SharpUtils.Unsafe
{
	public interface IMemoryUnion
	{
		int Size{get;}
		Type DataType{get;}
	}
	
	public interface IMemoryUnion<T1, T2> : IMemoryUnion
	{
		T1 Field1{get; set;}
		T2 Field2{get; set;}
	}
	
	public interface IMemoryUnion<T1, T2, T3> : IMemoryUnion<IMemoryUnion<T1, T2>, T3>
	{
		new T1 Field1{get; set;}
		new T2 Field2{get; set;}
		T3 Field3{get; set;}
	}
	
	public interface IMemoryUnion<T1, T2, T3, T4> : IMemoryUnion<IMemoryUnion<T1, T2, T3>, T4>
	{
		new T1 Field1{get; set;}
		new T2 Field2{get; set;}
		T3 Field3{get; set;}
		T4 Field4{get; set;}
	}
	
	public interface IMemoryUnion<T1, T2, T3, T4, T5> : IMemoryUnion<IMemoryUnion<T1, T2, T3, T4>, T5>
	{
		new T1 Field1{get; set;}
		new T2 Field2{get; set;}
		T3 Field3{get; set;}
		T4 Field4{get; set;}
		T5 Field5{get; set;}
	}
	
	public interface IMemoryUnion<T1, T2, T3, T4, T5, T6> : IMemoryUnion<IMemoryUnion<T1, T2, T3, T4, T5>, T6>
	{
		new T1 Field1{get; set;}
		new T2 Field2{get; set;}
		T3 Field3{get; set;}
		T4 Field4{get; set;}
		T5 Field5{get; set;}
		T6 Field6{get; set;}
	}
	
	public interface IMemoryUnion<T1, T2, T3, T4, T5, T6, T7> : IMemoryUnion<IMemoryUnion<T1, T2, T3, T4, T5, T6>, T7>
	{
		new T1 Field1{get; set;}
		new T2 Field2{get; set;}
		T3 Field3{get; set;}
		T4 Field4{get; set;}
		T5 Field5{get; set;}
		T6 Field6{get; set;}
		T7 Field7{get; set;}
	}
	
	public interface IMemoryUnion<T1, T2, T3, T4, T5, T6, T7, T8> : IMemoryUnion<IMemoryUnion<T1, T2, T3, T4, T5, T6, T7>, T8>
	{
		new T1 Field1{get; set;}
		new T2 Field2{get; set;}
		T3 Field3{get; set;}
		T4 Field4{get; set;}
		T5 Field5{get; set;}
		T6 Field6{get; set;}
		T7 Field7{get; set;}
		T8 Field8{get; set;}
	}
}
