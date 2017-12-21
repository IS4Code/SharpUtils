/* Date: 12.12.2017, Time: 20:15 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IllidanS4.SharpUtils.Collections.Streaming
{
	public static class Enumerator
	{
		public static IEnumerator<TSource> Where<TSource>(this IEnumerator<TSource> source, Func<TSource, bool> predicate)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			return WhereIterator<TSource>(source, predicate);
		}
		
		private static IEnumerator<TSource> WhereIterator<TSource>(IEnumerator<TSource> source, Func<TSource, bool> predicate)
		{
			while(source.MoveNext())
			{
				if(predicate(source.Current))
				{
					yield return source.Current;
				}
			}
		}
		
		public static IEnumerator<TSource> Where<TSource>(this IEnumerator<TSource> source, Func<TSource, int, bool> predicate)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			return WhereIterator<TSource>(source, predicate);
		}
		
		private static IEnumerator<TSource> WhereIterator<TSource>(IEnumerator<TSource> source, Func<TSource, int, bool> predicate)
		{
			int num = 0;
			while(source.MoveNext())
			{
				if(predicate(source.Current, num))
				{
					yield return source.Current;
				}
				checked{
					num++;
				}
			}
		}
		
		public static IEnumerator<TResult> Select<TSource, TResult>(this IEnumerator<TSource> source, Func<TSource, TResult> selector)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(selector == null)
			{
				throw new ArgumentNullException("selector");
			}
			return SelectIterator<TSource, TResult>(source, selector);
		}
		
		private static IEnumerator<TResult> SelectIterator<TSource, TResult>(this IEnumerator<TSource> source, Func<TSource, TResult> selector)
		{
			while(source.MoveNext())
			{
				yield return selector(source.Current);
			}
		}
		
		public static IEnumerator<TResult> Select<TSource, TResult>(this IEnumerator<TSource> source, Func<TSource, int, TResult> selector)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(selector == null)
			{
				throw new ArgumentNullException("selector");
			}
			return SelectIterator<TSource, TResult>(source, selector);
		}
		
		private static IEnumerator<TResult> SelectIterator<TSource, TResult>(this IEnumerator<TSource> source, Func<TSource, int, TResult> selector)
		{
			int num = 0;
			while(source.MoveNext())
			{
				yield return selector(source.Current, num);
				checked{
					num++;
				}
			}
		}
		
		public static IEnumerator<TResult> SelectMany<TSource, TResult>(this IEnumerator<TSource> source, Func<TSource, IEnumerator<TResult>> selector)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(selector == null)
			{
				throw new ArgumentNullException("selector");
			}
			
			return SelectManyIterator<TSource, TResult>(source, selector);
		}
		
		private static IEnumerator<TResult> SelectManyIterator<TSource, TResult>(IEnumerator<TSource> source, Func<TSource, IEnumerator<TResult>> selector)
		{
			while(source.MoveNext())
			{
				using(var enumerator = selector(source.Current))
				{
					while(enumerator.MoveNext())
					{
						yield return enumerator.Current;
					}
				}
			}
		}
		
		public static IEnumerator<TResult> SelectMany<TSource, TResult>(this IEnumerator<TSource> source, Func<TSource, int, IEnumerator<TResult>> selector)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(selector == null)
			{
				throw new ArgumentNullException("selector");
			}
			
			return SelectManyIterator<TSource, TResult>(source, selector);
		}
		
		private static IEnumerator<TResult> SelectManyIterator<TSource, TResult>(IEnumerator<TSource> source, Func<TSource, int, IEnumerator<TResult>> selector)
		{
			int num = 0;
			while(source.MoveNext())
			{
				using(var enumerator = selector(source.Current, num))
				{
					while(enumerator.MoveNext())
					{
						yield return enumerator.Current;
					}
				}
				checked{
					num++;
				}
			}
		}
		
		public static IEnumerator<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerator<TSource> source, Func<TSource, int, IEnumerator<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(collectionSelector == null)
			{
				throw new ArgumentNullException("collectionSelector");
			}
			if(resultSelector == null)
			{
				throw new ArgumentNullException("resultSelector");
			}
			
			return SelectManyIterator<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
		}
		
		private static IEnumerator<TResult> SelectManyIterator<TSource, TCollection, TResult>(IEnumerator<TSource> source, Func<TSource, int, IEnumerator<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			int num = 0;
			while(source.MoveNext())
			{
				using(var enumerator = collectionSelector(source.Current, num))
				{
					while(enumerator.MoveNext())
					{
						yield return resultSelector(source.Current, enumerator.Current);
					}
				}
				checked{
					num++;
				}
			}
		}
		
		public static IEnumerator<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerator<TSource> source, Func<TSource, IEnumerator<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(collectionSelector == null)
			{
				throw new ArgumentNullException("collectionSelector");
			}
			if(resultSelector == null)
			{
				throw new ArgumentNullException("resultSelector");
			}
			
			return SelectManyIterator<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
		}
		
		private static IEnumerator<TResult> SelectManyIterator<TSource, TCollection, TResult>(IEnumerator<TSource> source, Func<TSource, IEnumerator<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			while(source.MoveNext())
			{
				using(var enumerator = collectionSelector(source.Current))
				{
					while(enumerator.MoveNext())
					{
						yield return resultSelector(source.Current, enumerator.Current);
					}
				}
			}
		}
		
		public static IEnumerator<TSource> Take<TSource>(this IEnumerator<TSource> source, int count)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			return TakeIterator<TSource>(source, count);
		}
		
		private static IEnumerator<TSource> TakeIterator<TSource>(IEnumerator<TSource> source, int count)
		{
			while(count > 0)
			{
				if(!source.MoveNext())
				{
					break;
				}
				yield return source.Current;
				count--;
			}
		}
		
		public static IEnumerator<TSource> TakeWhile<TSource>(this IEnumerator<TSource> source, Func<TSource, bool> predicate)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			
			return TakeWhileIterator<TSource>(source, predicate);
		}
		
		private static IEnumerator<TSource> TakeWhileIterator<TSource>(IEnumerator<TSource> source, Func<TSource, bool> predicate)
		{
			while(source.MoveNext() && predicate(source.Current))
			{
				yield return source.Current;
			}
		}
		
		public static IEnumerator<TSource> TakeWhile<TSource>(this IEnumerator<TSource> source, Func<TSource, int, bool> predicate)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			
			return TakeWhileIterator<TSource>(source, predicate);
		}
		
		private static IEnumerator<TSource> TakeWhileIterator<TSource>(IEnumerator<TSource> source, Func<TSource, int, bool> predicate)
		{
			int num = 0;
			while(source.MoveNext() && predicate(source.Current, num))
			{
				yield return source.Current;
				checked{
					num++;
				}
			}
		}
		
		public static IEnumerator<TSource> Skip<TSource>(this IEnumerator<TSource> source, int count)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			return SkipIterator<TSource>(source, count);
		}
		
		private static IEnumerator<TSource> SkipIterator<TSource>(IEnumerator<TSource> source, int count)
		{
			while(count > 0)
			{
				if(!source.MoveNext())
				{
					yield break;
				}
				count--;
			}
			while(source.MoveNext())
			{
				yield return source.Current;
			}
		}
		
		public static IEnumerator<TSource> SkipWhile<TSource>(this IEnumerator<TSource> source, Func<TSource, bool> predicate)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			
			return SkipWhileIterator<TSource>(source, predicate);
		}
		
		private static IEnumerator<TSource> SkipWhileIterator<TSource>(IEnumerator<TSource> source, Func<TSource, bool> predicate)
		{
			bool flag;
			while((flag = source.MoveNext()) && predicate(source.Current))
			{
				
			}
			if(flag)
			{
				do{
					yield return source.Current;
				}while(source.MoveNext());
			}
		}
		
		public static IEnumerator<TSource> SkipWhile<TSource>(this IEnumerator<TSource> source, Func<TSource, int, bool> predicate)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			
			return SkipWhileIterator<TSource>(source, predicate);
		}
		
		private static IEnumerator<TSource> SkipWhileIterator<TSource>(IEnumerator<TSource> source, Func<TSource, int, bool> predicate)
		{
			int num = 0;
			bool flag;
			while((flag = source.MoveNext()) && predicate(source.Current, num))
			{
				checked{
					num++;
				}
			}
			if(flag)
			{
				do{
					yield return source.Current;
				}while(source.MoveNext());
			}
		}
		
		/*public static IEnumerator<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerator<TOuter> outer, IEnumerator<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
		{
			
		}
		
		public static IEnumerator<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerator<TOuter> outer, IEnumerator<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			
		}
		
		public static IEnumerator<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerator<TOuter> outer, IEnumerator<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerator<TInner>, TResult> resultSelector)
		{
			
		}
		
		public static IEnumerator<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerator<TOuter> outer, IEnumerator<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerator<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			
		}*/
		
		/*public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerator<TSource> source, Func<TSource, TKey> keySelector)
		{
			
		}
		
		public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerator<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			
		}
		
		public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerator<TSource> source, Func<TSource, TKey> keySelector)
		{
			
		}
		
		public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerator<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			
		}
		
		public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			
		}
		
		public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			
		}
		
		public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			
		}
		
		public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			
		}
		
		public static IEnumerator<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerator<TSource> source, Func<TSource, TKey> keySelector)
		{
			
		}
		
		public static IEnumerator<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerator<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			
		}
		
		public static IEnumerator<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerator<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			
		}
		
		public static IEnumerator<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerator<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			
		}*/
		
		/*public static IEnumerator<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerator<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerator<TSource>, TResult> resultSelector)
		{
			
		}
		
		public static IEnumerator<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerator<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerator<TElement>, TResult> resultSelector)
		{
			
		}
		
		public static IEnumerator<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerator<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerator<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			
		}
		
		public static IEnumerator<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerator<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerator<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			
		}
		
		public static IEnumerator<TSource> Concat<TSource>(this IEnumerator<TSource> first, IEnumerator<TSource> second)
		{
			
		}
		
		public static IEnumerator<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerator<TFirst> first, IEnumerator<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
		{
			
		}
		
		public static IEnumerator<TSource> Distinct<TSource>(this IEnumerator<TSource> source)
		{
			
		}
		
		public static IEnumerator<TSource> Distinct<TSource>(this IEnumerator<TSource> source, IEqualityComparer<TSource> comparer)
		{
			if(source == null)
			{
				throw new ArgumentNullException("first");
			}
			return DistinctIterator<TSource>(first, second, comparer);
		}
		
		public static IEnumerator<TSource> Union<TSource>(this IEnumerator<TSource> first, IEnumerator<TSource> second)
		{
			
		}
		
		public static IEnumerator<TSource> Union<TSource>(this IEnumerator<TSource> first, IEnumerator<TSource> second, IEqualityComparer<TSource> comparer)
		{
			if(first == null)
			{
				throw new ArgumentNullException("first");
			}
			if(second == null)
			{
				throw new ArgumentNullException("second");
			}
			return UnionIterator<TSource>(first, second, comparer);
		}
		
		public static IEnumerator<TSource> Intersect<TSource>(this IEnumerator<TSource> first, IEnumerator<TSource> second)
		{
			
		}
		
		public static IEnumerator<TSource> Intersect<TSource>(this IEnumerator<TSource> first, IEnumerator<TSource> second, IEqualityComparer<TSource> comparer)
		{
			if(first == null)
			{
				throw new ArgumentNullException("first");
			}
			if(second == null)
			{
				throw new ArgumentNullException("second");
			}
			return IntersectIterator<TSource>(first, second, comparer);
		}
		
		public static IEnumerator<TSource> Except<TSource>(this IEnumerator<TSource> first, IEnumerator<TSource> second)
		{
			if(first == null)
			{
				throw new ArgumentNullException("first");
			}
			if(second == null)
			{
				throw new ArgumentNullException("second");
			}
			return ExceptIterator<TSource>(first, second, null);
		}
		
		public static IEnumerator<TSource> Except<TSource>(this IEnumerator<TSource> first, IEnumerator<TSource> second, IEqualityComparer<TSource> comparer)
		{
			if(first == null)
			{
				throw new ArgumentNullException("first");
			}
			if(second == null)
			{
				throw new ArgumentNullException("second");
			}
			return ExceptIterator<TSource>(first, second, comparer);
		}*/
		
		public static IEnumerator<TSource> Reverse<TSource>(this IEnumerator<TSource> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			return ReverseIterator<TSource>(source);
		}
		
		private static IEnumerator<TSource> ReverseIterator<TSource>(IEnumerator<TSource> source)
		{
			var list = ToList(source);
			list.Reverse();
			return list.GetEnumerator();
		}
		
		public static bool SequenceEqual<TSource>(this IEnumerator<TSource> first, IEnumerator<TSource> second)
		{
			return first.SequenceEqual(second, null);
		}
		
		public static bool SequenceEqual<TSource>(this IEnumerator<TSource> first, IEnumerator<TSource> second, IEqualityComparer<TSource> comparer)
		{
			if(comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			if(first == null)
			{
				throw new ArgumentNullException("first");
			}
			if(second == null)
			{
				throw new ArgumentNullException("second");
			}
			
			while(first.MoveNext())
			{
				if(!second.MoveNext() || !comparer.Equals(first.Current, second.Current))
				{
					bool result = false;
					return result;
				}
			}
			if(second.MoveNext())
			{
				bool result = false;
				return result;
			}
			return true;
		}
		
		public static IEnumerator<TSource> AsEnumerable<TSource>(this IEnumerator<TSource> source)
		{
			return source;
		}
		
		public static TSource[] ToArray<TSource>(this IEnumerator<TSource> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			return ToList<TSource>(source).ToArray();
		}
		
		public static List<TSource> ToList<TSource>(this IEnumerator<TSource> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			var list = new List<TSource>();
			while(source.MoveNext())
			{
				list.Add(source.Current);
			}
			return list;
		}
		
		private static class IdentityFunction<TElement>
		{
	        public static readonly Func<TElement, TElement> Instance = x => x;
		}
		
		public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerator<TSource> source, Func<TSource, TKey> keySelector)
		{
			return ToDictionary(source, keySelector, IdentityFunction<TSource>.Instance, null);
		}
		
		public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerator<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			return ToDictionary(source, keySelector, IdentityFunction<TSource>.Instance, comparer);
		}
		
		public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerator<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return ToDictionary(source, keySelector, elementSelector, null);
		}
		
		public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerator<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			if(elementSelector == null)
			{
				throw new ArgumentNullException("elementSelector");
			}
			Dictionary<TKey, TElement> dictionary = new Dictionary<TKey, TElement>(comparer);
			while(source.MoveNext())
			{
				dictionary.Add(keySelector(source.Current), elementSelector(source.Current));
			}
			return dictionary;
		}
		
		/*public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerator<TSource> source, Func<TSource, TKey> keySelector)
		{
			return Lookup<TKey, TSource>.Create<TSource>(source, keySelector, IdentityFunction<TSource>.Instance, null);
		}
		
		public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerator<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			return Lookup<TKey, TSource>.Create<TSource>(source, keySelector, IdentityFunction<TSource>.Instance, comparer);
		}
		
		public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerator<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return Lookup<TKey, TElement>.Create<TSource>(source, keySelector, elementSelector, null);
		}
		
		public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerator<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			return Lookup<TKey, TElement>.Create<TSource>(source, keySelector, elementSelector, comparer);
		}*/
		
		public static IEnumerator<TSource> DefaultIfEmpty<TSource>(this IEnumerator<TSource> source)
		{
			return source.DefaultIfEmpty(default(TSource));
		}
		
		public static IEnumerator<TSource> DefaultIfEmpty<TSource>(this IEnumerator<TSource> source, TSource defaultValue)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			return DefaultIfEmptyIterator<TSource>(source, defaultValue);
		}
		
		private static IEnumerator<TSource> DefaultIfEmptyIterator<TSource>(IEnumerator<TSource> source, TSource defaultValue)
		{
			if(!source.MoveNext())
			{
				yield return default(TSource);
			}
			do{
				yield return source.Current;
			}while(source.MoveNext());
		}
		
		public static IEnumerator<TResult> OfType<TResult>(this IEnumerator source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			return OfTypeIterator<TResult>(source);
		}
		
		private static IEnumerator<TResult> OfTypeIterator<TResult>(IEnumerator source)
		{
			while(source.MoveNext())
			{
				if(source.Current is TResult)
				{
					yield return (TResult)source.Current;
				}
			}
		}
		
		public static IEnumerator<TResult> Cast<TResult>(this IEnumerator source)
		{
			IEnumerator<TResult> enumerator = source as IEnumerator<TResult>;
			if(enumerator != null)
			{
				return enumerator;
			}
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			return CastIterator<TResult>(source);
		}
		
		private static IEnumerator<TResult> CastIterator<TResult>(IEnumerator source)
		{
			while(source.MoveNext())
			{
				yield return (TResult)source.Current;
			}
		}
		
		public static TSource First<TSource>(this IEnumerator<TSource> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			if(source.MoveNext())
			{
				return source.Current;
			}
			throw Error.NoElements();
		}
		
		public static TSource First<TSource>(this IEnumerator<TSource> source, Func<TSource, bool> predicate)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			
			while(source.MoveNext())
			{
				if(predicate(source.Current))
				{
					return source.Current;
				}
			}
			throw Error.NoMatch();
		}
		
		public static TSource FirstOrDefault<TSource>(this IEnumerator<TSource> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			if(source.MoveNext())
			{
				return source.Current;
			}
			return default(TSource);
		}
		
		public static TSource FirstOrDefault<TSource>(this IEnumerator<TSource> source, Func<TSource, bool> predicate)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			
			while(source.MoveNext())
			{
				if(predicate(source.Current))
				{
					return source.Current;
				}
			}
			return default(TSource);
		}
		
		public static TSource Last<TSource>(this IEnumerator<TSource> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			if(source.MoveNext())
			{
				TSource current;
				do{
					current = source.Current;
				}while(source.MoveNext());
				return current;
			}
			throw Error.NoElements();
		}
		
		public static TSource Last<TSource>(this IEnumerator<TSource> source, Func<TSource, bool> predicate)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			
			TSource result = default(TSource);
			bool flag = false;
			while(source.MoveNext())
			{
				if(predicate(source.Current))
				{
					result = source.Current;
					flag = true;
				}
			}
			if(flag)
			{
				return result;
			}
			throw Error.NoMatch();
		}
		
		public static TSource LastOrDefault<TSource>(this IEnumerator<TSource> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			if(source.MoveNext())
			{
				TSource current = source.Current;
				do{
					current = source.Current;
				}while(source.MoveNext());
				return current;
			}
			return default(TSource);
		}
		
		public static TSource LastOrDefault<TSource>(this IEnumerator<TSource> source, Func<TSource, bool> predicate)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			
			TSource result = default(TSource);
			while(source.MoveNext())
			{
				if(predicate(source.Current))
				{
					result = source.Current;
				}
			}
			return result;
		}
		
		public static TSource Single<TSource>(this IEnumerator<TSource> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(!source.MoveNext())
			{
				throw Error.NoElements();
			}
			
			TSource current = source.Current;
			if(!source.MoveNext())
			{
				return current;
			}
			throw Error.MoreThanOneElement();
		}
		
		public static TSource Single<TSource>(this IEnumerator<TSource> source, Func<TSource, bool> predicate)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			
			TSource result = default(TSource);
			long num = 0;
			checked
			{
				while(source.MoveNext())
				{
					if(predicate(source.Current))
					{
						result = source.Current;
						checked{
							num += 1L;
						}
					}
				}
				if(num == 0)
				{
					throw Error.NoMatch();
				}
				if(num != 1)
				{
					throw Error.MoreThanOneMatch();
				}
				return result;
			}
		}
		
		public static TSource SingleOrDefault<TSource>(this IEnumerator<TSource> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			if(!source.MoveNext())
			{
				return default(TSource);
			}
			TSource current = source.Current;
			if(!source.MoveNext())
			{
				return current;
			}
			throw Error.MoreThanOneElement();
		}
		
		public static TSource SingleOrDefault<TSource>(this IEnumerator<TSource> source, Func<TSource, bool> predicate)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			
			TSource result = default(TSource);
			long num = 0;
			while(source.MoveNext())
			{
				if(predicate(source.Current))
				{
					result = source.Current;
					checked{
						num += 1L;
					}
				}
			}
			if(num == 0)
			{
				return default(TSource);
			}
			if(num != 1)
			{
				throw Error.MoreThanOneMatch();
			}
			return result;
		}
		
		public static TSource ElementAt<TSource>(this IEnumerator<TSource> source, int index)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			
			while(source.MoveNext())
			{
				if(index == 0)
				{
					return source.Current;
				}
				index--;
			}
			throw new ArgumentOutOfRangeException("index");
		}
		
		public static TSource ElementAtOrDefault<TSource>(this IEnumerator<TSource> source, int index)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			if(index >= 0)
			{
				while(source.MoveNext())
				{
					if(index == 0)
					{
						return source.Current;
					}
					index--;
				}
			}
			return default(TSource);
		}
		
		public static IEnumerator<int> Range(int start, int count)
		{
			long num = (long)start + count - 1;
			if(count < 0 || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			return RangeIterator(start, count);
		}
		
		private static IEnumerator<int> RangeIterator(int start, int count)
		{
			for(int i = 0; i < count; i++)
			{
				yield return start+i;
			}
		}
		
		public static IEnumerator<TResult> Repeat<TResult>(TResult element, int count)
		{
			if(count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			
			return RepeatIterator(element, count);
		}
		
		private static IEnumerator<TResult> RepeatIterator<TResult>(TResult element, int count)
		{
			for(int i = 0; i < count; i++)
			{
				yield return element;
			}
		}
		
		public static IEnumerator<TResult> Empty<TResult>()
		{
			yield break;
		}
		
		
		
		
		
		
		
		
		
		
		
		
		public static bool Any<TSource>(this IEnumerator<TSource> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			while(source.MoveNext())
			{
				return true;
			}
			return false;
		}
		
		public static bool Any<TSource>(this IEnumerator<TSource> source, Func<TSource, bool> predicate)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			
			while(source.MoveNext())
			{
				if(predicate(source.Current))
				{
					return true;
				}
			}
			return false;
		}
		
		public static bool All<TSource>(this IEnumerator<TSource> source, Func<TSource, bool> predicate)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			
			while(source.MoveNext())
			{
				if(!predicate(source.Current))
				{
					return false;
				}
			}
			return true;
		}
		
		public static int Count<TSource>(this IEnumerator<TSource> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			int num = 0;
			while(source.MoveNext())
			{
				checked{
					num++;
				}
			}
			return num;
		}
		
		public static int Count<TSource>(this IEnumerator<TSource> source, Func<TSource, bool> predicate)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			
			int num = 0;
			while(source.MoveNext())
			{
				if(predicate(source.Current))
				{
					checked{
						num++;
					}
				}
			}
			return num;
		}
		
		public static long LongCount<TSource>(this IEnumerator<TSource> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			long num = 0;
			while(source.MoveNext())
			{
				checked{
					num += 1;
				}
			}
			return num;
		}
		
		public static long LongCount<TSource>(this IEnumerator<TSource> source, Func<TSource, bool> predicate)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			
			long num = 0;
			while(source.MoveNext())
			{
				if(predicate(source.Current))
				{
					checked{
						num += 1;
					}
				}
			}
			return num;
		}
		
		public static bool Contains<TSource>(this IEnumerator<TSource> source, TSource value)
		{
			return source.Contains(value, null);
		}
		
		public static bool Contains<TSource>(this IEnumerator<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
		{
			if(comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			while(source.MoveNext())
			{
				if(comparer.Equals(source.Current, value))
				{
					return true;
				}
			}
			return false;
		}
		
		public static TSource Aggregate<TSource>(this IEnumerator<TSource> source, Func<TSource, TSource, TSource> func)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(func == null)
			{
				throw new ArgumentNullException("func");
			}
			if(!source.MoveNext())
			{
				throw Error.NoElements();
			}
			
			TSource tSource = source.Current;
			while(source.MoveNext())
			{
				tSource = func(tSource, source.Current);
			}
			return tSource;
		}
		
		public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerator<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(func == null)
			{
				throw new ArgumentNullException("func");
			}
			
			TAccumulate tAccumulate = seed;
			while(source.MoveNext())
			{
				tAccumulate = func(tAccumulate, source.Current);
			}
			return tAccumulate;
		}
		
		public static TResult Aggregate<TSource, TAccumulate, TResult>(this IEnumerator<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(func == null)
			{
				throw new ArgumentNullException("func");
			}
			if(resultSelector == null)
			{
				throw new ArgumentNullException("resultSelector");
			}
			
			TAccumulate tAccumulate = seed;
			while(source.MoveNext())
			{
				tAccumulate = func(tAccumulate, source.Current);
			}
			return resultSelector(tAccumulate);
		}
		
		public static int Sum(this IEnumerator<int> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(!source.MoveNext())
			{
				throw Error.NoElements();
			}
			
			int num = source.Current;
			while(source.MoveNext())
			{
				checked{
					num += source.Current;
				}
			}
			return num;
		}
		
		public static int? Sum(this IEnumerator<int?> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			int num = 0;
			while(source.MoveNext())
			{
				if(source.Current.HasValue)
				{
					checked{
						num += source.Current.GetValueOrDefault();
					}
				}
			}
			return num;
		}
		
		public static long Sum(this IEnumerator<long> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			long num = 0;
			while(source.MoveNext())
			{
				checked{
					num += source.Current;
				}
			}
			return num;
		}
		
		public static long? Sum(this IEnumerator<long?> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			long num = 0;
			while(source.MoveNext())
			{
				if(source.Current.HasValue)
				{
					checked{
						num += source.Current.GetValueOrDefault();
					}
				}
			}
			return num;
		}
		
		public static float Sum(this IEnumerator<float> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			double num = 0.0;
			while(source.MoveNext())
			{
				num += (double)source.Current;
			}
			return (float)num;
		}
		
		public static float? Sum(this IEnumerator<float?> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			double num = 0.0;
			while(source.MoveNext())
			{
				if(source.Current.HasValue)
				{
					num += (double)source.Current.GetValueOrDefault();
				}
			}
			return (float)num;
		}
		
		public static double Sum(this IEnumerator<double> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			double num = 0.0;
			while(source.MoveNext())
			{
				num += source.Current;
			}
			return num;
		}
		
		public static double? Sum(this IEnumerator<double?> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			double num = 0;
			while(source.MoveNext())
			{
				if(source.Current.HasValue)
				{
					num += source.Current.GetValueOrDefault();
				}
			}
			return num;
		}
		
		public static decimal Sum(this IEnumerator<decimal> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			decimal num = 0;
			while(source.MoveNext())
			{
				num += source.Current;
			}
			return num;
		}
		
		public static decimal? Sum(this IEnumerator<decimal?> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			decimal num = 0;
			while(source.MoveNext())
			{
				if(source.Current.HasValue)
				{
					num += source.Current.GetValueOrDefault();
				}
			}
			return new decimal?(num);
		}
		
		public static int Sum<TSource>(this IEnumerator<TSource> source, Func<TSource, int> selector)
		{
			return source.Select(selector).Sum();
		}
		
		public static int? Sum<TSource>(this IEnumerator<TSource> source, Func<TSource, int?> selector)
		{
			return source.Select(selector).Sum();
		}
		
		public static long Sum<TSource>(this IEnumerator<TSource> source, Func<TSource, long> selector)
		{
			return source.Select(selector).Sum();
		}
		
		public static long? Sum<TSource>(this IEnumerator<TSource> source, Func<TSource, long?> selector)
		{
			return source.Select(selector).Sum();
		}
		
		public static float Sum<TSource>(this IEnumerator<TSource> source, Func<TSource, float> selector)
		{
			return source.Select(selector).Sum();
		}
		
		public static float? Sum<TSource>(this IEnumerator<TSource> source, Func<TSource, float?> selector)
		{
			return source.Select(selector).Sum();
		}
		
		public static double Sum<TSource>(this IEnumerator<TSource> source, Func<TSource, double> selector)
		{
			return source.Select(selector).Sum();
		}
		
		public static double? Sum<TSource>(this IEnumerator<TSource> source, Func<TSource, double?> selector)
		{
			return source.Select(selector).Sum();
		}
		
		public static decimal Sum<TSource>(this IEnumerator<TSource> source, Func<TSource, decimal> selector)
		{
			return source.Select(selector).Sum();
		}
		
		public static decimal? Sum<TSource>(this IEnumerator<TSource> source, Func<TSource, decimal?> selector)
		{
			return source.Select(selector).Sum();
		}
		
		public static int Min(this IEnumerator<int> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(!source.MoveNext())
			{
				throw Error.NoElements();
			}
			
			int num = source.Current;
			while(source.MoveNext())
			{
				if(source.Current < num)
				{
					num = source.Current;
				}
			}
			return num;
		}
		
		public static int? Min(this IEnumerator<int?> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			int? num = null;
			while(source.MoveNext())
			{
				if(!num.HasValue || source.Current < num)
				{
					num = source.Current;
				}
			}
			return num;
		}
		
		public static long Min(this IEnumerator<long> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(!source.MoveNext())
			{
				throw Error.NoElements();
			}
			
			long num = source.Current;
			while(source.MoveNext())
			{
				if(source.Current < num)
				{
					num = source.Current;
				}
			}
			return num;
		}
		
		public static long? Min(this IEnumerator<long?> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			long? num = null;
			while(source.MoveNext())
			{
				if(!num.HasValue || source.Current < num)
				{
					num = source.Current;
				}
			}
			return num;
		}
		
		public static float Min(this IEnumerator<float> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			if(!source.MoveNext())
			{
				throw Error.NoElements();
			}
			float num = source.Current;
			while(source.MoveNext())
			{
				if(source.Current < num || System.Single.IsNaN(source.Current))
				{
					num = source.Current;
				}
			}
			return num;
		}
		
		public static float? Min(this IEnumerator<float?> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			float? num = null;
			while(source.MoveNext())
			{
				if(source.Current.HasValue && (!num.HasValue || source.Current < num || System.Single.IsNaN(source.Current.Value)))
				{
					num = source.Current;
				}
			}
			return num;
		}
		
		public static double Min(this IEnumerator<double> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(!source.MoveNext())
			{
				throw Error.NoElements();
			}
			
			double num = source.Current;
			while(source.MoveNext())
			{
				if(source.Current < num || Double.IsNaN(source.Current))
				{
					num = source.Current;
				}
			}
			return num;
		}
		
		public static double? Min(this IEnumerator<double?> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			double? num = null;
			while(source.MoveNext())
			{
				if(source.Current.HasValue && (!num.HasValue || source.Current < num || Double.IsNaN(source.Current.Value)))
				{
					num = source.Current;
				}
			}
			return num;
		}
		
		public static decimal Min(this IEnumerator<decimal> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(!source.MoveNext())
			{
				throw Error.NoElements();
			}
			
			decimal num = source.Current;
			while(source.MoveNext())
			{
				if(source.Current < num)
				{
					num = source.Current;
				}
			}
			return num;
		}
		
		public static decimal? Min(this IEnumerator<decimal?> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			decimal? num = null;
			while(source.MoveNext())
			{
				if(!num.HasValue || source.Current < num)
				{
					num = source.Current;
				}
			}
			return num;
		}
		
		public static TSource Min<TSource>(this IEnumerator<TSource> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(!source.MoveNext())
			{
				throw Error.NoElements();
			}
			
			TSource tSource = source.Current;
			Comparer<TSource> @default = Comparer<TSource>.Default;
			while(source.MoveNext())
			{
				if(@default.Compare(source.Current, tSource) < 0)
				{
					tSource = source.Current;
				}
			}
			return tSource;
		}
		
		public static int Min<TSource>(this IEnumerator<TSource> source, Func<TSource, int> selector)
		{
			return source.Select(selector).Min();
		}
		
		public static int? Min<TSource>(this IEnumerator<TSource> source, Func<TSource, int?> selector)
		{
			return source.Select(selector).Min();
		}
		
		public static long Min<TSource>(this IEnumerator<TSource> source, Func<TSource, long> selector)
		{
			return source.Select(selector).Min();
		}
		
		public static long? Min<TSource>(this IEnumerator<TSource> source, Func<TSource, long?> selector)
		{
			return source.Select(selector).Min();
		}
		
		public static float Min<TSource>(this IEnumerator<TSource> source, Func<TSource, float> selector)
		{
			return source.Select(selector).Min();
		}
		
		public static float? Min<TSource>(this IEnumerator<TSource> source, Func<TSource, float?> selector)
		{
			return source.Select(selector).Min();
		}
		
		public static double Min<TSource>(this IEnumerator<TSource> source, Func<TSource, double> selector)
		{
			return source.Select(selector).Min();
		}
		
		public static double? Min<TSource>(this IEnumerator<TSource> source, Func<TSource, double?> selector)
		{
			return source.Select(selector).Min();
		}
		
		public static decimal Min<TSource>(this IEnumerator<TSource> source, Func<TSource, decimal> selector)
		{
			return source.Select(selector).Min();
		}
		
		public static decimal? Min<TSource>(this IEnumerator<TSource> source, Func<TSource, decimal?> selector)
		{
			return source.Select(selector).Min();
		}
		
		public static TResult Min<TSource, TResult>(this IEnumerator<TSource> source, Func<TSource, TResult> selector)
		{
			return source.Select(selector).Min<TResult>();
		}
		
		public static int Max(this IEnumerator<int> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(!source.MoveNext())
			{
				throw Error.NoElements();
			}
			
			int num = source.Current;
			while(source.MoveNext())
			{
				if(source.Current > num)
				{
					num = source.Current;
				}
			}
			return num;
		}
		
		public static int? Max(this IEnumerator<int?> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			int? num = null;
			while(source.MoveNext())
			{
				if(!num.HasValue || source.Current > num)
				{
					num = source.Current;
				}
			}
			return num;
		}
		
		public static long Max(this IEnumerator<long> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(!source.MoveNext())
			{
				throw Error.NoElements();
			}
			
			long num = source.Current;
			while(source.MoveNext())
			{
				if(source.Current > num)
				{
					num = source.Current;
				}
			}
			return num;
		}
		
		public static long? Max(this IEnumerator<long?> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			long? num = null;
			while(source.MoveNext())
			{
				if(!num.HasValue || source.Current > num)
				{
					num = source.Current;
				}
			}
			return num;
		}
		
		public static double Max(this IEnumerator<double> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(!source.MoveNext())
			{
				throw Error.NoElements();
			}
			
			double num = source.Current;
			while(source.MoveNext())
			{
				if(source.Current > num || Double.IsNaN(num))
				{
					num = source.Current;
				}
			}
			return num;
		}
		
		public static double? Max(this IEnumerator<double?> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			double? num = null;
			while(source.MoveNext())
			{
				if(source.Current.HasValue && (!num.HasValue || source.Current > num || Double.IsNaN(num.Value)))
				{
					num = source.Current;
				}
			}
			return num;
		}
		
		public static float Max(this IEnumerator<float> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(!source.MoveNext())
			{
				throw Error.NoElements();
			}
			
			float num = source.Current;
			while(source.MoveNext())
			{
				if(source.Current > num || System.Single.IsNaN(num))
				{
					num = source.Current;
				}
			}
			return num;
		}
		
		public static float? Max(this IEnumerator<float?> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			float? num = null;
			while(source.MoveNext())
			{
				if(source.Current.HasValue && (!num.HasValue || source.Current > num || System.Single.IsNaN(num.Value)))
				{
					num = source.Current;
				}
			}
			return num;
		}
		
		public static decimal Max(this IEnumerator<decimal> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(!source.MoveNext())
			{
				throw Error.NoElements();
			}
			
			decimal num = source.Current;
			while(source.MoveNext())
			{
				if(source.Current > num)
				{
					num = source.Current;
				}
			}
			return num;
		}
		
		public static decimal? Max(this IEnumerator<decimal?> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			decimal? num = null;
			while(source.MoveNext())
			{
				if(!num.HasValue || source.Current > num)
				{
					num = source.Current;
				}
			}
			return num;
		}
		
		public static TSource Max<TSource>(this IEnumerator<TSource> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			if(!source.MoveNext())
			{
				throw Error.NoElements();
			}
			
			TSource tSource = source.Current;
			Comparer<TSource> @default = Comparer<TSource>.Default;
			while(source.MoveNext())
			{
				if(@default.Compare(source.Current, tSource) > 0)
				{
					tSource = source.Current;
				}
			}
			return tSource;
		}
		
		public static int Max<TSource>(this IEnumerator<TSource> source, Func<TSource, int> selector)
		{
			return source.Select(selector).Max();
		}
		
		public static int? Max<TSource>(this IEnumerator<TSource> source, Func<TSource, int?> selector)
		{
			return source.Select(selector).Max();
		}
		
		public static long Max<TSource>(this IEnumerator<TSource> source, Func<TSource, long> selector)
		{
			return source.Select(selector).Max();
		}
		
		public static long? Max<TSource>(this IEnumerator<TSource> source, Func<TSource, long?> selector)
		{
			return source.Select(selector).Max();
		}
		
		public static float Max<TSource>(this IEnumerator<TSource> source, Func<TSource, float> selector)
		{
			return source.Select(selector).Max();
		}
		
		public static float? Max<TSource>(this IEnumerator<TSource> source, Func<TSource, float?> selector)
		{
			return source.Select(selector).Max();
		}
		
		public static double Max<TSource>(this IEnumerator<TSource> source, Func<TSource, double> selector)
		{
			return source.Select(selector).Max();
		}
		
		public static double? Max<TSource>(this IEnumerator<TSource> source, Func<TSource, double?> selector)
		{
			return source.Select(selector).Max();
		}
		
		public static decimal Max<TSource>(this IEnumerator<TSource> source, Func<TSource, decimal> selector)
		{
			return source.Select(selector).Max();
		}
		
		public static decimal? Max<TSource>(this IEnumerator<TSource> source, Func<TSource, decimal?> selector)
		{
			return source.Select(selector).Max();
		}
		
		public static TResult Max<TSource, TResult>(this IEnumerator<TSource> source, Func<TSource, TResult> selector)
		{
			return source.Select(selector).Max<TResult>();
		}
		
		public static double Average(this IEnumerator<int> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			long num = 0;
			long num2 = 0;
			while(source.MoveNext())
			{
				checked{
					num += unchecked((long)source.Current);
					num2 += 1;
				}
			}
			if(num2 > 0)
			{
				return (double)num / (double)num2;
			}
			throw Error.NoElements();
		}
		
		public static double? Average(this IEnumerator<int?> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			long num = 0;
			long num2 = 0;
			checked
			{
				while(source.MoveNext())
				{
					if(source.Current.HasValue)
					{
						num += unchecked((long)source.Current.GetValueOrDefault());
						num2 += 1;
					}
				}
				if(num2 > 0)
				{
					return (double)num / (double)num2;
				}
				return null;
			}
		}
		
		public static double Average(this IEnumerator<long> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			long num = 0L;
			long num2 = 0L;
			checked
			{
				while(source.MoveNext())
				{
					num += source.Current;
					num2 += 1L;
				}
				if(num2 > 0L)
				{
					return (double)num / (double)num2;
				}
				throw Error.NoElements();
			}
		}
		
		public static double? Average(this IEnumerator<long?> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			long num = 0;
			long num2 = 0;
			while(source.MoveNext())
			{
				if(source.Current.HasValue)
				{
					checked{
						num += source.Current.GetValueOrDefault();
						num2 += 1;
					}
				}
			}
			if(num2 > 0)
			{
				return (double)num / (double)num2;
			}
			return null;
		}
		
		public static float Average(this IEnumerator<float> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			double num = 0;
			long num2 = 0;
			while(source.MoveNext())
			{
				checked{
					num += (double)source.Current;
					num2 += 1;
				}
			}
			if(num2 > 0)
			{
				return (float)(num / (double)num2);
			}
			throw Error.NoElements();
		}
		
		public static float? Average(this IEnumerator<float?> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			double num = 0;
			long num2 = 0;
			while(source.MoveNext())
			{
				if(source.Current.HasValue)
				{
					checked{
						num += (double)source.Current.GetValueOrDefault();
						num2 += 1L;
					}
				}
			}
			if(num2 > 0)
			{
				return (float)(num / (double)num2);
			}
			return null;
		}
		
		public static double Average(this IEnumerator<double> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			double num = 0;
			long num2 = 0;
			while(source.MoveNext())
			{
				checked{
					num += source.Current;
					num2 += 1;
				}
			}
			if(num2 > 0)
			{
				return num / (double)num2;
			}
			throw Error.NoElements();
		}
		
		public static double? Average(this IEnumerator<double?> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			double num = 0;
			long num2 = 0;
			while(source.MoveNext())
			{
				if(source.Current.HasValue)
				{
					checked{
						num += source.Current.GetValueOrDefault();
						num2 += 1;
					}
				}
			}
			if(num2 > 0)
			{
				return num / (double)num2;
			}
			return null;
		}
		
		public static decimal Average(this IEnumerator<decimal> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			decimal d = 0;
			long num = 0;
			while(source.MoveNext())
			{
				checked{
					d += source.Current;
					num += 1;
				}
			}
			if(num > 0)
			{
				return d / num;
			}
			throw Error.NoElements();
		}
		
		public static decimal? Average(this IEnumerator<decimal?> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException("source");
			}
			
			decimal d = 0;
			long num = 0;
			while(source.MoveNext())
			{
				if(source.Current.HasValue)
				{
					checked{
						d += source.Current.GetValueOrDefault();
						num += 1;
					}
				}
			}
			if(num > 0)
			{
				return d / num;
			}
			return null;
		}
		
		public static double Average<TSource>(this IEnumerator<TSource> source, Func<TSource, int> selector)
		{
			return source.Select(selector).Average();
		}
		
		public static double? Average<TSource>(this IEnumerator<TSource> source, Func<TSource, int?> selector)
		{
			return source.Select(selector).Average();
		}
		
		public static double Average<TSource>(this IEnumerator<TSource> source, Func<TSource, long> selector)
		{
			return source.Select(selector).Average();
		}
		
		public static double? Average<TSource>(this IEnumerator<TSource> source, Func<TSource, long?> selector)
		{
			return source.Select(selector).Average();
		}
		
		public static float Average<TSource>(this IEnumerator<TSource> source, Func<TSource, float> selector)
		{
			return source.Select(selector).Average();
		}
		
		public static float? Average<TSource>(this IEnumerator<TSource> source, Func<TSource, float?> selector)
		{
			return source.Select(selector).Average();
		}
		
		public static double Average<TSource>(this IEnumerator<TSource> source, Func<TSource, double> selector)
		{
			return source.Select(selector).Average();
		}
		
		public static double? Average<TSource>(this IEnumerator<TSource> source, Func<TSource, double?> selector)
		{
			return source.Select(selector).Average();
		}
		
		public static decimal Average<TSource>(this IEnumerator<TSource> source, Func<TSource, decimal> selector)
		{
			return source.Select(selector).Average();
		}
		
		public static decimal? Average<TSource>(this IEnumerator<TSource> source, Func<TSource, decimal?> selector)
		{
			return source.Select(selector).Average();
		}
		
		private static class Error
		{
			private static int[] Empty = new int[0];
			private static bool[] TwoFalse = new bool[2];
			
			public static Exception NoElements()
			{
				try{
					Empty.First();
					return null;
				}catch(Exception e)
				{
					return e;
				}
			}
			
			public static Exception NoMatch()
			{
				try{
					TwoFalse.First(b => b);
					return null;
				}catch(Exception e)
				{
					return e;
				}
			}
			
			public static Exception MoreThanOneElement()
			{
				try{
					TwoFalse.Single();
					return null;
				}catch(Exception e)
				{
					return e;
				}
			}
			
			public static Exception MoreThanOneMatch()
			{
				try{
					TwoFalse.Single(b => !b);
					return null;
				}catch(Exception e)
				{
					return e;
				}
			}
		}
	}
}
