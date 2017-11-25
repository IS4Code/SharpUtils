/* Date: 25.11.2017, Time: 14:38 */
using System;
using System.Threading;

namespace IllidanS4.SharpUtils.Threads
{
	/// <summary>
	/// Represents an object that can be used to create and access fibers on the current thread.
	/// </summary>
	public interface IFiberFactory
	{
		/// <summary>
		/// Returns the currently running fiber in this thread.
		/// </summary>
		FiberBase CurrentFiber{get;}
		
		/// <summary>
		/// Creates a new fiber from its method.
		/// </summary>
		/// <param name="start">The method that will be run when the fiber is resumed.</param>
		/// <returns>The newly created fiber.</returns>
		FiberBase CreateNew(ThreadStart start);
		
		/// <summary>
		/// Creates a new fiber from its method.
		/// </summary>
		/// <param name="start">The method that will be run when the fiber is resumed.</param>
		/// <param name="maxStackSize">The stack size for the new fiber, or 0 if the default one should be used.</param>
		/// <returns>The newly created fiber.</returns>
		FiberBase CreateNew(ThreadStart start, int maxStackSize);
		
		/// <summary>
		/// Creates a new fiber from its method, specifying the parameter that should be passed to the method.
		/// </summary>
		/// <param name="start">The method that will be run when the fiber is resumed.</param>
		/// <param name="parameter">The object that will be passed to the method.</param>
		/// <returns>The newly created fiber.</returns>
		FiberBase CreateNew(ParameterizedThreadStart start, object parameter);
		
		/// <summary>
		/// Creates a new fiber from its method, specifying the parameter that should be passed to the method.
		/// </summary>
		/// <param name="start">The method that will be run when the fiber is resumed.</param>
		/// <param name="maxStackSize">The stack size for the new fiber, or 0 if the default one should be used.</param>
		/// <param name="parameter">The object that will be passed to the method.</param>
		/// <returns>The newly created fiber.</returns>
		FiberBase CreateNew(ParameterizedThreadStart start, int maxStackSize, object parameter);
	}
}
