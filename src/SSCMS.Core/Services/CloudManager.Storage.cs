using System;
using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Enums;

namespace SSCMS.Core.Services
{
    public partial class CloudManager
    {
        // [Obsolete]
        // public Task<bool> IsEnabledAsync()
        // {
        //     return Task.FromResult(false);
        // }

        [Obsolete]
        public void ClearAllTask(int siteId)
        {
            throw new System.NotImplementedException();
        }

        [Obsolete]
        public SyncTaskSummary GetTaskSummary(int siteId)
        {
            throw new System.NotImplementedException();
        }

        [Obsolete]
        public Task<bool> IsSyncAsync(int siteId, SyncType syncType)
        {
            return Task.FromResult(false);
        }

        [Obsolete]
        public Task<bool> IsAutoSyncAsync(int siteId, SyncType syncType)
        {
            return Task.FromResult(false);
        }

        [Obsolete]
        public Task SyncAllAsync(int siteId)
        {
            throw new System.NotImplementedException();
        }

        [Obsolete]
        public Task<(bool, string)> SyncAsync(int siteId, string filePath)
        {
            return Task.FromResult((false, string.Empty));
        }

        public Task<bool> IsStorageAsync()
        {
            return Task.FromResult(false);
        }

        public void ClearStorageTasks(int siteId)
        {
            throw new System.NotImplementedException();
        }

        public SyncTaskSummary GetStorageTasks(int siteId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsStorageAsync(int siteId, SyncType syncType)
        {
            return Task.FromResult(false);
        }

        public Task<bool> IsAutoStorageAsync(int siteId, SyncType syncType)
        {
            return Task.FromResult(false);
        }

        public Task StorageAllAsync(int siteId)
        {
            throw new System.NotImplementedException();
        }

        public Task<(bool, string)> StorageAsync(int siteId, string filePath)
        {
            return Task.FromResult((false, string.Empty));
        }
    }
}
