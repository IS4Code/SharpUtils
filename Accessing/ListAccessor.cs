/* Date: 3.4.2015, Time: 20:36 */
using System;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.Accessing
{
	/// <summary>
	/// Represents an accessor to a list element.
	/// </summary>
	public class ListAccessor<T> : ReadListAccessor<T>, IWriteAccessor<T>
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
				List[Index] = (T)value;
			}
		}
	}
	
	public class ReadListAccessor<T> : BasicReadAccessor<T>
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
	}
}
