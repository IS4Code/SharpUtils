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
		
		internal void RegisterMarshaller(AdapterTools.TaskMarshaller marshaller)
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
	}
}
