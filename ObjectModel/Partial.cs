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
		protected ICompletable Container{get; private set;}
		
		Notifier notifier;
		
		public Partial(ICompletable completable, out Partial<T> property) : this(new Notifier(completable), out property)
		{
			Container = completable;
		}
		
		private Partial(Notifier notifier, out Partial<T> property) : base(notifier.WaitForValue)
		{
			property = this;
			notifier.Parent = this;
		}
		
		public Partial(ICompletable completable) : this(new Notifier(completable))
		{
			Container = completable;
		}
		
		private Partial(Notifier notifier) : base(notifier.WaitForValue)
		{
			this.notifier = notifier;
		}
		
		public void MarkCreated()
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
				OnValueObtained(completable.WaitForProperty(parent));
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
				//value = parent.Value; //causes recursion
			}
		}
		
		bool ICompletable.IsCompleted{
			get{
				return this.IsValueCreated;
			}
		}
		
		public void CreateValue()
		{
			var value = this.Value;
		}
		
		void ICompletable.Complete()
		{
			CreateValue();
		}
		
		readonly List<Action<T>> receivers = new List<Action<T>>();
		
		public virtual void RegisterReceiver<TArg>(Partial<TArg> property, Action<TArg> valueReceiver)
		{
			if(!Object.ReferenceEquals(property, this)) throw Completable.InvalidProperty();
			if(IsValueCreated)
			{
				To<Action<T>>.Cast(valueReceiver).Invoke(Value);
			}else{
				receivers.Add(To<Action<T>>.Cast(valueReceiver));
			}
		}
		
		public virtual TArg WaitForProperty<TArg>(Partial<TArg> property)
		{
			if(!Object.ReferenceEquals(property, this)) throw Completable.InvalidProperty();
			return To<TArg>.Cast(Value);
		}
		
		public virtual bool ContainsProperty<TArg>(Partial<TArg> property)
		{
			return Object.ReferenceEquals(property, this);
		}
	}
}
