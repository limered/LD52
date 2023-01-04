using System;
using System.Diagnostics;

namespace SystemBase.Utils
{
    public static class TimingUtils
    {
        public static long TimeThisInMs(Action action)
        {
            var sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
    }
}
