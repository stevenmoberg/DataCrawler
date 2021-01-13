using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataCrawler
{
    // https://github.com/BruceDone/awesome-crawler
    // https://github.com/lei-zhu/SimpleCrawler

    public class Crawler : ICrawler, IDisposable
    {
        private readonly CookieContainer cookieContainer;
        private readonly Random random;
        private readonly bool[] threadStatus;
        private readonly Thread[] threads;
        private bool disposed;
        private bool stopping;
        private System.Timers.Timer timer = null;

        // public event DataReceviedEventHandler DataReceivedEvent;
        public event CrawlEventHandler CrawlErrorEvent;
        public event CrawlEventHandler CrawlSuccessEvent;
        public event CrawlEventHandler CrawlProcessEvent;


        public Crawler(CrawlSettings settings)
        {
            this.Settings = settings;
            this.cookieContainer = new CookieContainer();
            this.random = new Random();

            this.threads = new Thread[settings.ThreadCount];
            this.threadStatus = new bool[settings.ThreadCount];
        }

        public CrawlSettings Settings { get; private set; }


        public void Crawl()
        {
            Initialize();

            // if limit by time
            if (Settings.Timeout > 0)
            {
                timer = new System.Timers.Timer(Settings.Timeout * 1000);
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
            }

            for (var i=0; i < this.threads.Length; i++)
            {
                threads[i].Start();
                threadStatus[i] = false;
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // TODO - logging
            Stop();
        }

        public void Stop()
        {
            stopping = true;

            // abort or just let them stop gracefully?
            foreach (Thread thread in this.threads)
                thread.Abort();
        }


        private void Initialize()
        {
            ServicePointManager.DefaultConnectionLimit = 256;

            for (int i = 0; i < this.Settings.ThreadCount; i++)
            {
                // var threadStart = new ParameterizedThreadStart(this.CrawlProcess);
                this.threads[i] = new Thread(async () => await CrawlProcess(i));
            }
        }

        private async Task<bool> CrawlProcess(object threadIndex)
        {
            Input input;
            var currentThreadIndex = (int)threadIndex;
            var sw = new Stopwatch();
            var sw2 = new Stopwatch();
            string currentStep = string.Empty;

            while (!stopping)
            {
                // TODO -- channels?
                if (!InputQueue.Instance.TryDequeue(out input))
                {
                    // allow inputQueue to be feed
                    threadStatus[currentThreadIndex] = true;

                    // if all threads are empty then end crawler thread
                    if (!threadStatus.Any(t => t == false))
                        break;

                    Thread.Sleep(2000);
                    continue;
                }

                this.threadStatus[currentThreadIndex] = false;
                if (!InputQueue.Instance.TryDequeue(out input))
                    continue;


                if (Settings.AutoSpeedLimit)
                {
                    // allow for throttleing
                    var span = this.random.Next(1000, 5000);
                    Thread.Sleep(span);
                }

                var pb = new PropertyBag();
                pb.Input = input;


                try
                {
                    sw.Restart();
                    // Step) PROCESS configured steps
                    foreach (var step in Settings.Steps)
                    {
                        currentStep = step.Name;
                        sw2.Restart();
                        await step.Process(this, pb);
                        CrawlStats.GetDuration(currentStep).Add(sw2.Elapsed);
                        
                        if (pb.Exception != null)
                            break;

                        CrawlStats.GetCounter(currentStep + ":Success").Increment(); // success
                        if (pb.StopProcess)
                            break; // what else should we do here?                        
                    }
                }
                catch (Exception ex)
                {
                    CrawlStats.GetDuration(currentStep).Add(sw2.Elapsed);
                    pb.Exception = ex;
                }

                if (pb.Exception != null)
                {
                    CrawlStats.GetCounter("Failure").Increment();
                    CrawlStats.GetCounter(currentStep + ":Failure").Increment();
                    CrawlErrorEvent?.Invoke(new CrawlEventArgs(pb));
                    // Log Error
                }
                else if (pb.StopProcess)
                {
                    // Log early cancelation, missing steps
                   
                }
                else
                {
                    CrawlStats.GetCounter("Success").Increment();
                    CrawlSuccessEvent?.Invoke(new CrawlEventArgs(pb));
                }

                CrawlStats.GetCounter("Processed").Increment();
                // TODO - since the arg contains exception and stop data, no need for dedicated success/failure events
                CrawlProcessEvent?.Invoke(new CrawlEventArgs(pb));


                if (pb.StopCrawler)
                {
                    this.Stop();
                    break;
                }
            }

            return true;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (timer != null)
                {
                    timer.Elapsed -= Timer_Elapsed;
                    timer.Dispose();
                    timer = null;
                }
            }

            disposed = true;
        }

    }
}
