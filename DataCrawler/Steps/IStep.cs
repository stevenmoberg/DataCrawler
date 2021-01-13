using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCrawler.Steps
{
    public interface IStep
    {
        Task<bool> Process(ICrawler crawler, PropertyBag propertyBag);

        string Name { get; }
    }
}
