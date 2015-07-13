/* Date: 3.4.2015, Time: 20:36 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace IllidanS4.SharpUtils.Accessing
{
	/// <summary>
	/// Represents an accessor to a list element.
	/// </summary>
	public class ListAccessor<T> : ReadListAccessor<T>, IReadWriteAccessor<T>, IStrongBox
	{
		public ListAccessor(IList<T> list, int index) : base(list, index)
		{
			if(list.IsReadOnly)
			{
				throw new ArgumentException("This list is read-only.", "list");
			}
		}
		
		public new T Item{
			set{
				List[Index] = value;
			}
			get{
				return base.Item;
			}
		}
		
		object IWriteAccessor.Item{
			set{
				Item = (T)value;
			}
		}
		object IReadWriteAccessor.Item{
			get{
				return Item;
			}
			set{
				Item = (T)value;
			}
		}
		
		object IStrongBox.Value{
			get{
				return Item;
			}
			set{
				Item = (T)value;
			}
		}
	}
	
	public class ReadListAccessor<T> : BasicReadAccessor<T>, IListAccessor<T>
	{
		public IList<T> List{get; private set;}
		public int Index{get; private set;}
		
		public ReadListAccessor(IList<T> list, int index)
		{
			List = list;
			Index = index;
		}
		
		public override T Item{
			get{
				return List[Index];
			}
		}
		
		IEnumerable ICollectionAccessor.Collection{
			get{
				return List;
			}
		}
	}
	
	public interface ICollectionAccessor : IReadAccessor
	{
		IEnumerable Collection{get;}
		int Index{get;}
	}
	
	public interface IListAccessor<T> : IReadAccessor<T>, ICollectionAccessor
	{
		IList<T> List{get;}
	}
	
	public static class ListAccessor
	{
		public static ListAccessor<T> Create<T>(IList<T> list, int index)
		{
			return new ListAccessor<T>(list, index);
		}
		
		public static ReadListAccessor<T> CreateReadOnly<T>(IList<T> list, int index)
		{
			return new ReadListAccessor<T>(list, index);
		}
	}
}
