/* Date: 24.4.2015, Time: 15:38 */
using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using IllidanS4.SharpUtils.Interop;

namespace IllidanS4.SharpUtils.Proxies.Dynamic
{
	/// <summary>
	/// Collection of marshalling tools for adapters and proxies.
	/// </summary>
	public static class AdapterTools
	{
		public static ObjectTypeHandle[] Marshal(params object[] args)
		{
			ObjectTypeHandle[] argHandles = new ObjectTypeHandle[args.Length];
			for(int i = 0; i < args.Length; i++)
			{
				argHandles[i] = Marshal(args[i]);
			}
			return argHandles;
		}
		
		public static ObjectTypeHandle Marshal(object arg)
		{
			if(arg == null)
			{
				return null;
			}
			ObjectTypeHandle handle = arg as ObjectTypeHandle;
			if(handle != null)
			{
				return handle;
			}
			Delegate del = arg as Delegate;
			if(del != null)
			{
				return new DelegateAdapter(del);
			}
			Task task = arg as Task;
			if(task != null)
			{
				return new TaskAdapter(task);
			}
			return new ObjectTypeHandle(arg);
		}
		
		public static object[] Unmarshal(params ObjectHandle[] args)
		{
			object[] oArgs = new object[args.Length];
			for(int i = 0; i < args.Length; i++)
			{
				oArgs[i] = Unmarshal(args[i]);
			}
			return oArgs;
		}
		
		public static object Unmarshal(ObjectHandle arg)
		{
			if(arg == null)
			{
				return null;
			}else{
				DelegateAdapter delAd = arg as DelegateAdapter;
				if(delAd != null)
				{
					return CreateDelegate(delAd);
				}
				TaskAdapter taskAd = arg as TaskAdapter;
				if(taskAd != null)
				{
					return CreateTask(taskAd);
				}
				DynamicAdapter dynAd = arg as DynamicAdapter;
				if(dynAd != null)
				{
					return new DynamicProxy(dynAd);
				}
				StaticAdapter statAd = arg as StaticAdapter;
				if(statAd != null)
				{
					return new StaticProxy(statAd);
				}
				return arg.Unwrap();
			}
		}
		
		public static Delegate CreateDelegate(DelegateAdapter del)
		{
			var mdel = new MarshalArgsDelegate(del);
			return mdel.CreateInvoker();
		}
		
		public static Task CreateTask(TaskAdapter task)
		{
			Type resType = task.ResultType;
			TaskMarshaller marshal;
			if(resType != null)
			{
				marshal = (TaskMarshaller)Activator.CreateInstance(TaskMarshaller.Type.MakeGenericType(resType));
			}else{
				marshal = new TaskMarshaller<NoResult>();
			}
			task.RegisterMarshaller(marshal);
			return marshal.Task;
		}
		
		internal abstract class TaskMarshaller : MarshalByRefObject
		{
			internal static readonly Type Type = typeof(TaskMarshaller<>);
			
			public abstract void SetException(Exception[] exc);
			public abstract void SetCanceled();
			public abstract void SetResult(ObjectTypeHandle result);
			internal abstract Task Task{get;}
		}
		
		private sealed class NoResult
		{
			private NoResult()
			{
				
			}
		}
		
		internal class TaskMarshaller<T> : TaskMarshaller
		{
			private readonly TaskCompletionSource<T> tcs;
			
			public TaskMarshaller()
			{
				tcs = new TaskCompletionSource<T>();
			}
			
			internal override Task Task{
				get{
					return tcs.Task;
				}
			}
			
			public override void SetResult(ObjectTypeHandle result)
			{
				tcs.SetResult((T)AdapterTools.Unmarshal(result));
			}
			
			public override void SetException(Exception[] exc)
			{
				tcs.SetException(exc);
			}
			
			public override void SetCanceled()
			{
				tcs.SetCanceled();
			}
		}
		
		private class MarshalArgsDelegate : VarArgsDelegate
		{
			public DelegateAdapter Delegate{get; private set;}
			
			public MarshalArgsDelegate(DelegateAdapter del)
			{
				Delegate = del;
			}
			
			public Delegate CreateInvoker()
			{
				return CreateInvoker(Delegate.ProxyType);
			}
			
			public override object Invoke(__arglist)
			{
				ArgIterator ai = new ArgIterator(__arglist);
				ObjectTypeHandle[] args = new ObjectTypeHandle[ai.GetRemainingCount()];
				while(ai.GetRemainingCount() > 0)
				{
					int idx = args.Length-ai.GetRemainingCount();
					object arg = TypedReference.ToObject(ai.GetNextArg());
					args[idx] = AdapterTools.Marshal(arg);
				}
				var res = Delegate.InvokeMember("Invoke", ref args, new bool[args.Length]);
				ai = new ArgIterator(__arglist);
				while(ai.GetRemainingCount() > 0)
				{
					int idx = args.Length-ai.GetRemainingCount();
					ObjectHandle arg = args[idx];
					var tr = ai.GetNextArg();
					if(__reftype(tr).IsByRef)
						TypedReferenceTools.SetValue(tr, arg.Unwrap());
				}
				if(res != null)
					return res.Unwrap();
				else return null;
			}
		}
	}
}
