using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class ScheduledHostedService : BackgroundService
    {
        private readonly Ping _ping;
        private readonly ILogger<ScheduledHostedService> _logger;
        private static readonly TimeSpan FREQUENCY = TimeSpan.FromSeconds(5);
        private readonly ICloudManager _cloudManager;
        private readonly ICreateManager _createManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;
        private readonly IScheduledTaskRepository _scheduledTaskRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IStorageFileRepository _storageFileRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public ScheduledHostedService(ILogger<ScheduledHostedService> logger, IServiceProvider serviceProvider)
        {
            _ping = new Ping();
            _logger = logger;
            serviceProvider = serviceProvider.CreateScope().ServiceProvider;
            _cloudManager = serviceProvider.GetRequiredService<ICloudManager>();
            _createManager = serviceProvider.GetRequiredService<ICreateManager>();
            _pathManager = serviceProvider.GetRequiredService<IPathManager>();
            _configRepository = serviceProvider.GetRequiredService<IConfigRepository>();
            _scheduledTaskRepository = serviceProvider.GetRequiredService<IScheduledTaskRepository>();
            _siteRepository = serviceProvider.GetRequiredService<ISiteRepository>();
            _channelRepository = serviceProvider.GetRequiredService<IChannelRepository>();
            _contentRepository = serviceProvider.GetRequiredService<IContentRepository>();
            _storageFileRepository = serviceProvider.GetRequiredService<IStorageFileRepository>();
            _errorLogRepository = serviceProvider.GetRequiredService<IErrorLogRepository>();
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(FREQUENCY, stoppingToken);

                ScheduledTask task = null;
                try
                {
                    task = await _scheduledTaskRepository.GetNextAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

                if (task == null) continue;

                try
                {
                    task.LatestStartDate = DateTime.Now;

                    var running = RunTaskAsync(task);
                    var cancelTask = Task.Delay(TimeSpan.FromMinutes(task.Timeout), stoppingToken);

                    //double await so exceptions from either task will bubble up
                    await await Task.WhenAny(running, cancelTask);

                    if (running.IsCompletedSuccessfully)
                    {
                        task.IsLatestSuccess = true;
                        task.LatestEndDate = DateTime.Now;
                        task.LatestFailureCount = 0;
                        task.LatestErrorMessage = string.Empty;
                        await _scheduledTaskRepository.UpdateAsync(task);
                    }
                    else
                    {
                        task.IsLatestSuccess = false;
                        task.LatestEndDate = DateTime.Now;
                        task.LatestFailureCount++;
                        task.LatestErrorMessage = $"任务执行超时（{task.Timeout}分钟）";
                        await _scheduledTaskRepository.UpdateAsync(task);
                    }
                }
                catch (Exception ex)
                {
                    task.IsLatestSuccess = false;
                    task.LatestEndDate = DateTime.Now;
                    task.LatestFailureCount++;
                    task.LatestErrorMessage = ex.Message;
                    await _scheduledTaskRepository.UpdateAsync(task);

                    await _errorLogRepository.AddErrorLogAsync(ex);
                    _logger.LogError(ex.Message);
                }

                await NoticeAsync(task);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scheduled Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }

        private async Task NoticeAsync(ScheduledTask task)
        {
            if (task.IsLatestSuccess)
            {
                if (task.IsNoticeSuccess)
                {
                    if (task.IsNoticeMobile && StringUtils.IsMobile(task.NoticeMobile))
                    {
                        var parameters = new Dictionary<string, string>();
                        parameters.Add("name", task.Title);
                        parameters.Add("time", DateUtils.GetTimeString(task.LatestEndDate.Value));

                        await _cloudManager.SendSmsAsync(task.NoticeMobile, SmsCodeType.TaskSuccess, parameters);
                    }
                    if (task.IsNoticeMail && StringUtils.IsEmail(task.NoticeMail))
                    {
                        var items = new List<KeyValuePair<string, string>>();
                        items.Add(new KeyValuePair<string, string>("任务名称", task.Title));
                        items.Add(new KeyValuePair<string, string>("开始时间", DateUtils.GetDateAndTimeString(task.LatestStartDate.Value)));
                        items.Add(new KeyValuePair<string, string>("完成时间", DateUtils.GetDateAndTimeString(task.LatestEndDate.Value)));

                        await _cloudManager.SendMailAsync(task.NoticeMail, "任务执行成功", string.Empty, items);
                    }
                }
            }
            else
            {
                if (task.IsNoticeFailure && task.NoticeFailureCount < task.LatestFailureCount)
                {
                    if (task.IsNoticeMobile && StringUtils.IsMobile(task.NoticeMobile))
                    {
                        var parameters = new Dictionary<string, string>();
                        parameters.Add("name", task.Title);
                        parameters.Add("time", DateUtils.GetTimeString(task.LatestEndDate.Value));

                        await _cloudManager.SendSmsAsync(task.NoticeMobile, SmsCodeType.TaskFailure, parameters);
                    }
                    if (task.IsNoticeMail && StringUtils.IsEmail(task.NoticeMail))
                    {
                        var items = new List<KeyValuePair<string, string>>();
                        items.Add(new KeyValuePair<string, string>("任务名称", task.Title));
                        items.Add(new KeyValuePair<string, string>("执行时间", DateUtils.GetDateAndTimeString(task.LatestEndDate.Value)));
                        items.Add(new KeyValuePair<string, string>("失败原因", task.LatestErrorMessage));

                        await _cloudManager.SendMailAsync(task.NoticeMail, "任务执行失败", string.Empty, items);
                    }
                }
            }

        }

        private async Task RunTaskAsync(ScheduledTask task)
        {
            if (task.TaskType == TaskType.Create)
            {
                await CreateAsync(task);
            }
            else if (task.TaskType == TaskType.Ping)
            {
                await PingAsync(task);
            }
            else if (task.TaskType == TaskType.Publish)
            {
                await PublishAsync(task);
            }
            else if (task.TaskType == TaskType.CloudSync)
            {
                await CloudSyncAsync(task);
            }
        }
    }
}
