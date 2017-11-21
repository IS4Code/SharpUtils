/* Date: 21.11.2017, Time: 23:05 */
using System;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.ObjectModel
{
	public sealed class PartialArray<T> : Partial<T[]>
	{
		readonly Dictionary<int, PartialElement> parts = new Dictionary<int, PartialElement>();
		
		public PartialArray(ICompletable completable, out Partial<T[]> property) : base(completable, out property)
		{
			
		}
		
		public PartialArray(ICompletable completable) : base(completable)
		{
			
		}
		
		public Partial<T> this[int index]
		{
			get{
				PartialElement part;
				if(!parts.TryGetValue(index, out part))
				{
					part = parts[index] = new PartialElement(index, this);
					part.MarkCreated();
				}
				return part;
			}
		}
		
		public override void RegisterReceiver<TArg>(Partial<TArg> property, Action<TArg> valueReceiver)
		{
			if(base.ContainsProperty(property))
			{
				base.RegisterReceiver(property, valueReceiver);
				return;
			}
			if(!ContainsProperty(property)) throw Completable.InvalidProperty("property");
			
			Container.RegisterReceiver(property, valueReceiver);
			this.RegisterReceiver(this, arr => valueReceiver(To<TArg>.Cast(arr[((PartialElement)(object)property).Index])));
		}
		
		public override TArg WaitForProperty<TArg>(Partial<TArg> property)
		{
			if(base.ContainsProperty(property)) return base.WaitForProperty(property);
			if(!ContainsProperty(property)) throw Completable.InvalidProperty("property");
			
			try{
				return Container.WaitForProperty(property);
			}catch(PropertyIncompleteException)
			{
				return To<TArg>.Cast(Value[((PartialElement)(object)property).Index]);
			}
		}
		
		public override bool ContainsProperty<TArg>(Partial<TArg> property)
		{
			if(base.ContainsProperty(property)) return true;
			
			var prop = property as PartialElement;
			if(prop != null)
			{
				return parts.ContainsValue(prop);
			}
			return false;
		}
		
		private class PartialElement : Partial<T>
		{
			public int Index{get; private set;}
			
			public PartialElement(int index, ICompletable completable, out Partial<T> property) : base(completable, out property)
			{
				Index = index;
			}
			
			public PartialElement(int index, ICompletable completable) : base(completable)
			{
				Index = index;
			}
		}
	}
}
