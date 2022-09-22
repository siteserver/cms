using System.Threading.Tasks;
using SSCMS.Dto;

namespace SSCMS.Services
{
    public interface IStorageManager
    {
        Task<bool> IsEnabledAsync(int siteId);

        Task SyncAllAsync(int siteId);

        Task SyncAsync(int siteId, string filePath);

        void ClearAllTask(int siteId);

        SyncTaskSummary GetTaskSummary(int siteId);
    }
}
