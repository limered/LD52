using System;
using System.Collections.Generic;
using System.Diagnostics;
using UniRx;

namespace SystemBase.Utils
{
    public class StopWatcher
    {
        public readonly ReactiveProperty<Tuple<string, float>> onTimerFlushed = new();

        public readonly Dictionary<string, Stopwatch> timers = new();
        public readonly Dictionary<string, RingBuffer<long>> buffers = new();

        public void InitializeTimers(IEnumerable<string> timerNames, int bufferLength)
        {
            foreach (var name in timerNames)
            {
                timers.Add(name, new Stopwatch());
                buffers.Add(name, new RingBuffer<long>(bufferLength));
            }
        }

        public void StartTimer(string name)
        {
            if (!timers.TryGetValue(name, out var timer)) return;

            timer.Reset();
            timer.Start();
        }

        public void StopTimer(string name)
        {
            if (timers.TryGetValue(name, out var timer))
            {
                timer.Stop();
            }
        }

        public void SafeTimeToBuffer(string name)
        {
            if (timers.TryGetValue(name, out var timer) && buffers.TryGetValue(name, out var buffer))
            {
                buffer.Add(timer.ElapsedMilliseconds);
            }
        }

        public void FlushTimer(string name, bool fromBuffer = false)
        {
            if (!fromBuffer)
            {
                if (!timers.TryGetValue(name, out var timer)) return;

                onTimerFlushed.Value = new Tuple<string, float>(name, timer.ElapsedMilliseconds);
            }
            else
            {
                if (!buffers.TryGetValue(name, out var buffer)) return;

                long time = 0;
                for (var i = 0; i < buffer.Capacity; i++)
                {
                    time += buffer[i];
                }
                onTimerFlushed.Value = new Tuple<string, float>(name, (float)time / buffer.Capacity);
            }
        }

        public float GetTime(string name, bool fromBuffer = false)
        {
            if (!fromBuffer)
            {
                return !timers.TryGetValue(name, out var timer) ? 0 : timer.ElapsedMilliseconds;
            }

            if (!buffers.TryGetValue(name, out var buffer)) return 0;

            long time = 0;
            for (var i = 0; i < buffer.Capacity; i++)
            {
                time += buffer[i];
            }
            return (float)time / buffer.Capacity;
        }

        public void FlushAll(bool fromBuffer)
        {
            foreach (var name in timers.Keys)
            {
                FlushTimer(name, fromBuffer);
            }
        }
    }
}
