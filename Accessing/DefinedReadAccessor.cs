/* Date: ‎20.12.‎2012, Time: ‏‎17:02 */
using System;
namespace IllidanS4.SharpUtils.Accessing
{
	/// <summary>
	/// Read accessor that uses a passed function that returns its value.
	/// </summary>
	public class DefinedReadAccessor<T> : BasicReadAccessor<T>
	{
		Func<T> getter;
		
		/// <summary>
		/// Creates a new read accessor using a getter function.
		/// </summary>
		/// <param name="getter">The getter function that returns the current value.</param>
		public DefinedReadAccessor(Func<T> getter)
		{
			this.getter = getter;
		}
		
		/// <summary>
		/// Creates a new read accessor using a tuple storage.
		/// </summary>
		/// <param name="tuple">The tuple storage.</param>
		public DefinedReadAccessor(Tuple<T> tuple) : this(()=>tuple.Item1)
		{
			
		}
		
		/// <summary>
		/// Creates a new read accessor using a constant value.
		/// </summary>
		/// <param name="constant">The constant value.</param>
		public DefinedReadAccessor(T constant) : this(()=>constant)
		{
			
		}
		
		public override T Item{
			get{
				return getter();
			}
		}
	}
}