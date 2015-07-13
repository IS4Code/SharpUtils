/* Date: 22.6.2015, Time: 18:44 */
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace IllidanS4.SharpUtils.Accessing
{
	public class PropertyAccessor<T> : ReadPropertyAccessor<T>, IReadWriteAccessor<T>
	{
		readonly Action<T> setter;
		
		public PropertyAccessor(PropertyInfo pi, object target) : base(pi, target)
		{
			setter = CreateSet(pi, target);
		}
		
		public new T Item{
			set{
				setter(value);
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
		
		protected static Action<T> CreateSet(PropertyInfo pi, object target)
		{
			if(target == null)
			{
				return (Action<T>)pi.GetSetMethod().CreateDelegate(TypeOf<Action<T>>.TypeID);
			}else{
				return (Action<T>)pi.GetSetMethod().CreateDelegate(TypeOf<Action<T>>.TypeID, target);
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
	
	public class ReadPropertyAccessor<T> : DefinedReadAccessor<T>, IPropertyAccessor
	{
		public PropertyInfo Property{get; private set;}
		public object Target{get; private set;}
		
		public ReadPropertyAccessor(PropertyInfo pi, object target) : base(CreateGet(pi, target))
		{
			Property = pi;
			Target = target;
		}
		
		protected static Func<T> CreateGet(PropertyInfo pi, object target)
		{
			if(target == null)
			{
				return (Func<T>)pi.GetGetMethod().CreateDelegate(TypeOf<Func<T>>.TypeID);
			}else{
				return (Func<T>)pi.GetGetMethod().CreateDelegate(TypeOf<Func<T>>.TypeID, target);
			}
		}
	}
	
	public interface IPropertyAccessor : IReadAccessor
	{
		PropertyInfo Property{get;}
		object Target{get;}
	}
}
