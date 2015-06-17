/* Date: 15.6.2015, Time: 20:13 */
//#define STORE_REFERENCE //Removed so you can't access the reference to a released local.
	
using System;
using System.Diagnostics;
using System.Threading;
using IllidanS4.SharpUtils.Interop;

namespace IllidanS4.SharpUtils.Unsafe.Experimental
{
	/* If STORE_REFERENCE is uncommented, ReferencePinHandle will expose Reference
	 * property, which may cause undefined behavior when the reference to a variable
	 * on the stack is no longer valid due to the release of the stack frame.
	 * Therefore, use this define only when really needed, and with caution.
	 */
	
	/// <summary>
	/// An object that pins a reference (so it doesn't change its address).
	/// </summary>
	public abstract class PinHandle : IDisposable
	{
		protected AutoResetEvent Reset{get; set;}
		
		protected PinHandle()
		{
			Reset = new AutoResetEvent(false);
		}
		
		~PinHandle()
		{
			Dispose(false);
		}
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			Reset.Set();
		}
	}
	
	/// <summary>
	/// Pins a reference so that its address remains unchanged.
	/// </summary>
	
	public abstract class ReferencePinHandle<T> : PinHandle
	{
		#if STORE_REFERENCE
		/// <summary>
		/// Retrieves the reference this handle pins.
		/// Problem: If the reference points to the stack and the stack frame is released, the handle becomes invalid, without any way of keeping it alive.
		/// Possible solution: Find a way to check whether a reference is located on the stack or on the heap, and limit the usage of this class to heap references only.
		/// </summary>
		public SafeReference Reference{get; private set;}
		#endif
		
		protected void ReferenceAction(SafeReference r)
		{
			using(var re1 = new AutoResetEvent(false))
			{
				var thr = new Thread(
					delegate()
					{
						r.GetReference( //References are thread-unsafe!
							tr => {
								#if STORE_REFERENCE
								SafeReference.Create(
									tr,
									r2 => {
										Reference = r2;
										re1.Set();
										Reset.WaitOne();
									}
								);
								#else
								tr.Pin(
									delegate{
										re1.Set();
										Reset.WaitOne();
									}
								);
								#endif
							}
						);
					}
				);
				thr.Start();
				re1.WaitOne();
			}
		}
		
		#if STORE_REFERENCE
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				Reference.Dispose();
			}
			base.Dispose(disposing);
		}
		#endif
	}
	
	/// <summary>
	/// Pins a 'ref' reference so that its address remains unchanged.
	/// </summary>
	public class RefPinHandle<T> : ReferencePinHandle<T>
	{
		public RefPinHandle(ref T obj)
		{
			SafeReference.Create(ref obj, ReferenceAction);
		}
	}
	
	/// <summary>
	/// Pins an 'out' reference so that its address remains unchanged.
	/// </summary>
	public class OutPinHandle<T> : ReferencePinHandle<T>
	{
		public OutPinHandle(out T obj)
		{
			SafeReference.CreateOut(out obj, ReferenceAction);
		}
	}
	
	/// <summary>
	/// Pins an object on the heap, so its address stays unchanged during the lifetime of this object.
	/// Use only if <see cref="System.Runtime.InteropServices.GCHandle"/> cannot be used for the object.
	/// Note that this class doesn't provide a way to get the data pointer, like GCHandle does, and that's for the same reason a pinned GCHandle cannot be used for some objects.
	/// </summary>
	public class ObjectPinHandle : PinHandle
	{
		public object Object{get; private set;}
		
		public ObjectPinHandle(object obj)
		{
			Object = obj;
			using(var re1 = new AutoResetEvent(false))
			{
				var thr = new Thread(
					delegate()
					{
						InteropTools.Pin(
							obj,
							delegate{
								re1.Set();
								Reset.WaitOne();
							}
						);
					}
				);
				thr.Start();
				re1.WaitOne();
			}
		}
	}
}
