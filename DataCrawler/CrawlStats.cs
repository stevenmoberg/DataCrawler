using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCrawler
{
    public static class CrawlStats
    {
        private static readonly Dictionary<string, Counter> _counters = new Dictionary<string, Counter>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, Duration> _durations = new Dictionary<string, Duration>(StringComparer.OrdinalIgnoreCase);
        private static readonly object sync = new object();

        public static Counter GetCounter(string name)
        {
            Counter value;
            lock (sync)
                if (!_counters.TryGetValue(name, out value))
                     _counters[name] = value = new Counter(name);

            return value;
        }

        public static Duration GetDuration(string name)
        {
            Duration value;
            lock (sync)
                if (!_durations.TryGetValue(name, out value))
                    _durations[name] = value = new Duration(name);

            return value;
        }

        /*
        public static Dictionary<string, long> GetCounters()
        {
            lock (sync)
                return _counters.Values.ToDictionary(c => c.Name, c => c.Value);
        }

        public static Dictionary<string, TimeSpan> GetDurations()
        {
            lock (sync)
                return _durations.Values.ToDictionary(c => c.Name, c => c.Value);
        }
        */

        public static (Dictionary<string, long> counters, Dictionary<string, TimeSpan> durations) GetCountersAndDurations()
        {
            lock (sync)
                return (_counters.Values.ToDictionary(c => c.Name, c => c.Value), _durations.Values.ToDictionary(c => c.Name, c => c.Value));
        }

    }
}
