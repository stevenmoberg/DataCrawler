using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataCrawler
{
    public class Counter
    {
        public Counter(string name)
        {
            Name = name;
        }

        public string Name { get; }

        private long value = 0;
        public long Value => value;
        public long Increment() => Interlocked.Increment(ref this.value);
        public long Decrement() => Interlocked.Decrement(ref this.value);
        public void Reset() => Interlocked.Exchange(ref this.value, 0);

        public long Add(long amount) => Interlocked.Add(ref this.value, amount);

        public static implicit operator long(Counter counter) => counter.Value;
    }
}
