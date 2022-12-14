using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public class ScheduledTaskRepository : IScheduledTaskRepository
    {
        private readonly Repository<ScheduledTask> _repository;
        private readonly IConfigRepository _configRepository;

        public ScheduledTaskRepository(ISettingsManager settingsManager, IConfigRepository configRepository)
        {
            _repository = new Repository<ScheduledTask>(settingsManager.Database, settingsManager.Redis);
            _configRepository = configRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static readonly string CacheKey = CacheUtils.GetClassKey(typeof(ScheduledTaskRepository));

        public async Task<ScheduledTask> GetAsync(int id)
        {
            var tasks = await GetAllAsync();
            return tasks.FirstOrDefault(t => t.Id == id);
        }

        public async Task<List<ScheduledTask>> GetAllAsync()
        {
            return await _repository.GetAllAsync(Q
              .OrderBy(nameof(ScheduledTask.Id))
              .CachingGet(CacheKey)
            );
        }

        public async Task<ScheduledTask> GetNextAsync()
        {
            var tasks = await GetAllAsync();
            var nextTask = tasks.Where(x => !x.IsDisabled && x.ScheduledDate.HasValue && x.ScheduledDate.Value < DateTime.Now).OrderBy(x => x.ScheduledDate).FirstOrDefault();
            return nextTask;
        }

        private DateTime? CalcScheduledDate(ScheduledTask task)
        {
            var now = DateTime.Now;
            if (task.IsDisabled) return null;

            DateTime? date = null;
            if (task.TaskInterval == TaskInterval.Once)
            {
                if (task.StartDate < now) return null;
                date = task.StartDate;
            }
            else if (task.TaskInterval == TaskInterval.EveryHour)
            {
                if (task.LatestStartDate.HasValue)
                {
                    var ts = new TimeSpan(now.Ticks - task.LatestStartDate.Value.Ticks);
                    if (ts.Hours > task.Every)
                    {
                        date = new DateTime(now.Year, now.Month, now.Day, now.Hour, task.StartDate.Minute, task.StartDate.Second);
                        if (date < now)
                        {
                            date = date.Value.AddHours(1);
                        }
                    }
                    else
                    {
                        date = new DateTime(now.Year, now.Month, now.Day, task.LatestStartDate.Value.Hour, task.StartDate.Minute, task.StartDate.Second);
                        date = date.Value.AddHours(task.Every);
                    }
                }
                else
                {
                    if (task.StartDate < now)
                    {
                        date = new DateTime(now.Year, now.Month, now.Day, now.Hour, task.StartDate.Minute, task.StartDate.Second);
                        if (date < now)
                        {
                            date = date.Value.AddHours(1);
                        }
                    }
                    else
                    {
                        date = task.StartDate;
                    }
                }
            }
            else if (task.TaskInterval == TaskInterval.EveryDay)
            {
                if (task.LatestStartDate.HasValue)
                {
                    var ts = new TimeSpan(now.Ticks - task.LatestStartDate.Value.Ticks);
                    if (ts.Days > task.Every)
                    {
                        date = new DateTime(now.Year, now.Month, now.Day, task.StartDate.Hour, task.StartDate.Minute, task.StartDate.Second);
                        if (date < now)
                        {
                            date = date.Value.AddDays(1);
                        }
                    }
                    else
                    {
                        date = new DateTime(now.Year, now.Month, now.Day, task.StartDate.Hour, task.StartDate.Minute, task.StartDate.Second);
                        date = date.Value.AddDays(task.Every);
                    }
                }
                else
                {
                    if (task.StartDate < now)
                    {
                        date = new DateTime(now.Year, now.Month, now.Day, task.StartDate.Hour, task.StartDate.Minute, task.StartDate.Second);
                        if (date < now)
                        {
                            date = date.Value.AddDays(1);
                        }
                    }
                    else
                    {
                        date = task.StartDate;
                    }
                }
            }
            else if (task.TaskInterval == TaskInterval.EveryWeek)
            {
                var nowWeek = (int)now.DayOfWeek;
                if (task.Weeks != null && task.Weeks.Count > 0)
                {
                    date = new DateTime(now.Year, now.Month, now.Day, task.StartDate.Hour, task.StartDate.Minute, task.StartDate.Second);
                    for (var i = nowWeek; i <= 6 + nowWeek; i++)
                    {
                        var theWeek = i % 7;
                        if (task.Weeks.Contains(theWeek))
                        {
                            date = date.Value.AddDays(i - nowWeek);
                            break;
                        }
                    }
                }
            }
            return date;
        }

        public async Task<int> InsertAsync(ScheduledTask task)
        {
            task.ScheduledDate = CalcScheduledDate(task);
            return await _repository.InsertAsync(task, Q.CachingRemove(CacheKey));
        }

        public async Task<int> InsertPublishAsync(Content content, DateTime scheduledDate)
        {
            var config = await _configRepository.GetAsync();
            var task = new ScheduledTask
            {
                Title = TaskType.Publish.GetDisplayName(),
                TaskType = TaskType.Publish,
                TaskInterval = TaskInterval.Once,
                StartDate = DateTime.Now,
                IsNoticeSuccess = true,
                IsNoticeFailure = true,
                NoticeFailureCount = 0,
                IsNoticeMobile = true,
                NoticeMobile = config.CloudMobile,
                IsDisabled = false,
                Timeout = 10,
                ScheduledDate = scheduledDate,
                PublishSiteId = content.SiteId,
                PublishChannelId = content.ChannelId,
                PublishContentId = content.Id,
            };
            return await _repository.InsertAsync(task, Q.CachingRemove(CacheKey));
        }

        public async Task<int> InsertCloudSyncAsync()
        {
            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            startDate = startDate.AddHours(-1);
            var task = new ScheduledTask
            {        
                Title = TaskType.CloudSync.GetDisplayName(),
                TaskType = TaskType.CloudSync,
                TaskInterval = TaskInterval.EveryHour,
                Every = 1,
                StartDate = startDate,
                IsDisabled = false,
                Timeout = 60,
            };
            return await InsertAsync(task);
        }

        public async Task<int> InsertCloudBackupAsync()
        {
            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, now.Day, StringUtils.GetRandomInt(0, 6), StringUtils.GetRandomInt(0, 59), 0);
            startDate = startDate.AddDays(-1);
            var task = new ScheduledTask
            {        
                Title = TaskType.CloudBackup.GetDisplayName(),
                TaskType = TaskType.CloudBackup,
                TaskInterval = TaskInterval.EveryDay,
                Every = 1,
                StartDate = startDate,
                IsDisabled = false,
                Timeout = 60,
            };
            return await InsertAsync(task);
        }

        public async Task UpdateAsync(ScheduledTask task)
        {
            task.ScheduledDate = CalcScheduledDate(task);
            await _repository.UpdateAsync(task, Q.CachingRemove(CacheKey));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id, Q.CachingRemove(CacheKey));
        }
    }
}
