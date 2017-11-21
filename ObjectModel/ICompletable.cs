/* Date: 18.11.2017, Time: 1:43 */
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IllidanS4.SharpUtils.ObjectModel
{
	/// <summary>
	/// Designates an object whose state may be incomplete.
	/// </summary>
	public interface ICompletable
	{
		/// <summary>
		/// Returns true if the object is in the completed state, or false if some properties aren't completed yet.
		/// </summary>
		bool IsCompleted{get;}
		
		/// <summary>
		/// Completes all properties of the object.
		/// </summary>
		void Complete();
		
		/// <summary>
		/// Registers an action which shall be invoked when a property is completed.
		/// </summary>
		/// <param name="property">The property whose value should be obtained.</param>
		/// <param name="valueReceiver">The action that receives the value of the property when it is completed.</param>
		void RegisterReceiver<T>(Partial<T> property, Action<T> valueReceiver);
		
		/// <summary>
		/// Resumes the completion of the object until the specified property is completed.
		/// </summary>
		/// <param name="property">The property whose value should be obtained.</param>
		/// <returns></returns>
		T WaitForProperty<T>(Partial<T> property);
		
		/// <summary>
		/// Checks if this instance contains the specified completable property.
		/// </summary>
		/// <param name="property">The property that should be checked.</param>
		/// <returns>True if this instance contains the property, false otherwise.</returns>
		bool ContainsProperty<T>(Partial<T> property);
	}
}
