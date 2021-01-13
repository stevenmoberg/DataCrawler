using DataCrawler.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCrawler
{
    public class CrawlSettings
    {
        public string UserAgent { get; set; } = "Mozilla/5.0 (Windows; U; Windows NT 6.1; rv:2.2) Gecko/20110201";
        public int NumberOfRetries { get; set; }
        public bool AutoSpeedLimit { get; set; }

        public int ThreadCount { get; set; } = 1;

        public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;

        /// <summary>
        /// HttpRequest timeout in seconds
        /// </summary>
        public int Timeout { get; set; }

        public IEnumerable<IStep> Steps { get; set; }
    }
}
