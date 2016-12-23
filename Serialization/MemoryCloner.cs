/* Date: 23.3.2016, Time: 22:33 */
using System;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils.Serialization
{
	public class MemoryCloner<T> : ICloner<T>
	{
		public static T Clone(T instance)
		{
			if(instance == null) throw new ArgumentNullException("source");
			Type t = TypeOf<T>.TypeID;
			if(t.IsValueType) return instance;
			var copy = (T)UnsafeTools.GetUninitializedObject(t);
			Copier.CopyShallow(instance, copy);
			return copy;
		}
		
		T ICloner<T>.Clone(T instance)
		{
			return Clone(instance);
		}
	}
}
