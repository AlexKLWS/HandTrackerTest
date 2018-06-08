using System.Collections.Generic;

namespace Scripts.Utils
{
    public static class IListExtensions {
        /// <summary>
        /// Shuffles the element order of the specified list.
        /// Uses Unity random!
        /// </summary>
        public static void Shuffle<T>(this IList<T> ts) {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i) {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }
    }
}