using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IScheduledTaskRepository : IRepository
    {
        Task<ScheduledTask> GetAsync(int id);

        Task<List<ScheduledTask>> GetAllAsync();

        Task<ScheduledTask> GetNextAsync();

        Task<int> InsertAsync(ScheduledTask task);

        Task<int> InsertPublishAsync(Content content, DateTime scheduledDate);

        Task<int> InsertCloudSyncAsync();

        Task<int> InsertCloudBackupAsync();

        Task UpdateAsync(ScheduledTask task);

        Task<bool> DeleteAsync(int id);
    }
}