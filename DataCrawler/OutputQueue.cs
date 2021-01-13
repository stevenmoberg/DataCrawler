using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCrawler
{
    public class OutputQueue : ProcessQueue<Output>
    {
        private OutputQueue() 
        { }

        public static OutputQueue Instance => Nested.Inner;

        private static class Nested
        {
            internal static readonly OutputQueue Inner = new OutputQueue();
        }
    }
}
