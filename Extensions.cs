using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Resources;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils
{
	/// <summary>
	/// This class contains various useful methods and extension methods. It is core class of SharpUtils.
	/// </summary>
	public static class Extensions
	{
		public static IEnumerable<KeyValuePair<int, T>> PairEnumerate<T>(this T[] array)
		{
			for(int i = 0; i < array.Length; i++)
			{
				yield return new KeyValuePair<int, T>(i, array[i]);
			}
		}
		
		/// <summary>
		/// Creates an enumerable of one element.
		/// </summary>
		/// <param name="elem">The one element in this enumerable.</param>
		/// <returns>The enumerable.</returns>
		public static IEnumerable<TElement> Once<TElement>(TElement elem)
		{
			yield return elem;
		}
		
		public static IEnumerable<KeyValuePair<int, T>> PairEnumerate<T>(this IList<T> list)
		{
			for(int i = 0; i < list.Count; i++)
			{
				yield return new KeyValuePair<int, T>(i, list[i]);
			}
		}
		
		public static T[] Populate<T>(this T[] arr, T value)
		{
			int l = arr.Length;
			for(int i = 0; i < l; i++)
			{
				arr[i] = value;
			}
			return arr;
		}
		
		public static void Process<T>(this IList<T> list, Func<T,T> changeFunc)
		{
			for(int i = 0; i < list.Count; i++)
			{
				list[i] = changeFunc(list[i]);
			}
		}
		
		public static void Process<T>(this IList<T> list, Func<int,T,T> changeFunc)
		{
			for(int i = 0; i < list.Count; i++)
			{
				list[i] = changeFunc(i, list[i]);
			}
		}
		
		public static void Process<TKey,TValue>(this IDictionary<TKey,TValue> dictionary, Func<TKey,TValue,TValue> changeFunc)
		{
			foreach(TKey key in dictionary.Keys)
			{
				dictionary[key] = changeFunc(key, dictionary[key]);
			}
		}
		
		public static void ForEach(this IEnumerator enumerator, Action<object> each)
		{
			while(enumerator.MoveNext())
			{
				each(enumerator.Current);
			}
		}
		
		public static void ForEach<T>(this IEnumerator<T> enumerator, Action<T> each)
		{
			while(enumerator.MoveNext())
			{
				each(enumerator.Current);
			}
		}
		
		public static IEnumerable<T> ToIEnumerable<T>(this IEnumerable<T> enumerable)
		{
			return enumerable;
		}
		
		public static IEnumerable ToIEnumerable(this IEnumerator enumerator)
		{
			return new Enumerator(enumerator);
		}
		
		public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator)
		{
			return new Enumerator<T>(enumerator);
		}
		
		private class Enumerator : IEnumerable
		{
			private IEnumerator enumerator;
			
			public Enumerator(IEnumerator ienum)
			{
				enumerator = ienum;
			}
			
			public IEnumerator GetEnumerator()
			{
				return enumerator;
			}
		}
		
		private class Enumerator<T> : IEnumerable<T>
		{
			private IEnumerator<T> enumerator;
			
			public Enumerator(IEnumerator<T> ienum)
			{
				enumerator = ienum;
			}
			
			IEnumerator IEnumerable.GetEnumerator()
			{
				return enumerator;
			}
			
			public IEnumerator<T> GetEnumerator()
			{
				return enumerator;
			}
		}
		
		public static IEnumerable ToIEnumerable(this ArgIterator arglist)
		{
			return new ArgListEnumerable(arglist);
		}
		
		private class ArgListEnumerable : IEnumerable
		{
			private object Arglist;
			private object Initial;
			
			public ArgListEnumerable(ArgIterator arglist)
			{
				Arglist = UnsafeTools.Box(arglist);
				Initial = UnsafeTools.Box(arglist);
			}
			
			public IEnumerator GetEnumerator()
			{
				var enumerator = new ArgListEnumerator(Arglist);
				Arglist = Initial;
				return enumerator;
			}
			
			private class ArgListEnumerator : IEnumerator
			{
				private object Arglist;
				private object Initial;
				
				public ArgListEnumerator(object arglist)
				{
					Arglist = arglist;
					Initial = arglist;
				}
				
				private object current;
				private int state = -1;
				
				public object Current
				{
					get{
						if(state == -1)
						{
							throw new InvalidOperationException(GetResourceString("InvalidOperation_EnumNotStarted"));
						}else if(state == 1)
						{
							throw new InvalidOperationException(GetResourceString("InvalidOperation_EnumEnded"));
						}
						return current;
					}
				}
				
				public void Reset()
				{
					Arglist = Initial;
				}
				
				public bool MoveNext()
				{
					ArgIterator arglist = (ArgIterator)Arglist;
					state = 0;
					if(arglist.GetRemainingCount() == 0)
					{
						state = 1;
						return false;
					}
					current = TypedReference.ToObject(arglist.GetNextArg());
					Arglist = UnsafeTools.Box(arglist);
					return true;
				}
			}
		}
		
		public static IEnumerable Enumerate(this RuntimeArgumentHandle arglist)
		{
			return new ArgIterator(arglist).ToIEnumerable();
		}
		
		public static TTo FastCast<TFrom,TTo>(TFrom arg)
		{
			return __refvalue(__makeref(arg), TTo);
		}
		private static ResourceManager clr_resources;
		
		static Extensions()
		{
			clr_resources = new ResourceManager("mscorlib", Assembly.Load("mscorlib.dll"));
		}
		
		public static string GetResourceString(string key)
		{
			return clr_resources.GetString(key);
		}
		
		public static void Swap<T>(ref T a, ref T b)
		{
			T t = a;
			a = b;
			b = t;
		}
		
		public static IEnumerable<Tuple<object,object>> Combine(IEnumerable larg, IEnumerable rarg)
		{
			IEnumerator enum1 = larg.GetEnumerator();
			IEnumerator enum2 = rarg.GetEnumerator();
			while(enum1.MoveNext() && enum2.MoveNext())
			{
				yield return new Tuple<object,object>(enum1.Current, enum2.Current);
			}
		}
		
		public static IEnumerable<Tuple<T1,T2>> Combine<T1,T2>(IEnumerable<T1> larg, IEnumerable<T2> rarg)
		{
			IEnumerator<T1> enum1 = larg.GetEnumerator();
			IEnumerator<T2> enum2 = rarg.GetEnumerator();
			while(enum1.MoveNext() && enum2.MoveNext())
			{
				yield return new Tuple<T1,T2>(enum1.Current, enum2.Current);
			}
		}
	}
}