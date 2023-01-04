using UnityEngine;

namespace SystemBase.Utils
{
    public static class QuaternionExtensions
    {
        public static float[] ToArray(this Quaternion q)
        {
            return new[] { q.x, q.y, q.z, q.w };
        }

        public static Quaternion FromArray(this Quaternion q, float[] arr)
        {
            q.Set(arr[0], arr[1], arr[2], arr[3]);
            return q;
        }
    }
}
