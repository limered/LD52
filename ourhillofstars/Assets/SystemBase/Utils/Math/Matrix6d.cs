using System.Runtime.InteropServices;

namespace SystemBase.Utils
{
    // ReSharper disable once InconsistentNaming
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix6d
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
        public double[] Data;

        public double Get(int row, int col)
        {
            var index = row * 6 + col;
            return Data[index];
        }

        public void Set(int row, int col, double value)
        {
            var index = row * 6 + col;
            Data[index] = value;
        }
    }
}
