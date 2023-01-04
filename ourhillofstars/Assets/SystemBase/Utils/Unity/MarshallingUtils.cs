using System;
using System.Runtime.InteropServices;

namespace SystemBase.Utils
{
    public static class MarshallingUtils
    {
        public static void MarshalUnmananagedArray2Struct<T>(IntPtr unmanagedArray, int length, out T[] mangagedArray)
        {
            var size = Marshal.SizeOf(typeof(T));
            mangagedArray = new T[length];

            for (var i = 0; i < mangagedArray.Length; i++)
            {
                mangagedArray[i] = (T)Marshal.PtrToStructure(new IntPtr(unmanagedArray.ToInt64() + i * size), typeof(T));
            }
        }
    }
}
