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
		Type Type{get;}
	}
	
	/// <summary>
	/// The interface for "get" accessors.
	/// </summary>
	[DefaultMember("Item")]
	public interface IReadAccessor : IStorageAccessor
	{
		object Item{get;}
	}
	
	/// <summary>
	/// The interface for "set" accessors.
	/// </summary>
	[DefaultMember("Item")]
	public interface IWriteAccessor : IStorageAccessor
	{
		object Item{set;}
	}
	
	/// <summary>
	/// The interface for "get" accessors.
	/// </summary>
	[DefaultMember("Item")]
	public interface IReadAccessor<out T> : IReadAccessor
	{
		new T Item{get;}
	}
	
	/// <summary>
	/// The interface for "set" accessors.
	/// </summary>
	[DefaultMember("Item")]
	public interface IWriteAccessor<in T> : IWriteAccessor
	{
		new T Item{set;}
	}
	
	[CLSCompliant(false)]
	public interface ITypedReference : IReadAccessor, IWriteAccessor, IStrongBox
	{
		TRet GetReference<TRet>(TypedReferenceTools.TypedRefFunc<TRet> func);
	}
	
	/// <summary>
	/// An accessor that provides an "out" reference.
	/// </summary>
	public interface IOutReference<T> : IWriteAccessor<T>, IStorageAccessor
	{
		TRet GetReference<TRet>(Reference.OutFunc<T, TRet> func);
	}
	
	/// <summary>
	/// An accessor that provides a "ref" reference.
	/// </summary>
	public interface IRefReference<T> : IReadAccessor<T>, IWriteAccessor<T>, IOutReference<T>
	{
		TRet GetReference<TRet>(Reference.RefFunc<T, TRet> func);
	}
	
	/// <summary>
	/// Basic generic accessor.
	/// </summary>
	public abstract class BasicAccessor<T> : IStorageAccessor
	{
		public Type Type{
			get{
				return typeof(T);
			}
		}
	}
	
	[DefaultMember("Item")]
	public abstract class BasicReadAccessor<T> : BasicAccessor<T>, IReadAccessor<T>
	{
		public abstract T Item{get;}
		object IReadAccessor.Item{
			get{
				return this.Item;
			}
		}
	}
	
	[DefaultMember("Item")]
	public abstract class BasicWriteAccessor<T> : BasicAccessor<T>, IWriteAccessor<T>
	{
		public abstract T Item{set;}
		object IWriteAccessor.Item{
			set{
				Item = (T)value;
			}
		}
	}
	
	[DefaultMember("Item")]
	public abstract class BasicReadWriteAccessor<T> : BasicAccessor<T>, IReadAccessor<T>, IWriteAccessor<T>, IStrongBox
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