/* Date: ‎20.12.‎2012, Time: ‏‎17:02 */
using System;
using System.Reflection;

namespace IllidanS4.SharpUtils.Accessing
{
	public sealed class FieldAccessor<T> : IReadWriteAccessor<T>
	{
		readonly Action<T> setter;
		readonly Func<T> getter;
		
		public FieldAccessor(FieldInfo field, object target)
		{
			if(field.FieldType != TypeOf<T>.TypeID)throw new TypeLoadException("Field is not of type "+TypeOf<T>.TypeID.ToString()+".");
			if(target != null && field.DeclaringType != target.GetType())throw new TypeLoadException("Target does not contain this field.");
			setter = AccessorGenerator.GenerateSetter<T>(field, target);
			getter = AccessorGenerator.GenerateGetter<T>(field, target);
		}
		
		public T Value
		{
			get{
				return getter();
			}
			set{
				setter(value);
			}
		}
	}
}