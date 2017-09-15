using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Resources;
using IllidanS4.SharpUtils.Reflection.Linq;
using IllidanS4.SharpUtils.Unsafe;

namespace IllidanS4.SharpUtils
{
	/// <summary>
	/// This class contains various useful methods and extension methods.
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Enumerates an array, returning the (index, value) pairs.
		/// </summary>
		/// <param name="array">The array to enumerate.</param>
		/// <returns>An enumerable of the pairs.</returns>
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
		
		/// <summary>
		/// Enumerates a list, returning the (index, value) pairs.
		/// </summary>
		/// <param name="list">The list to enumerate.</param>
		/// <returns>An enumerable of the pairs.</returns>
		public static IEnumerable<KeyValuePair<int, T>> PairEnumerate<T>(this IList<T> list)
		{
			for(int i = 0; i < list.Count; i++)
			{
				yield return new KeyValuePair<int, T>(i, list[i]);
			}
		}
		
		/// <summary>
		/// Fills an array with a specific value.
		/// </summary>
		/// <param name="arr">The array to populate.</param>
		/// <param name="value">The value to set each array element to.</param>
		/// <returns>The same array as passed in <paramref name="arr"/>.</returns>
		public static T[] Populate<T>(this T[] arr, T value)
		{
			int l = arr.Length;
			for(int i = 0; i < l; i++)
			{
				arr[i] = value;
			}
			return arr;
		}
		
		/// <summary>
		/// Processes each element in a list with a specifiec function and sets it to the return value.
		/// </summary>
		/// <param name="list">The list to process.</param>
		/// <param name="changeFunc">The processing function.</param>
		public static void Process<T>(this IList<T> list, Func<T,T> changeFunc)
		{
			for(int i = 0; i < list.Count; i++)
			{
				list[i] = changeFunc(list[i]);
			}
		}
		
		/// <summary>
		/// Processes each element in a list with a specifiec function and sets it to the return value.
		/// </summary>
		/// <param name="list">The list to process.</param>
		/// <param name="changeFunc">The processing function.</param>
		public static void Process<T>(this IList<T> list, Func<int,T,T> changeFunc)
		{
			for(int i = 0; i < list.Count; i++)
			{
				list[i] = changeFunc(i, list[i]);
			}
		}
		
		/// <summary>
		/// Processes each element in a dictionary with a specifiec function and sets it to the return value.
		/// </summary>
		/// <param name="dictionary">The dictionary to process.</param>
		/// <param name="changeFunc">The processing function.</param>
		public static void Process<TKey,TValue>(this IDictionary<TKey,TValue> dictionary, Func<TKey,TValue,TValue> changeFunc)
		{
			foreach(TKey key in dictionary.Keys)
			{
				dictionary[key] = changeFunc(key, dictionary[key]);
			}
		}
		
		/// <summary>
		/// Automatically enumerates an enumerator and runs an action for each returned object.
		/// </summary>
		/// <param name="enumerator">The enumerator to enumerate.</param>
		/// <param name="each">The action to call for each object returned by the enumerator.</param>
		public static void ForEach(this IEnumerator enumerator, Action<object> each)
		{
			while(enumerator.MoveNext())
			{
				each(enumerator.Current);
			}
		}
		
		/// <summary>
		/// Automatically enumerates an enumerator and runs an action for each returned object.
		/// </summary>
		/// <param name="enumerator">The enumerator to enumerate.</param>
		/// <param name="each">The action to call for each object returned by the enumerator.</param>
		public static void ForEach<T>(this IEnumerator<T> enumerator, Action<T> each)
		{
			while(enumerator.MoveNext())
			{
				each(enumerator.Current);
			}
		}
		
		/// <summary>
		/// Performs a cast on an enumerable object, returning its interface representation without changing its value.
		/// </summary>
		/// <param name="enumerable">The enumerable object.</param>
		/// <returns>The same object.</returns>
		public static IEnumerable<T> ToIEnumerable<T>(this IEnumerable<T> enumerable)
		{
			return enumerable;
		}
		
		/// <summary>
		/// Wraps an enumerator in an enumerable object.
		/// </summary>
		/// <param name="enumerator">The enumerator to wrap.</param>
		/// <returns>The object that can be enumerated.</returns>
		[Obsolete("The resulting object doesn't fulfill the IEnumerable contract correctly. Do not use this for iterating an enumerator.")]
		public static IEnumerable ToIEnumerable(this IEnumerator enumerator)
		{
			return new Enumerator(enumerator);
		}
		
