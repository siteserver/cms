using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Services;

namespace SSCMS.Core.Services
{
    public class StorageManager : IStorageManager
    {
        public Task<bool> IsEnabledAsync()
        {
            return Task.FromResult(false);
        }

        public void ClearAllTask(int siteId)
        {
            throw new System.NotImplementedException();
        }

        public SyncTaskSummary GetTaskSummary(int siteId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsSyncAsync(int siteId, SyncType syncType)
        {
            return Task.FromResult(false);
        }

        public Task<bool> IsAutoSyncAsync(int siteId, SyncType syncType)
        {
            return Task.FromResult(false);
        }

        public Task SyncAllAsync(int siteId)
        {
            throw new System.NotImplementedException();
        }

        public Task<(bool, string)> SyncAsync(int siteId, string filePath)
        {
            return Task.FromResult((false, string.Empty));
        }
    }
}
