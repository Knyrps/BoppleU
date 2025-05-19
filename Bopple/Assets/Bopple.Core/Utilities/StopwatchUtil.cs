using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Bopple.Core.Utilities
{
    public static class StopwatchUtil
    {
        public static ConcurrentDictionary<string, Stopwatch> Stopwatches = new();

        public static Stopwatch StartNew(string identifier)
        {
            Stopwatch sw = new();
            sw.Start();

            return Stopwatches.AddOrUpdate(identifier, sw, (key, existingValue) => existingValue);
        }

        public static Stopwatch Get(string identifier)
        {
            if (Stopwatches.TryGetValue(identifier, out Stopwatch sw))
            {
                return sw;
            }

            LogUtil.LogWarning($"No stopwatch found for identifier: {identifier}");
            return null;
        }

        public static void Reset(string identifier)
        {
            if (Stopwatches.TryGetValue(identifier, out Stopwatch sw))
            {
                sw.Reset();
                return;
            }

            LogUtil.LogWarning($"No stopwatch found for identifier: {identifier}");
        }

        public static long Elapsed(string identifier)
        {
            if (Stopwatches.TryGetValue(identifier, out Stopwatch sw))
            {
                return sw.ElapsedMilliseconds;
            }

            LogUtil.LogWarning($"No stopwatch found for identifier: {identifier}");
            return -1;
        }

        public static long Stop(string identifier)
        {
            if (Stopwatches.TryGetValue(identifier, out Stopwatch sw))
            {
                sw.Stop();
                Stopwatches.Remove(identifier, out _);
                return sw.ElapsedMilliseconds;
            }

            LogUtil.LogWarning($"No stopwatch found for identifier: {identifier}");
            return -1;
        }
    }
}