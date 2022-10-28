using System;
using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Enums;

namespace SSCMS.Services
{
    public interface IStorageManager
    {
        [Obsolete]
        Task<bool> IsSyncAsync(int siteId, SyncType syncType);

        [Obsolete]
        Task<bool> IsAutoSyncAsync(int siteId, SyncType syncType);

        [Obsolete]
        Task SyncAllAsync(int siteId);

        [Obsolete]
        Task<(bool, string)> SyncAsync(int siteId, string filePath);

        [Obsolete]
        void ClearAllTask(int siteId);

        [Obsolete]
        SyncTaskSummary GetTaskSummary(int siteId);

        Task<bool> IsStorageAsync(int siteId, SyncType syncType);

        Task<bool> IsAutoStorageAsync(int siteId, SyncType syncType);

        Task StorageAllAsync(int siteId);

        Task<(bool, string)> StorageAsync(int siteId, string filePath);

        void ClearStorageTasks(int siteId);

        SyncTaskSummary GetStorageTasks(int siteId);
    }
}
