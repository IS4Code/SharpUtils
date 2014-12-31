using System;
namespace IllidanS4.SharpUtils.Interop
{
	/// <summary>
	/// Non-generic base of <see cref="Pointer&lt;T&gt;"/>.
	/// </summary>
	[CLSCompliant(false)]
	public unsafe interface IPointer
	{
		IntPtr ToIntPtr();
		void* ToPointer();
		bool IsNull{get;}
	}
}