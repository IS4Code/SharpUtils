using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils.Serialization
{
	public static class Serializer
	{
		public static byte[] MemorySerialize<T>(T value) where T : struct
		{
			Type t = TypeOf<T>.TypeID;
			int size = UnsafeTools.SizeOf(t);
			byte[] arr = new byte[size];
			Copier<T>.Copy(arr, value);
			return arr;
		}
		
		public static byte[] MemorySerialize<T>(T? value) where T : struct
		{
			Type t = TypeOf<T?>.TypeID;
			int size = UnsafeTools.SizeOf(t);
			byte[] arr = new byte[size];
			Copier<T?>.Copy(arr, value);
			return arr;
		}
		
		private static class Copier<T>
		{
			private delegate void Copy_d(byte[] arr, T value);
			private static readonly Copy_d Copy_f;
			
			static Copier()
			{
				DynamicMethod copy = new DynamicMethod("Copy", null, new[]{TypeOf<byte[]>.TypeID, TypeOf<T>.TypeID}, typeof(Copier<T>), true);
				var il = copy.GetILGenerator();
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(OpCodes.Conv_U);
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Stelem, TypeOf<T>.TypeID);
				il.Emit(OpCodes.Ret);
				Copy_f = copy.CreateDelegate(TypeOf<Copy_d>.TypeID) as Copy_d;
			}
			
			public static void Copy(byte[] arr, T value)
			{
				Copy_f(arr, value);
			}
		}
	}
}
