/* Date: ‎20.12.‎2012, Time: ‏‎17:03 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using IllidanS4.SharpUtils.Accessing;

namespace IllidanS4.SharpUtils.Coroutines
{
	public class Coroutine
	{
		static readonly Stack<Coroutine> running = new Stack<Coroutine>();
		
		public static Coroutine Running
		{
			get{
				if(running.Count == 0)
				{
					return null;
				}else{
					return running.Peek();
				}
			}
		}
		
		IEnumerator enumerator;
		IEnumerable enumerable;
		Delegate initFunc;
		
		public CoroutineStatus Status
		{
			get; private set;
		}
		
		public Coroutine(IEnumerable enumerable)
		{
			this.enumerable = enumerable;
		}
		
		public Coroutine(Func<IEnumerable> init)
		{
			initFunc = init;
		}
		
		public Coroutine(Delegate init)
		{
			if(!TypeOf<IEnumerable>.TypeID.IsAssignableFrom(init.Method.ReturnType))
			{
				throw new ArgumentException("Passed method must return IEnumerable object.", "init");
			}
			initFunc = init;
		}
		
		public object Resume()
		{
			if(Status == CoroutineStatus.Dead)
				throw new Exception("Cannot resume dead coroutine");
			if(enumerator == null && enumerable == null)
				InitEnumerable<object>();
			if(enumerator == null)
				enumerator = enumerable.GetEnumerator();
			Status = CoroutineStatus.Running;
			running.Push(this);
			if(enumerator.MoveNext())
			{
				running.Pop();
				Status = CoroutineStatus.Suspended;
				return enumerator.Current;
			}else{
				running.Pop();
				Status = CoroutineStatus.Dead;
				return null;
			}
		}
		
		public object Resume(params object[] args)
		{
			if(Status == CoroutineStatus.Dead)
				throw new Exception("Cannot resume dead coroutine");
			if(enumerator == null && enumerable == null)
				InitEnumerable(args);
			if(enumerator == null)
				enumerator = enumerable.GetEnumerator();
			return Resume();
		}
		
		public object Resume<T>(params T[] args)
		{
			if(Status == CoroutineStatus.Dead)
				throw new Exception("Cannot resume dead coroutine");
			if(enumerator == null && enumerable == null)
				InitEnumerable(args);
			if(enumerator == null)
				enumerator = enumerable.GetEnumerator();
			return Resume();
		}
		
		private void InitEnumerable<T>(params T[] args)
		{
			ParameterInfo[] initParams = initFunc.Method.GetParameters();
			object[] initArgs = new object[initParams.Length];
			for(int i = 0; i < args.Length && i < initParams.Length; i++)
			{
				/*if(initParams[i].ParameterType == TypeOf<IReadWriteAccessor<T>>.TypeID) //TODO
				{
					initArgs[i] = new ReferenceAccessor<T>(ref args[i]);
				}else{*/
					initArgs[i] = args[i];
				//}
			}
			enumerable = initFunc.DynamicInvoke(initArgs) as IEnumerable;
		}
		
		public Func<object> Wrap()
		{
			return Resume;
		}
		
		public Func<object> Wrap(params object[] args)
		{
			return delegate{return Resume(args);};
		}
	}
}