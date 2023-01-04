using UnityEngine;

namespace SystemBase.Utils
{
    public static class MatrixExtensions
    {
        public static Matrix4x4 ScalarMultiply(this Matrix4x4 m, float scalar)
        {
            m.m00 *= scalar;
            m.m01 *= scalar;
            m.m02 *= scalar;
            m.m03 *= scalar;

            m.m10 *= scalar;
            m.m11 *= scalar;
            m.m12 *= scalar;
            m.m13 *= scalar;

            m.m20 *= scalar;
            m.m21 *= scalar;
            m.m22 *= scalar;
            m.m23 *= scalar;

            m.m30 *= scalar;
            m.m31 *= scalar;
            m.m32 *= scalar;
            m.m33 *= scalar;

            return m;
        }
        public static Quaternion ExtractRotation(this Matrix4x4 matrix)
        {
            Vector3 forward;
            forward.x = matrix.m02;
            forward.y = matrix.m12;
            forward.z = matrix.m22;

            Vector3 upwards;
            upwards.x = matrix.m01;
            upwards.y = matrix.m11;
            upwards.z = matrix.m21;

            return Quaternion.LookRotation(forward, upwards);
        }

        public static Matrix3d ExtractRotationMat(this Matrix4x4 m)
        {
            return new Matrix3d
            {
                Data = new double[]
                {
                    m.m00, m.m01, m.m02,
                    m.m10, m.m11, m.m12,
                    m.m20, m.m21, m.m22
                }
            };
        }

        public static Vector3 ExtractPosition(this Matrix4x4 m)
        {
            Vector3 position;
            position.x = m.m03;
            position.y = m.m13;
            position.z = m.m23;
            return position;
        }

        public static Matrix4x4 SetPosition(this Matrix4x4 m, Vector3 pos)
        {
            m.m03 = pos.x;
            m.m13 = pos.y;
            m.m23 = pos.z;
            return m;
        }

        public static Vector3 ExtractScale(this Matrix4x4 matrix)
        {
            Vector3 scale;
            scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
            scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
            scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
            return scale;
        }

        public static Matrix4x4 FromRotMatTransVec(this Matrix4x4 m, double[] rotation, double[] translation)
        {
            m.m00 = (float)rotation[0];
            m.m01 = (float)rotation[1];
            m.m02 = (float)rotation[2];

            m.m10 = (float)rotation[3];
            m.m11 = (float)rotation[4];
            m.m12 = (float)rotation[5];

            m.m20 = (float)rotation[6];
            m.m21 = (float)rotation[7];
            m.m22 = (float)rotation[8];

            m.m03 = (float)translation[0];
            m.m13 = (float)translation[1];
            m.m23 = (float)translation[2];

            m.m33 = 1f;

            return m;
        }

        public static float[] ToArray(this Matrix4x4 m)
        {
            return new[]
            {
                m.m00, m.m01, m.m02, m.m03,
                m.m10, m.m11, m.m12, m.m13,
                m.m20, m.m21, m.m22, m.m23,
                m.m30, m.m31, m.m32, m.m33
            };
        }

        public static Matrix4x4 FromArray(this Matrix4x4 m, float[] arr)
        {
            m.m00 = arr[0];
            m.m01 = arr[1];
            m.m02 = arr[2];
            m.m03 = arr[3];
            m.m10 = arr[4];
            m.m11 = arr[5];
            m.m12 = arr[6];
            m.m13 = arr[7];
            m.m20 = arr[8];
            m.m21 = arr[9];
            m.m22 = arr[10];
            m.m23 = arr[11];
            m.m30 = arr[12];
            m.m31 = arr[13];
            m.m32 = arr[14];
            m.m33 = arr[15];
            return m;
        }
        public static Matrix4x4 FromArray(this Matrix4x4 m, double[] arr)
        {
            m.m00 = (float)arr[0];
            m.m01 = (float)arr[1];
            m.m02 = (float)arr[2];
            m.m03 = (float)arr[3];
            m.m10 = (float)arr[4];
            m.m11 = (float)arr[5];
            m.m12 = (float)arr[6];
            m.m13 = (float)arr[7];
            m.m20 = (float)arr[8];
            m.m21 = (float)arr[9];
            m.m22 = (float)arr[10];
            m.m23 = (float)arr[11];
            m.m30 = (float)arr[12];
            m.m31 = (float)arr[13];
            m.m32 = (float)arr[14];
            m.m33 = (float)arr[15];
            return m;
        }
    }
}
