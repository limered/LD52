using System.Runtime.InteropServices;

namespace SystemBase.Utils
{
    [StructLayout(LayoutKind.Sequential)]
    public class Matrix2d
    {
        [MarshalAs(UnmanagedType.R8)]
        public double m00;

        [MarshalAs(UnmanagedType.R8)]
        public double m01;

        [MarshalAs(UnmanagedType.R8)]
        public double m10;

        [MarshalAs(UnmanagedType.R8)]
        public double m11;

        public static Vector2d operator *(Matrix2d m, Vector2d v)
        {
            return new Vector2d(m.m00 * v.x + m.m01 * v.y, m.m10 * v.x + m.m11 * v.y);
        }

        public static Matrix2d operator *(double s, Matrix2d m)
        {
            return new Matrix2d { m00 = m.m00 * s, m01 = m.m01 * s, m10 = m.m10 * s, m11 = m.m11 * s };
        }

        public double Determinant => m00 * m11 - m10 * m01;

        public Matrix2d Inverse
        {
            get
            {
                var d = Determinant;
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (d == 0) return null;
                return 1 / d * new Matrix2d { m00 = m11, m01 = -m01, m10 = -m10, m11 = m00 };
            }
        }

        public Vector2d Col0
        {
            set
            {
                m00 = value.x;
                m10 = value.y;
            }
        }

        public Vector2d Col1
        {
            set
            {
                m01 = value.x;
                m11 = value.y;
            }
        }

        public override string ToString()
        {
            return m00 + " " + m01 + "\n" + m10 + " " + m11;
        }
    }
}
