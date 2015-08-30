/* Date: 1.8.2015, Time: 19:05 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IllidanS4.SharpUtils.Sequences
{
	public static class SequenceTools
	{
		/// <summary>
		/// Flattens an finite sequence of finite sequences of finite sequences etc.
		/// </summary>
		public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<IEnumerable> source)
		{
			if(source == null) yield break;
			foreach(var e in source)
			{
				var e2 = e as IEnumerable<IEnumerable>;
				if(e2 != null)
				{
					foreach(var o in SelectManyRecursive<T>(e2))
					{
						yield return o;
					}
				}else{
					var e3 = e as IEnumerable<T>;
					if(e3 != null)
					{
						foreach(var o in e3)
						{
							yield return o;
						}
					}else{
						throw new ArgumentException("Invalid elements in the sequence.", "source");
					}
				}
			}
		}
		
		/// <summary>
		/// Flattens an infinite sequence of infinite sequences of infinite sequences etc.
		/// </summary>
		public static IEnumerable<T> SelectManyRecursiveInfinite<T>(this IEnumerable<IEnumerable<IEnumerable>> source)
		{
			var inner = SelectManyInfinite(source);
			return SelectManyInfinite(inner.Select(SelectManyRecursiveInner<T>));
		}
		
		private static IEnumerable<T> SelectManyRecursiveInner<T>(IEnumerable source)
		{
			var enums = new List<IEnumerator<T>>();
			var senum = source.GetEnumerator();
			while(true)
			{
				bool next = false;
				if(senum.MoveNext())
				{
					var cur = senum.Current;
					if(cur != null)
					{
						var seq = cur as IEnumerable<T>;
						if(seq != null)
						{
							enums.Add(seq.GetEnumerator());
						}else{
							var inner = cur as IEnumerable<IEnumerable>;
							if(inner != null)
							{
								enums.Add(SelectManyInfinite(inner.Select(SelectManyRecursiveInner<T>)).GetEnumerator());
							}else{
								yield return (T)cur;
							}
						}
					}
					next = true;
				}
				foreach(var e in enums)
				{
					if(e.MoveNext())
					{
						yield return e.Current;
						next = true;
					}
				}
				if(!next) yield break;
			}
		}
		
		/// <summary>
		/// Flattens an infinite sequence of infinite sequences.
		/// </summary>
		public static IEnumerable<T> SelectManyInfinite<T>(this IEnumerable<IEnumerable<T>> source)
		{
			var enums = new List<IEnumerator<T>>();
			var senum = source.GetEnumerator();
			while(true)
			{
				bool next = false;
				if(senum.MoveNext())
				{
					var cur = senum.Current;
					if(cur != null)
						enums.Add(cur.GetEnumerator());
					next = true;
				}
				foreach(var e in enums)
				{
					if(e.MoveNext())
					{
						yield return e.Current;
						next = true;
					}
				}
				if(!next) yield break;
			}
		}
		
		/// <summary>
		/// Flattens a finite sequence of infinite sequences.
		/// </summary>
		public static IEnumerable<T> SelectManyOuter<T>(params IEnumerable<T>[] source)
		{
			return SelectManyOuter((IEnumerable<IEnumerable<T>>)source);
		}
		
		/// <summary>
		/// Flattens a finite sequence of infinite sequences.
		/// </summary>
		public static IEnumerable<T> SelectManyOuter<T>(this IEnumerable<IEnumerable<T>> source)
		{
			var enums = new List<IEnumerator<T>>();
			foreach(var e in source)
			{
				enums.Add(e.GetEnumerator());
			}
			while(true)
			{
				bool next = false;
				foreach(var e in enums)
				{
					if(e.MoveNext())
					{
						yield return e.Current;
						next = true;
					}
				}
				if(!next) yield break;
			}
		}
		
		public static ISequence<T> RepeatInfinite<T>(T value)
		{
			return Sequence.Infinite(RepeatInfiniteInner(value));
		}
		
		private static IEnumerable<T> RepeatInfiniteInner<T>(T value)
		{
			while(true) yield return value;
		}
	}
}