		/// <summary>
		/// Wraps an enumerator in an enumerable object.
		/// </summary>
		/// <param name="enumerator">The enumerator to wrap.</param>
		/// <returns>The object that can be enumerated.</returns>
		[Obsolete("The resulting object doesn't fulfill the IEnumerable contract correctly. Do not use this for iterating an enumerator.")]
		public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator)
		{
			return new Enumerator<T>(enumerator);
		}
		
		class Enumerator : IEnumerable
		{
			readonly IEnumerator enumerator;
			
			public Enumerator(IEnumerator ienum)
			{
				enumerator = ienum;
			}
			
			public IEnumerator GetEnumerator()
			{
				return enumerator;
			}
		}
		
		class Enumerator<T> : IEnumerable<T>
		{
			readonly IEnumerator<T> enumerator;
			
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
		
		/// <summary>
		/// Converts a vararg iterator to an enumerable.
		/// </summary>
		/// <param name="arglist">The iterator to convert.</param>
		/// <returns>The enumerable object.</returns>
		public static IEnumerable ToIEnumerable(this ArgIterator arglist)
		{
			return new ArgListEnumerable(arglist);
		}
		
		class ArgListEnumerable : IEnumerable
		{
			object Arglist;
			readonly object Initial;
			
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
			
			class ArgListEnumerator : IEnumerator
			{
				object Arglist;
				object Initial;
				
				public ArgListEnumerator(object arglist)
				{
					Arglist = arglist;
					Initial = arglist;
				}
				
				object current;
				int state = -1;
				
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
		
		/// <summary>
		/// Creates an enumerable object from a varargs argument.
		/// </summary>
		/// <param name="arglist">The handle to the arglist.</param>
		/// <returns>The enumerable object.</returns>
		public static IEnumerable Enumerate(this RuntimeArgumentHandle arglist)
		{
			return new ArgIterator(arglist).ToIEnumerable();
		}
		
		/// <summary>
		/// Performs a direct cast on the <typeparamref name="TFrom"/> argument to the same type, represented by <typeparamref name="TTo"/>.
		/// </summary>
		/// <param name="arg">The argument to cast.</param>
		/// <returns>The same value, expressed using <typeparamref name="TTo"/>.</returns>
		/// <exception cref="System.InvalidCastException">This exception is thrown when <typeparamref name="TFrom"/> and <typeparamref name="TTo"/> do not represent the same type.</exception>
		public static TTo FastCast<TFrom,TTo>(TFrom arg)
		{
			return __refvalue(__makeref(arg), TTo);
		}
		static ResourceManager clr_resources;
		
		static Extensions()
		{
			clr_resources = new ResourceManager("mscorlib", Assembly.Load("mscorlib.dll"));
		}
		
		/// <summary>
		/// Returns a mscorlib.dll resource string from its key.
		/// </summary>
		/// <param name="key">The resource key.</param>
		/// <returns>The resource text.</returns>
		public static string GetResourceString(string key)
		{
			return clr_resources.GetString(key);
		}
		
		/// <summary>
		/// Swaps the values of two variables.
		/// </summary>
		/// <param name="a">The first variable.</param>
		/// <param name="b">The second variable.</param>
		public static void Swap<T>(ref T a, ref T b)
		{
			T t = a;
			a = b;
			b = t;
		}
		
		/// <summary>
		/// Combines two enumerables, producing a pair enumerable.
		/// </summary>
		/// <param name="larg">The left enumerable.</param>
		/// <param name="rarg">The right.</param>
		/// <returns>The pair enumerable.</returns>
		public static IEnumerable<Tuple<object,object>> Combine(IEnumerable larg, IEnumerable rarg)
		{
			IEnumerator enum1 = larg.GetEnumerator();
			IEnumerator enum2 = rarg.GetEnumerator();
			while(enum1.MoveNext() && enum2.MoveNext())
			{
				yield return new Tuple<object,object>(enum1.Current, enum2.Current);
			}
		}
		
		/// <summary>
		/// Combines two enumerables, producing a pair enumerable.
		/// </summary>
		/// <param name="larg">The left enumerable.</param>
		/// <param name="rarg">The right.</param>
		/// <returns>The pair enumerable.</returns>
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