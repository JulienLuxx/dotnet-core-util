using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Util
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// DistinctBy
        /// </summary>
        /// <typeparam name="TSource">TheSourceWantToDistinct</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static string JoinString(this IEnumerable<string> source)
        {
            var str = string.Empty;
            if (source.Count() == 1)
            {
                str = source.First();
            }
            else
            {
                foreach (var item in source)
                {
                    str += item + ",";
                }
                str = str.Remove(str.Length - 1);
            }
            return str;
        }
    }
}
