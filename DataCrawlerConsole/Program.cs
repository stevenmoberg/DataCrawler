using System;
using System.Diagnostics;
using System.Timers;

namespace DataCrawler
{
    class Program
    {
        private static readonly Stopwatch sw = new Stopwatch();

        static void Main(string[] args)
        {
            var settings = new CrawlSettings();

            // need to set initial queue or lazy getmore helper
            // InputQueue.Instance.Enqueue

            var crawler = new Crawler(settings);

            crawler.CrawlProcessEvent += Crawler_CrawlProcessEvent;


            var timer = new Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Start();

            sw.Start();
            crawler.Crawl();

            do
            {
                // escape key?
            } while (Console.ReadKey().KeyChar != 'q');

            // TODO - need to await crawler stop
            crawler.Stop();
            timer.Stop();
            sw.Stop();

            WriteStats();
            // WriteStatsToFile();
            
            // print application stopped
            // press any key to close -------- if opened by double click??
            Console.ReadKey();
        }

        private static void WriteStats()
        {
            var running = sw.Elapsed;
            var (counts, durations) = CrawlStats.GetCountersAndDurations();

            // running:      0.00:00:00
            // remaining:    0.00:00:00 (estimate)
            // processed:    ###.##% - counts("Processed") of {total}, ## remain
            // speed:        x per minute, y per hour, estimate 
            //
            // success:      counts("Success")
            // failure:      counts("Failure")
            // total size:   counts("Download:Size") MB
            // avg size:     ##MB ---- counts("Download:Size")/success-count
            // avg download: 00:00:00  --- duration("Download")/counts("Download:Success")
            // avg upload:   00:00:00  --- duration("Upload")/counts("Upload:Success")
            //


            // press any key to quit
        }


        private static void Crawler_CrawlProcessEvent(CrawlEventArgs args)
        {
            // log status to output CSV file
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            WriteStats();
        }


    }
}
