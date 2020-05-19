using System;
using System.Threading;
using System.Threading.Tasks;

namespace SSCMS.Services
{
    public interface ITaskManager
    {
        void Queue(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueAsync(
            CancellationToken cancellationToken);
    }
}
