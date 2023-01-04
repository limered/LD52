namespace SystemBase.Utils
{
    // ReSharper disable once InconsistentNaming
    public class Matrix3d
    {
        public double[] Data = new double[9];

        public double GetAt(int col, int row)
        {
            var index = row * 3 + col;
            return Data[index];
        }

        public void SetAt(int col, int row, double value)
        {
            var index = row * 3 + col;
            Data[index] = value;
        }

        public static Matrix3d Zero()
        {
            return new Matrix3d
            {
                Data = new[]{
                0.0, 0.0, 0.0,
                0.0, 0.0, 0.0,
                0.0, 0.0, 0.0
            }
            };
        }

        public static Matrix3d Identity()
        {
            return new Matrix3d
            {
                Data = new[]{
                1.0, 0.0, 0.0,
                0.0, 1.0, 0.0,
                0.0, 0.0, 1.0
            }
            };
        }
    }
}
