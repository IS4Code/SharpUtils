/* Date: 20.12.2012, Time: 17:02 */
using System;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Metadata;

namespace IllidanS4.SharpUtils.Accessing
{
	public interface IStorageAccessor
	{
		Type ItemType{get;}
	}
	
	/// <summary>
	/// The interface for "get" accessors.
	/// </summary>
	public interface IReadAccessor : IStorageAccessor
	{
		object Item{get;}
	}
	
	/// <summary>
	/// The interface for "set" accessors.
	/// </summary>
	public interface IWriteAccessor : IStorageAccessor
	{
		object Item{set;}
	}
	
	/// <summary>
	/// The interface for "get" accessors.
	/// </summary>
	public interface IReadAccessor<out T> : IReadAccessor
	{
		new T Item{get;}
	}
	
	/// <summary>
	/// The interface for "set" accessors.
	/// </summary>
	public interface IWriteAccessor<in T> : IWriteAccessor
	{
		new T Item{set;}
	}
	
	[CLSCompliant(false)]
	public interface ITypedReference
	{
		[Boxed(typeof(TypedReference))]
		ValueType Reference{get;}
		Type Type{get;}
		unsafe void GetReference([Out]TypedReference* tr);
	}
	
	public abstract class BasicAccessor<T> : IStorageAccessor
	{
		public Type ItemType{
			get{
				return typeof(T);
			}
		}
	}
	
	public abstract class BasicReadAccessor<T> : BasicAccessor<T>, IReadAccessor<T>
	{
		public abstract T Item{get;}
		object IReadAccessor.Item{
			get{
				return this.Item;
			}
		}
	}
	
	public abstract class BasicWriteAccessor<T> : BasicAccessor<T>, IWriteAccessor<T>
	{
		public abstract T Item{set;}
		object IWriteAccessor.Item{
			set{
				Item = (T)value;
			}
		}
	}
	
	public abstract class BasicReadWriteAccessor<T> : BasicAccessor<T>, IReadAccessor<T>, IWriteAccessor<T>
	{
		public abstract T Item{get; set;}
		object IReadAccessor.Item{
			get{
				return this.Item;
			}
		}
		object IWriteAccessor.Item{
			set{
				Item = (T)value;
			}
		}
	}
}