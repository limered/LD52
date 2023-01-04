using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SystemBase.Utils
{
    public static class LinqExtensions
    {
        public static T NthElement<T>(this IEnumerable<T> coll, int n)
        {
            return coll.OrderBy(x => x).Skip(n - 1).FirstOrDefault();
        }

        public static List<T> Randomize<T>(this T[] list)
        {
            var result = new List<T>(list.Length);
            foreach (var item in list)
            {
                var rnd = (int) (Random.value * result.Count);
                result.Insert(rnd, item);
            }

            return result;
        }

        public static List<T> Randomize<T>(this List<T> list)
        {
            var result = new List<T>(list.Count);
            while (list.Count > 0)
            {
                var rnd = (int) (Random.value * list.Count);
                result.Add(list[rnd]);
                list.RemoveAt(rnd);
            }

            return result;
        }

        public static IEnumerable<T> AddDefaultCount<T>(this IEnumerable<T> coll, int n) where T : new()
        {
            var result = new List<T>();
            for (var i = 0; i < n; i++) result.Add(new T());

            return coll.Concat(result);
        }

        public static void Print<T>(this IEnumerable<T> list)
        {
            foreach (var element in list) Debug.Log(element);
        }

        public static void Print<T>(this IEnumerable<T> list, Func<T, string> returnStringToPrint)
        {
            foreach (var element in list) Debug.Log(returnStringToPrint(element));
        }
    }
}