/* Date: 22.9.2017, Time: 11:46 */
using System;
using System.Runtime.CompilerServices;

namespace IllidanS4.SharpUtils
{
	/// <summary>
	/// Class used for fast (no-conversion) casts.
	/// </summary>
	public static class To<TTo>
	{
		/// <summary>
		/// Shortcut to <see cref="Extensions.FastCast"/>. You can utilize generic type inference when calling this method.
		/// </summary>
		/// <param name="arg">The value, expressed as <typeparamref name="TFrom"/>.</param>
		/// <returns>The same value, represented by <typeparamref name="TTo"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TTo Cast<TFrom>(TFrom arg)
		{
			return Extensions.FastCast<TFrom, TTo>(arg);
		}
	}
}
