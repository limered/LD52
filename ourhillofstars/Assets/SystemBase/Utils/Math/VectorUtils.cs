using UnityEngine;

namespace SystemBase.Utils
{
    public static class VectorUtils
    {
        // ReSharper disable once InconsistentNaming
        public static Vector2 XY(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector2Int XYInt(this Vector3 v)
        {
            return new Vector2Int((int) v.x, (int) v.y);
        }
        
        public static Vector2 XZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }
        
        public static Vector2Int XZInt(this Vector3 v)
        {
            return new Vector2Int((int) v.x, (int) v.z);
        }

        public static float Angle(this Vector2 vector)
        {
            return Angle(vector, Vector2.up);
        }

        public static float Angle(Vector2 vector, Vector2 zeroAngleNormal)
        {
            return Mathf.Deg2Rad * Vector2.Angle(zeroAngleNormal, vector.normalized);
        }

        public static Vector2 FromAngle(this float angle)
        {
            angle += 90 * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        public static float AngleBetweenPoints(Vector2 position1, Vector2 position2)
        {
            var fromLine = position2 - position1;
            var toLine = new Vector2(1, 0);

            var angle = Vector2.Angle(fromLine, toLine);
            var cross = Vector3.Cross(fromLine, toLine);

            if (cross.z > 0)
                angle = 360f - angle;

            return angle * Mathf.Deg2Rad;
        }

        public static Vector3 RandomVector(this Vector3 self)
        {
            return new Vector3(Random.value, Random.value, Random.value);
        }

        public static Vector3 RandomVector(this Vector3 self, Vector3 minVec, Vector3 maxVec)
        {
            return new Vector3(
                Random.value * (maxVec.x - minVec.x) + minVec.x,
                Random.value * (maxVec.y - minVec.y) + minVec.y,
                Random.value * (maxVec.z - minVec.z) + minVec.z);
        }

        public static Vector2 Project(this Vector3 input)
        {
            return new Vector2(input.x / input.z, input.y / input.z);
        }

        public static Vector3 Unproject(this Vector2 input)
        {
            return new Vector3(input.x, input.y, 1f);
        }

        public static float[] GetArray(this Vector3 v)
        {
            return new[] { v.x, v.y, v.z };
        }

        public static double[] ToArrayd(this Vector3 v)
        {
            return new double[] { v.x, v.y, v.z };
        }

        public static Vector3 FromArray(this Vector3 v, double[] arr)
        {
            v.x = (float)arr[0];
            v.y = (float)arr[1];
            v.z = (float)arr[2];
            return v;
        }

        public static float[] GetArray(this Vector2 v)
        {
            return new[] { v.x, v.y };
        }

        public static Vector3 FromArray(this Vector3 v, float[] arr)
        {
            v.Set(arr[0], arr[1], arr[2]);
            return v;
        }

        public static Vector3 DirectionTo(this Vector3 from, Vector3 to)
        {
            return (to - from).normalized;
        }

        public static float DistanceToSq(this Vector3 from, Vector3 to)
        {
            return (to - from).sqrMagnitude;
        }

        public static float DistanceTo(this Vector3 from, Vector3 to)
        {
            return (to - from).magnitude;
        }

        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            var sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            var cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            var tx = v.x;
            var ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }
    }
}