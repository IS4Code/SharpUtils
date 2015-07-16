/* Date: 16.7.2015, Time: 1:48 */
using System;
using System.Collections.Generic;
using IllidanS4.SharpUtils.Accessing;

namespace IllidanS4.SharpUtils.Collections
{
	public class ValueTypeEnumerator<TEnumerator, TElement> : IEnumerator<TElement> where TEnumerator : struct, IEnumerator<TElement>
	{
		readonly IReadWriteAccessor<TEnumerator> acc;
		
		private TEnumerator Item{
			get{
				return acc.Item;
			}
			set{
				acc.Item = value;
			}
		}
		
		public ValueTypeEnumerator(IReadWriteAccessor<TEnumerator> acc)
		{
			this.acc = acc;
		}
		
		public TElement Current{
			get{
				return Item.Current;
			}
		}
		
		object System.Collections.IEnumerator.Current{
			get{
				return Current;
			}
		}
		
		public void Dispose()
		{
			var it = Item;
			try{
				it.Dispose();
			}finally{
				Item = it;
			}
		}
		
		public bool MoveNext()
		{
			var it = Item;
			try{
				return it.MoveNext();
			}finally{
				Item = it;
			}
		}
		
		public void Reset()
		{
			var it = Item;
			try{
				it.Reset();
			}finally{
				Item = it;
			}
		}
	}
}
