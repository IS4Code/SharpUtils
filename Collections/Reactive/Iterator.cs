/* Date: 30.10.2017, Time: 1:38 */
using System;

namespace IllidanS4.SharpUtils.Collections.Reactive
{
	public class Iterator<T> : IIterator<T>
	{
		readonly Func<T, bool> onNext;
		readonly Action onCompleted;
		
		public Iterator(Func<T, bool> onNext, Action onCompleted)
		{
			this.onNext = onNext;
			this.onCompleted = onCompleted;
		}
		
		public bool OnNext(T value)
		{
			if(onNext != null) return onNext(value);
			return true;
		}
		
		public void OnCompleted()
		{
			if(onCompleted != null) onCompleted();
		}
	}
	
	public static class Iterator
	{
		public static Iterator<T> Create<T>(Func<T, bool> onNext=null, Action onCompleted=null)
		{
			return new Iterator<T>(onNext, onCompleted);
		}
	}
}
