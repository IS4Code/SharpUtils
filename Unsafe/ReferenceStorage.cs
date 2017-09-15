/* Date: 9.8.2017, Time: 1:30 */
using System;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.Unsafe
{
	/// <summary>
	/// This class can be used to enforce value-type semantics onto
	/// a reference-containing structure. It allows users to query
	/// for a reference type based on the current address of a provided
	/// value type.
	/// </summary>
	public static class ReferenceStorage
	{
		/// <summary>
		/// Call this method from an "Update" method inside the struct, passing
		/// "ref this" as the first argument, and its field as the second.
		/// Store the returned object in the field.
		/// </summary>
		/// <param name="val">The reference to index with.</param>
		/// <param name="orig">The original value of the field.</param>
		/// <returns>The new value of the field.</returns>
		/// <typeparam name="T">
		/// The structure should contain a single private field, having the type <typeparamref name="TData"/>.
		/// </typeparam>
		/// <typeparam name="TData">
		/// The underlying data of the structure.
		/// </typeparam>
		public static TData FindInstance<T, TData>(ref T val, TData orig) where T : struct where TData : class, ICloneable
		{
			return Storage<T, TData>.FindInstance(ref val, orig);
		}
		
		static class Storage<T, TData> where T : struct where TData : class, ICloneable
		{
			static readonly Dictionary<IntPtr, WeakReference<TData>> Cache = new Dictionary<IntPtr, WeakReference<TData>>();
			
			public static TData FindInstance(ref T val, TData orig)
			{
				return UnsafeTools.GetPointer(
					out val,
					ptr => {
						WeakReference<TData> wref;
						TData inst;
						if(Cache.TryGetValue(ptr, out wref) && wref.TryGetTarget(out inst))
						{
							if(inst != orig)
							{
								inst = (TData)orig.Clone();
								Cache[ptr] = new WeakReference<TData>(inst);
							}
							return inst;
						}else{
							inst = (TData)orig.Clone();
							Cache[ptr] = new WeakReference<TData>(inst);
							return inst;
						}
					}
				);
			}
		}
	}
}
