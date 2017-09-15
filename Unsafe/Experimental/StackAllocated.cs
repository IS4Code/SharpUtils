/* Date: 17.7.2015, Time: 17:42 */
using System;
using IllidanS4.SharpUtils.Interop;

namespace IllidanS4.SharpUtils.Unsafe.Experimental
{
	/// <summary>
	/// Allows allocations of simple referene types on the stack.
	/// </summary>
	/// <example>
	/// <code><![CDATA[
	/// unsafe{
	/// 	void* ptr = stackalloc byte[StackAllocated<object>.Size];
	///		var obj = StackAllocated<object>.Init(ptr);
	///		Console.WriteLine(obj.GetType());
	/// }
	/// ]]></code>
	/// </example>
	[CLSCompliant(false)]
	public static class StackAllocated<T>
	{
		private static readonly Type type = typeof(T);
		/// <summary>
		/// The minimum required size to hold the instance.
		/// </summary>
		public static readonly int Size = UnsafeTools.BaseInstanceSizeOf(type);
		
		static StackAllocated()
		{
			if(InteropTools.ContainsReferences<T>())
			{
				throw new InvalidOperationException("Allocating types that contain references on the stack is not supported.");
			}
			if(type.IsArray || type == TypeOf<string>.TypeID)
			{
				throw new InvalidOperationException("Variable-sized instances aren't supported.");
			}
		}
		
		/// <summary>
		/// Constructs an object instance in a specific place in the memory.
		/// </summary>
		/// <param name="target">The target address of the object.</param>
		/// <returns>The constructed object.</returns>
		public static unsafe T Init(void* target)
		{
			IntPtr* vtable = ((IntPtr*)target)+1;
			*vtable = type.TypeHandle.Value;
			return (T)UnsafeTools.GetObject((IntPtr)(vtable));
		}
	}
}
