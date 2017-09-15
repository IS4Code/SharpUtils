/* Date: 20.12.2012, Time: 17:02 */
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Interop;
using IllidanS4.SharpUtils.Metadata;

namespace IllidanS4.SharpUtils.Accessing
{
	/// <summary>
	/// Basic interface for all accessors.
	/// </summary>
	public interface IStorageAccessor
	{
		/// <summary>
		/// The type of the accessor.
		/// </summary>
		Type Type{get;}
	}
	
	/// <summary>
	/// The interface for "get" accessors.
	/// </summary>
	[DefaultMember("Item")]
	public interface IReadAccessor : IStorageAccessor
	{
		/// <summary>
		/// The value of the object.
		/// </summary>
		object Item{get;}
	}
	
	/// <summary>
	/// The interface for "set" accessors.
	/// </summary>
	[DefaultMember("Item")]
	public interface IWriteAccessor : IStorageAccessor
	{
		/// <summary>
		/// The value of the object.
		/// </summary>
		object Item{set;}
	}
	
	/// <summary>
	/// The interface for "get-set" accessors.
	/// </summary>
	[DefaultMember("Item")]
	public interface IReadWriteAccessor : IReadAccessor, IWriteAccessor, IStrongBox
	{
		/// <summary>
		/// The value of the object.
		/// </summary>
		new object Item{get;set;}
	}
	
	/// <summary>
	/// The interface for "get" accessors.
	/// </summary>
	[DefaultMember("Item")]
	public interface IReadAccessor<out T> : IReadAccessor
	{
		/// <summary>
		/// The value of the object.
		/// </summary>
		new T Item{get;}
	}
	
	/// <summary>
	/// The interface for "set" accessors.
	/// </summary>
	[DefaultMember("Item")]
	public interface IWriteAccessor<in T> : IWriteAccessor
	{
		/// <summary>
		/// The value of the object.
		/// </summary>
		new T Item{set;}
	}
	
	/// <summary>
	/// The interface for "get-set" accessors.
	/// </summary>
	[DefaultMember("Item")]
	public interface IReadWriteAccessor<T> : IReadAccessor<T>, IWriteAccessor<T>, IReadWriteAccessor
	{
		/// <summary>
		/// The value of the object.
		/// </summary>
		new T Item{get;set;}
	}
	
	/// <summary>
	/// An accessor that provides a safe reference.
	/// </summary>
	public interface ITypedReference : IStorageAccessor
	{
		/// <summary>
		/// Creates a reference to the value.
		/// </summary>
		TRet GetReference<TRet>(Func<SafeReference,TRet> func);
	}
	
	/// <summary>
	/// An accessor that provides an "out" reference.
	/// </summary>
	public interface IOutReference<T> : IWriteAccessor<T>, IStorageAccessor, ITypedReference
	{
		/// <summary>
		/// Creates a reference to the value.
		/// </summary>
		TRet GetReference<TRet>(Reference.OutFunc<T, TRet> func);
	}
	
	/// <summary>
	/// An accessor that provides a "ref" reference.
	/// </summary>
	public interface IRefReference<T> : IReadWriteAccessor<T>, IOutReference<T>
	{
		/// <summary>
		/// Creates a reference to the value.
		/// </summary>
		TRet GetReference<TRet>(Reference.RefFunc<T, TRet> func);
	}
	
	/// <summary>
	/// Basic generic accessor.
	/// </summary>
	public abstract class BasicAccessor<T> : MarshalByRefObject, IStorageAccessor
	{
		Type IStorageAccessor.Type{
			get{
				return typeof(T);
			}
		}
	}
	
	/// <summary>
	/// The basic implementation of a read accessor.
	/// </summary>
	[DefaultMember("Item")]
	public abstract class BasicReadAccessor<T> : BasicAccessor<T>, IReadAccessor<T>
	{
		/// <summary>
		/// The value of the object.
		/// </summary>
		public abstract T Item{get;}
		object IReadAccessor.Item{
			get{
				return this.Item;
			}
		}
	}
	
	/// <summary>
	/// The basic implementation of a write accessor.
	/// </summary>
	[DefaultMember("Item")]
	public abstract class BasicWriteAccessor<T> : BasicAccessor<T>, IWriteAccessor<T>
	{
		/// <summary>
		/// The value of the object.
		/// </summary>
		public abstract T Item{set;}
		object IWriteAccessor.Item{
			set{
				Item = (T)value;
			}
		}
	}
	
	/// <summary>
	/// The basic implementation of a read-write accessor.
	/// </summary>
	[DefaultMember("Item")]
	public abstract class BasicReadWriteAccessor<T> : BasicAccessor<T>, IReadWriteAccessor<T>, IStrongBox
	{
		/// <summary>
		/// The value of the object.
		/// </summary>
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
		object IReadWriteAccessor.Item{
			get{
				return this.Item;
			}
			set{
				Item = (T)value;
			}
		}
		
		object IStrongBox.Value{
			get{
				return this.Item;
			}
			set{
				Item = (T)value;
			}
		}
	}
}