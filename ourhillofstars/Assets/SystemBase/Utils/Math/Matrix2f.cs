using System.Runtime.InteropServices;
using UnityEngine;

namespace SystemBase.Utils
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix2f
    {
        [MarshalAs(UnmanagedType.R4)]
        public float m00;

        [MarshalAs(UnmanagedType.R4)]
        public float m01;

        [MarshalAs(UnmanagedType.R4)]
        public float m10;

        [MarshalAs(UnmanagedType.R4)]
        public float m11;

        public static Vector2 operator *(Matrix2f m, Vector2 v)
        {
            return new Vector2(m.m00 * v.x + m.m01 * v.y, m.m10 * v.x + m.m11 * v.y);
        }

        public float Determinant => m00 * m11 - m10 * m01;

        public Vector2 Col0 {
            set {
                m00 = value.x;
                m10 = value.y;
            }
        }
        public Vector2 Col1
        {
            set
            {
                m01 = value.x;
                m11 = value.y;
            }
        }
    }
}
