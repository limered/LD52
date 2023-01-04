using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace SystemBase.Utils
{
    public static class MathExtensions
    {
        public static bool Approx(this float v1, float v2)
        {
            return Mathf.Approximately(v1, v2);
        }

        public static bool Approx(this Vector3 v1, Vector3 v2)
        {
            return Mathf.Approximately(v1.x, v2.x) && Mathf.Approximately(v1.z, v2.y) && Mathf.Approximately(v1.z, v2.z);
        }
        
        public static float Max(this Vector3 v)
        {
            return Mathf.Max(v.x, v.y, v.z);
        }

        public static List<int> RandomShuffleInt(this List<int> list)
        {
            var rand = new Random();
            var n = list.Last() - list.First();
            for (var i = n - 1; i > 0; i--)
            {
                Swap(list, i, rand.Next(i+1));
            }
            return list;
        }

        public static void Swap<T>(List<T> list, int first, int second)
        {
            (list[first], list[second]) = (list[second], list[first]);
        }
    }
}