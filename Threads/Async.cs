/* Date: 12.11.2014, Time: 14:59 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IllidanS4.SharpUtils.Threads
{
	public static class Async
	{
		public static Task AsTask(Func<IEnumerable<Task>> proc)
		{
			return new Task(()=>RunTaskSync(proc));
		}
		
		public static Task AsTask(this IEnumerable<Task> proc)
		{
			return new Task(()=>RunTaskSync(proc));
		}
		
		public static Task AsTask(IEnumerator<Task> proc)
		{
			return new Task(()=>RunTaskSync(proc));
		}
		
		public static void RunTaskAsync<T>(Func<IEnumerable<Task<T>>> proc)
		{
			RunTaskAsync((Func<IEnumerable<Task>>)proc);
		}
		
		public static void RunTaskAsync(Func<IEnumerable<Task>> proc)
		{
			RunTaskAsync(proc());
		}
		
		public static void RunTaskAsync<T>(this IEnumerable<Task<T>> proc)
		{
			RunTaskAsync((IEnumerable<Task>)proc);
		}
		
		public static void RunTaskAsync(this IEnumerable<Task> proc)
		{
			RunTaskAsync(proc.GetEnumerator());
		}
		
		public static void RunTaskAsync<T>(IEnumerator<Task<T>> proc)
		{
			RunTaskAsync((IEnumerator<Task>)proc);
		}
		
		public static void RunTaskAsync(IEnumerator<Task> proc)
		{
			if(!proc.MoveNext()) return;
			Task task = proc.Current;
			task.Start();
			new Thread(
				(ThreadStart)delegate{
					task.Wait();
					RunTaskSync(proc);
				}
			).Start();
		}
		
		public static void RunTaskSync(Func<IEnumerable<Task>> proc)
		{
			RunTaskSync(proc());
		}
		
		public static void RunTaskSync(this IEnumerable<Task> proc)
		{
			RunTaskSync(proc.GetEnumerator());
		}
		
		public static void RunTaskSync(IEnumerator<Task> proc)
		{
			while(proc.MoveNext())
			{
				Task task = proc.Current;
				task.Start();
				task.Wait();
			}
		}
	}
}
