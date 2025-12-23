using System.Collections.Generic;

namespace ProphetAR
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<(T1, T2)> Zip<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second)
        {
            using IEnumerator<T1> e1 = first.GetEnumerator();
            using IEnumerator<T2> e2 = second.GetEnumerator();

            while (e1.MoveNext() && e2.MoveNext())
            {
                yield return (e1.Current, e2.Current);
            }
        }
    }
}