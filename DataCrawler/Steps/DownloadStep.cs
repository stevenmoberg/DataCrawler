using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataCrawler.Steps
{
    public class DownloadStep : IStep
    {
		public string Name { get; } = "Download";

        public async Task<bool> Process(ICrawler crawler, PropertyBag propertyBag)
        {
			var input = propertyBag.Input;
			// TODO - gen download URL from input
			// TODO - shared auth session to be used by multiple threads
			// TODO - WebRequest should handle pooling so should be safe to create vs use single HttpClient instance
			
			var url = string.Empty; 
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

			// TODO - add GZip header to minimize download overhead

			try
			{
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)await request.GetResponseAsync())
				using (Stream downloadStream = httpWebResponse.GetResponseStream())
				using (MemoryStream ms = new MemoryStream())
				{
					if (downloadStream != null)
					{
						await downloadStream.CopyToAsync(ms);
					}

					CrawlStats.GetCounter(Name + ":Size").Add(ms.Length);
					propertyBag["pdf"] = ms.ToArray();
				}
			}
			catch (Exception ex)
			{
				propertyBag.Exception = ex;
			}

			return true;
		}
    }
}
