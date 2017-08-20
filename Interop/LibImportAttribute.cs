/* Date: 22.4.2017, Time: 11:01 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Interop
{
	partial class NativeLibrary
	{
		[AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
		public sealed class LibImportAttribute : Attribute
		{
			public string EntryPoint;
			public bool ExactSpelling;
			public bool PreserveSig;
			
			public CallingConvention CallingConvention;
			public CharSet CharSet;
			public bool SetLastError;
			public bool BestFitMapping;
			public bool ThrowOnUnmappableChar;
			
			private static readonly Type UFPType = typeof(UnmanagedFunctionPointerAttribute);
			private static readonly ConstructorInfo UFPCtor = UFPType.GetConstructor(new[]{typeof(CallingConvention)});
				
			public IEnumerable<CustomAttributeBuilder> CreateBuilders()
			{
				var namedProperties = new List<PropertyInfo>();
				var propertyValues = new List<object>();
				
				if(CharSet != 0)
				{
					namedProperties.Add(UFPType.GetProperty("CharSet"));
					propertyValues.Add(CharSet);
				}
				if(SetLastError != false)
				{
					namedProperties.Add(UFPType.GetProperty("SetLastError"));
					propertyValues.Add(SetLastError);
				}
				if(BestFitMapping != false)
				{
					namedProperties.Add(UFPType.GetProperty("BestFitMapping"));
					propertyValues.Add(BestFitMapping);
				}
				if(ThrowOnUnmappableChar != false)
				{
					namedProperties.Add(UFPType.GetProperty("ThrowOnUnmappableChar"));
					propertyValues.Add(ThrowOnUnmappableChar);
				}
				
				var builder = new CustomAttributeBuilder(UFPCtor, new object[]{CallingConvention}, namedProperties.ToArray(), propertyValues.ToArray());
				yield return builder;
			}
			
			public void Apply(TypeBuilder type)
			{
				foreach(var builder in CreateBuilders())
				{
					type.SetCustomAttribute(builder);
				}
			}
		}
	}
}
