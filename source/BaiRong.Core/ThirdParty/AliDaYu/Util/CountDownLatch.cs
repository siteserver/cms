using System.Threading;

namespace Top.Api.Util
{
    public class CountDownLatch
    {
        private int count;
        private EventWaitHandle ewh;

        public CountDownLatch(int count)
        {
            this.count = count;
            this.ewh = new ManualResetEvent(false);
        }

        public void Signal()
        {
            // The last thread to signal also sets the event.
            if (Interlocked.Decrement(ref count) == 0)
            {
                this.ewh.Set();
            }
        }

        public void Wait()
        {
            this.ewh.WaitOne();
        }
    }
}
