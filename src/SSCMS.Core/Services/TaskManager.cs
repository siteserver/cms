using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using FluentScheduler;
using SSCMS.Services;

namespace SSCMS.Core.Services
{
    public class TaskManager : ITaskManager 
    {
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _workItems =
            new ConcurrentQueue<Func<CancellationToken, Task>>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void Queue(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(
            CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }

        public void RunOnceAt(Action job, DateTime dateTime)
        {
            JobManager.AddJob(job, s => s.ToRunOnceAt(dateTime));
        }
    }
}
