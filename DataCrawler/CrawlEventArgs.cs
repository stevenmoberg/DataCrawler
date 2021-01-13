using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCrawler
{
    public delegate void CrawlEventHandler(CrawlEventArgs args);

    public class CrawlEventArgs : EventArgs
    {
        public CrawlEventArgs(PropertyBag properties)
        {
            Properties = properties;
        }

        public PropertyBag Properties { get; set; }
    }
}
