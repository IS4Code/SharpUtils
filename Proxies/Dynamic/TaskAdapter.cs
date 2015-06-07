/* Date: 29.4.2015, Time: 21:45 */
using System;
using System.Linq;
using System.Threading.Tasks;
using IllidanS4.SharpUtils.Reflection;
using Microsoft.CSharp.RuntimeBinder;

namespace IllidanS4.SharpUtils.Proxies.Dynamic
{
	/// <summary>
	/// An adapter used for marshalling tasks.
	/// </summary>
	public class TaskAdapter : StaticAdapter
	{
		private readonly Task Task;
		
		public Type ResultType{
			get{
				var type = Task.GetType();
				if(type.IsGenericType)
				{
					Type resType = type.GenericTypeArguments[0];
					if(resType == Types.CommonLanguageRuntimeLibrary.GetType("System.Threading.Tasks.VoidTaskResult"))
					{
						return null;
					}else{
						return resType;
					}
				}else{
					return null;
				}
			}
		}
		
		public TaskAdapter(Task task) : base(task)
		{
			Task = task;
		}
		
		private void RegisterMarshaller(TaskMarshaller marshaller)
		{
			Task.ContinueWith(
				t => {
					if(t.IsFaulted)
					{
						marshaller.SetException(t.Exception.InnerExceptions.ToArray());
					}else if(t.IsCanceled)
					{
						marshaller.SetCanceled();
					}else if(t.IsCompleted)
					{
						if(ResultType != null)
						{
							marshaller.SetResult(AdapterTools.Marshal(((dynamic)t).Result));
						}else{
							marshaller.SetResult(null);
						}
					}
				}
			);
		}
		
		public static Task CreateTask(TaskAdapter adapter)
		{
			Type resType = adapter.ResultType;
			TaskMarshaller marshal;
			if(resType != null)
			{
				marshal = (TaskMarshaller)Activator.CreateInstance(TaskMarshaller.Type.MakeGenericType(resType));
			}else{
				marshal = new TaskMarshaller<NoResult>();
			}
			adapter.RegisterMarshaller(marshal);
			return marshal.Task;
		}
		
		private abstract class TaskMarshaller : MarshalByRefObject
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
		
		private class TaskMarshaller<T> : TaskMarshaller
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
	}
}
