/* Date: 20.12.2012, Time: 17:02 */
namespace IllidanS4.SharpUtils.Accessing
{
	/// <summary>
	/// The interface for "get" accessors.
	/// </summary>
	public interface IReadAccessor<out T>
	{
		T Value{get;}
	}
	
	/// <summary>
	/// The interface for "set" accessors.
	/// </summary>
	public interface IWriteAccessor<in T>
	{
		T Value{set;}
	}
	
	/*
	/// <summary>
	/// The interface for both "get" and "set" accessors.
	/// </summary>
	public interface IReadWriteAccessor<T> : IReadAccessor<T>, IWriteAccessor<T>
	{
		
	}*/
}