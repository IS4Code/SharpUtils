/* Date: 18.11.2017, Time: 1:46 */
using System;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.ObjectModel
{
	/// <summary>
	/// Represents a container whose contents may not be loaded yet.
	/// </summary>
	public class Partial<T> : Lazy<T>, ICompletable
	{
		Notifier notifier;
		
		public Partial(ICompletable completable, out Partial<T> property) : this(new Notifier(completable), out property)
		{
			
		}
		
		private Partial(Notifier notifier, out Partial<T> property) : base(notifier.WaitForValue)
		{
			property = this;
			notifier.Parent = this;
		}
		
		public Partial(ICompletable completable) : this(new Notifier(completable))
		{
			
		}
		
		private Partial(Notifier notifier) : base(notifier.WaitForValue)
		{
			this.notifier = notifier;
		}
		
		public void Created()
		{
			if(notifier == null) throw new InvalidOperationException("The property was already created.");
			notifier.Parent = this;
			notifier = null;
		}
		
		class Notifier
		{
			readonly ICompletable completable;
			bool valueReceived;
			T value;
			
			Partial<T> parent;
			
			public Partial<T> Parent{
				get{
					return parent;
				}
				set{
					parent = value;
					completable.RegisterReceiver(parent, OnValueReceived);
				}
			}
			
			public Notifier(ICompletable completable)
			{
				this.completable = completable;
			}
			
			public T WaitForValue()
			{
				if(valueReceived) return value;
				if(parent == null) throw new InvalidOperationException("The object hasn't been initialized yet.");
				OnValueObtained(completable.WaitForValue(parent));
				return value;
			}
			
			private void OnValueObtained(T value)
			{
				this.value = value;
				valueReceived = true;
				
				foreach(var receiver in parent.receivers)
				{
					receiver(value);
				}
			}
			
			private void OnValueReceived(T value)
			{
				OnValueObtained(value);
				value = parent.Value;
			}
		}
		
		bool ICompletable.IsCompleted{
			get{
				return this.IsValueCreated;
			}
		}
		
		void ICompletable.Complete()
		{
			var value = this.Value;
		}
		
		readonly List<Action<T>> receivers = new List<Action<T>>();
		
		void ICompletable.RegisterReceiver<TArg>(Partial<TArg> property, Action<TArg> valueReceiver)
		{
			if(!Object.ReferenceEquals(property, this)) throw Completable.InvalidProperty();
			if(IsValueCreated)
			{
				To<Action<T>>.Cast(valueReceiver).Invoke(Value);
			}else{
				receivers.Add(To<Action<T>>.Cast(valueReceiver));
			}
		}
		
		TArg ICompletable.WaitForValue<TArg>(Partial<TArg> property)
		{
			if(!Object.ReferenceEquals(property, this)) throw Completable.InvalidProperty();
			return To<TArg>.Cast(Value);
		}
		
		bool ICompletable.ContainsProperty<TArg>(Partial<TArg> property)
		{
			return Object.ReferenceEquals(property, this);
		}
	}
}
