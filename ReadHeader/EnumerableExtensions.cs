using System;
using System.Collections.Generic;

namespace DiskHeader
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> Source, int Count)
            => Source.Split((index, item) => (index % Count) == 0);
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> Source, Func<T, bool> Predicate)
            => Source.Split((index, item) => Predicate(item));
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> Source, Func<int, T, bool> Predicate)
        {
            var items = new List<T>();
            var index = 0;
            foreach(var item in Source)
            {
                items.Add(item);
                ++index;

                if(Predicate(index, item))
                {
                    yield return items;
                    items.Clear();
                }
                if (items.Count > 0)
                    yield return items;
            }
        }
    }
}