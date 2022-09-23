using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Enums;

namespace SSCMS.Services
{
    public interface IStorageManager
    {
        Task<bool> IsSyncAsync(int siteId, SyncType syncType);

        Task<bool> IsAutoSyncAsync(int siteId, SyncType syncType);

        Task SyncAllAsync(int siteId);

        Task SyncAsync(int siteId, string filePath);

        void ClearAllTask(int siteId);

        SyncTaskSummary GetTaskSummary(int siteId);
    }
}
