using System.Collections.Generic;
using System.Threading;

namespace DataCrawler
{
    public class ProcessQueue<T> where T : class
    {
        protected readonly Queue<T> InnerQueue = new Queue<T>();
        protected readonly object sync = new object();

        public AutoResetEvent AutoResetEvent { get; } = new AutoResetEvent(false);


        public delegate bool BeforeEnQueueEventHandler(T target);

        public event BeforeEnQueueEventHandler BeforeEnQueueEvent;

        public bool HasValue => Count > 0;

        public int Count
        {
            get
            {
                lock (sync)
                    return InnerQueue.Count;
            }
        }

        public T Dequeue()
        {
            lock (sync)
                if (InnerQueue.Count > 0)
                    return InnerQueue.Dequeue();

            return default(T);
        }

        public void Enqueue(T item)
        {
            lock (sync)
            {
                var evt = BeforeEnQueueEvent;
                if (evt == null || evt(item))
                {
                    InnerQueue.Enqueue(item);
                    AutoResetEvent.Set();
                }
            }
        }

        public bool TryDequeue(out T item)
        {
            lock (sync)
            {
                if (InnerQueue.Count > 0)
                {
                    item = InnerQueue.Dequeue();
                    return true;
                }
            }

            item = default(T);
            return false;
        }
    }
}
