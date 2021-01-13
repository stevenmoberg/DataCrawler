using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCrawler
{
	public class PropertyBag
	{
		// A Hashtable to contain the properties in the bag
		private Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

		public object this[string name]
		{
			get
			{
				if (data.TryGetValue(name, out object value))
					return value;

				return null;
			}
			set
            {
				if (value == null)
					data.Remove(name);
				else
					data[name] = value;
            }
		}

		public Input Input { get; set; }
		public Output Output { get; set; }
		public Exception Exception { get; set; }

		public bool StopProcess { get; set; }
		public bool StopCrawler { get; set; }
	}
}
