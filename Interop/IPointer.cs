using System;
namespace IllidanS4.SharpUtils.Interop
{
	/// <summary>
	/// Non-generic base of <see cref="Pointer&lt;T&gt;"/>.
	/// </summary>
	public interface IPointer
	{
		IntPtr ToIntPtr();
		Type Type{get;}
		bool IsNull{get;}
	}
}