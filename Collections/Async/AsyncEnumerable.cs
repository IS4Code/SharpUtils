/* Date: 14.7.2015, Time: 0:39 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IllidanS4.SharpUtils.Collections.Async
{
	public static class AsyncEnumerable
	{
		/*public static async Task<IEnumerable<T>> When<T>(this IAsyncEnumerable<T> source)
		{
			var aenum = source.GetEnumerator();
			while(await aenum.MoveNext())
			{
				yield return aenum.Current;
			}
		}*/
		
		public static IAsyncEnumerable<T> Run<T>(IEnumerable<T> ienum)
		{
			return new EnumerableAsyncWrapper<T>(ienum);
		}
		
		/// <example>
		/// This code creates a simple asynchronous enumerable.
		/// <code><![CDATA[
		/// public static IAsyncEnumerable<string> Test()
		/// {
		///     return AsyncEnumerable.Create<string>(
		/// 		async yield =>
		/// 		{
		/// 			await Task.Delay(100);
		/// 			await yield("A");
		/// 			await Task.Delay(200);
		/// 			await yield("B");
		/// 			await Task.Delay(300);
		/// 			await yield("C");
		/// 			await Task.Delay(400);
		/// 			await yield("D");
		/// 			await Task.Delay(500);
		/// 			await yield("E");
		/// 		}
		/// 	);
		/// }
		/// ]]></code>
		/// </example>
		public static IAsyncEnumerable<T> Create<T>(Func<Func<T, Task>,Task> task)
		{
			return new TaskEnumerableWrapper<T>(task);
		}
		
		public static async Task<List<T>> ToList<T>(this IAsyncEnumerable<T> source)
		{
			List<T> list = new List<T>();
			using(var aenum = source.GetEnumerator())
			{
				while(await aenum.MoveNext())
				{
					list.Add(aenum.Current);
				}
			}
			return list;
		}
		
		public static IEnumerable<T> Wait<T>(this IAsyncEnumerable<T> source)
		{
			using(var aenum = source.GetEnumerator())
			{
				while(aenum.MoveNext().Result)
				{
					yield return aenum.Current;
				}
			}
		}
		
		private class TaskEnumerableWrapper<T> : IAsyncEnumerable<T>
		{
			private readonly Func<Func<T, Task>,Task> task;
			
			public TaskEnumerableWrapper(Func<Func<T, Task>,Task> task)
			{
				this.task = task;
			}
			
			public IAsyncEnumerator<T> GetEnumerator()
			{
				return new Enumerator(task);
			}
			
			IAsyncEnumerator IAsyncEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
			
			private class Enumerator : IAsyncEnumerator<T>
			{
				public T Current{get; set;}
				private TaskCompletionSource<bool> nextTask;
				private TaskCompletionSource<object> yieldTask = new TaskCompletionSource<object>();
				
				public Enumerator(Func<Func<T, Task>,Task> task)
				{
					Run(task);
				}
			
				private async void Run(Func<Func<T, Task>,Task> task)
				{
					await yieldTask.Task;
					await task(OnNext);
					nextTask.SetResult(false);
				}
				
				private Task OnNext(T value)
				{
					Current = value;
					nextTask.SetResult(true);
					yieldTask = new TaskCompletionSource<object>();
					return yieldTask.Task;
				}
				
				object IAsyncEnumerator.Current{
					get{
						return this.Current;
					}
				}
				
				public Task<bool> MoveNext()
				{
					nextTask = new TaskCompletionSource<bool>();
					yieldTask.SetResult(null);
					return nextTask.Task;
				}
				
				public void Reset()
				{
					throw new NotImplementedException();
				}
				
				public void Dispose()
				{
					throw new NotImplementedException();
				}
			}
		}
		
		private class EnumerableAsyncWrapper<T> : IAsyncEnumerable<T>
		{
			private readonly IEnumerable<T> enumerable;
			
			public EnumerableAsyncWrapper(IEnumerable<T> enumerable)
			{
				this.enumerable = enumerable;
			}
			
			IAsyncEnumerator IAsyncEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
			
			public IAsyncEnumerator<T> GetEnumerator()
			{
				return new Enumerator(enumerable.GetEnumerator());
			}
			
			private class Enumerator : IAsyncEnumerator<T>
			{
				private readonly IEnumerator<T> enumerator;
			
				public Enumerator(IEnumerator<T> enumerator)
				{
					this.enumerator = enumerator;
				}
				
				public T Current{
					get{
						return enumerator.Current;
					}
				}
				
				object IAsyncEnumerator.Current{
					get{
						return this.Current;
					}
				}
				
				public Task<bool> MoveNext()
				{
					return Task.Run((Func<bool>)enumerator.MoveNext);
				}
				
				public void Reset()
				{
					enumerator.Reset();
				}
				
				public void Dispose()
				{
					enumerator.Dispose();
				}
			}
		}
	}
}
