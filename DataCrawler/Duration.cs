using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataCrawler
{
    public class Duration
    {
        public Duration(string name)
        {
            Name = name;
        }

        public string Name { get; }
        private long value = 0;
        public TimeSpan Value => TimeSpan.FromMilliseconds(value);
        public TimeSpan Add(TimeSpan duration) => TimeSpan.FromMilliseconds(Interlocked.Add(ref this.value, (long)duration.TotalMilliseconds));
        public void Reset() => Interlocked.Exchange(ref this.value, 0);

        public static implicit operator TimeSpan(Duration duration) => duration.Value;
    }
}
