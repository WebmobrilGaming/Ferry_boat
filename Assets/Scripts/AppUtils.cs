using System.Collections.Generic;
using UnityEngine;

namespace AppUtils
{
    public static class ListExtensions
    {
        public static T Random<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0)
                throw new System.ArgumentException("List is empty or null");
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
    }
}
