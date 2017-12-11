/* Date: 18.11.2017, Time: 2:06 */
using System;
using System.Collections.Generic;

namespace IllidanS4.SharpUtils.ObjectModel
{
	/// <summary>
	/// Contains basic implementation of the <see cref="ICompletable"/> interface.
	/// </summary>
	public abstract class Completable : ICompletable, IDisposable
	{
		bool completed;
		
		readonly IEnumerator<Property> completion;
		
		public bool IsCompleted{
			get{
				return completed;
			}
		}
		
		public Completable()
		{
			completion = Completion().GetEnumerator();
		}
		
		public void Complete()
		{
			if(completed) return;
			
			while(completion.MoveNext())
			{
				var prop = completion.Current;
				Properties.Add(prop);
				Delegate receiver;
				if(PropertyReceivers.TryGetValue(prop.PropertyObject, out receiver))
				{
					receiver.DynamicInvoke(prop.Value);
					PropertyReceivers.Remove(prop.PropertyObject);
				}
			}
			
			completed = true;
		}
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				completion.Dispose();
			}
		}
		
		~Completable()
		{
			Dispose(false);
		}
		
		/// <summary>
		/// This method should contain the main property-yielding process.
		/// </summary>
		/// <returns>An iterator that completes properties of the instance.</returns>
		protected abstract IEnumerable<Property> Completion();
		
		public abstract bool ContainsProperty<T>(Partial<T> property);
		
		private readonly List<Property> Properties = new List<Property>();
		private readonly Dictionary<object, Delegate> PropertyReceivers = new Dictionary<object, Delegate>();
		
		public void RegisterReceiver<T>(Partial<T> property, Action<T> valueReceiver)
		{
			if(!ContainsProperty(property)) throw InvalidProperty();
			
			foreach(var prop in Properties)
			{
				if(prop.PropertyObject == property)
				{
					valueReceiver((T)prop.Value);
					return;
				}
			}
			PropertyReceivers.Add(property, valueReceiver);
		}
		
		public T WaitForProperty<T>(Partial<T> property)
		{
			if(!ContainsProperty(property)) throw InvalidProperty();
			
			foreach(var prop in Properties)
			{
				if(prop.PropertyObject == property)
				{
					return (T)prop.Value;
				}
			}
			
			if(!completed)
			{
				while(completion.MoveNext())
				{
					var prop = completion.Current;
					Properties.Add(prop);
					if(prop.PropertyObject == property)
					{
						return (T)prop.Value;
					}
					Delegate receiver;
					if(PropertyReceivers.TryGetValue(prop.PropertyObject, out receiver))
					{
						prop.InvokeReceiver(receiver);
						PropertyReceivers.Remove(prop.PropertyObject);
					}
				}
				completed = true;
			}
			
			throw new PropertyIncompleteException();
		}
		
		public static ArgumentException InvalidProperty(string paramName="property")
		{
			return new ArgumentException("This object doesn't support the specified property.", paramName);
		}
		
		/// <summary>
		/// This class is used to construct property assignments for the completion of the object.
		/// </summary>
		protected abstract class Property
		{
			public object PropertyObject{get; private set;}
			public object Value{get; private set;}
			
			protected Property(object property, object value)
			{
				PropertyObject = property;
				Value = value;
			}
			
			public static Property Set<T>(Partial<T> property, T value)
			{
				return new Property<T>(property, value);
			}
			
			public abstract void InvokeReceiver(Delegate receiver);
		}
		
		private class Property<T> : Property
		{
			public Property(Partial<T> property, T value) : base(property, value)
			{
				
			}
			
			public override void InvokeReceiver(Delegate receiver)
			{
				((Action<T>)receiver).Invoke((T)Value);
			}
		}
	}
}
