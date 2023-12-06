using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Datory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class ScheduledHostedService : BackgroundService
    {
        private readonly Ping _ping;
        private readonly ILogger<ScheduledHostedService> _logger;
        private static readonly TimeSpan FREQUENCY = TimeSpan.FromSeconds(5);
        private readonly ISettingsManager _settingsManager;
        private readonly ICloudManager _cloudManager;
        private readonly ICreateManager _createManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;

        public ScheduledHostedService(ILogger<ScheduledHostedService> logger, IServiceProvider serviceProvider)
        {
            _ping = new Ping();
            _logger = logger;
            serviceProvider = serviceProvider.CreateScope().ServiceProvider;
            _settingsManager = serviceProvider.GetRequiredService<ISettingsManager>();
            _cloudManager = serviceProvider.GetRequiredService<ICloudManager>();
            _createManager = serviceProvider.GetRequiredService<ICreateManager>();
            _pathManager = serviceProvider.GetRequiredService<IPathManager>();
            _databaseManager = serviceProvider.GetRequiredService<IDatabaseManager>();
            _pluginManager = serviceProvider.GetRequiredService<IPluginManager>();
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(FREQUENCY, stoppingToken);

                if (string.IsNullOrEmpty(_settingsManager.DatabaseConnectionString)) continue;

                ScheduledTask task = null;
                try
                {
                    var config = await _databaseManager.ConfigRepository.GetAsync();
                    if (config.CloudType == CloudType.Free || string.IsNullOrEmpty(config.CloudUserName) || string.IsNullOrEmpty(config.CloudToken) || config.CloudUserId == 0)
                    {
                        continue;
                    }

                    task = await _databaseManager.ScheduledTaskRepository.GetNextAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

                if (task == null) continue;

                await ExecuteTaskAsync(task, stoppingToken);
            }
        }

        public async Task ExecuteTaskAsync(ScheduledTask task, CancellationToken stoppingToken)
        {
            try
            {
                task.IsRunning = true;
                task.LatestStartDate = DateTime.Now;
                await _databaseManager.ScheduledTaskRepository.UpdateAsync(task);

                var running = RunTaskAsync(task);
                var cancelTask = Task.Delay(TimeSpan.FromMinutes(task.Timeout), stoppingToken);

                //double await so exceptions from either task will bubble up
                await await Task.WhenAny(running, cancelTask);

                task.IsRunning = false;
                task.LatestEndDate = DateTime.Now;

                if (running.IsCompletedSuccessfully)
                {
                    task.IsLatestSuccess = true;
                    task.LatestFailureCount = 0;
                    task.LatestErrorMessage = string.Empty;
                    await _databaseManager.ScheduledTaskRepository.UpdateAsync(task);
                }
                else
                {
                    task.IsLatestSuccess = false;
                    task.LatestEndDate = DateTime.Now;
                    task.LatestFailureCount++;
                    task.LatestErrorMessage = $"任务执行超时（{task.Timeout}分钟）";
                    await _databaseManager.ScheduledTaskRepository.UpdateAsync(task);
                }
            }
            catch (Exception ex)
            {
                task.IsRunning = false;
                task.IsLatestSuccess = false;
                task.LatestEndDate = DateTime.Now;
                task.LatestFailureCount++;
                task.LatestErrorMessage = ex.Message;
                await _databaseManager.ScheduledTaskRepository.UpdateAsync(task);

                await _databaseManager.ErrorLogRepository.AddErrorLogAsync(ex);
                _logger.LogError(ex.Message);
            }

            try
            {
                await NoticeAsync(task);
            }
            catch (Exception ex)
            {
                await _databaseManager.ErrorLogRepository.AddErrorLogAsync(ex);
                _logger.LogError(ex.Message);
            }
        }

        private async Task RunTaskAsync(ScheduledTask task)
        {
            if (StringUtils.EqualsIgnoreCase(task.TaskType, TaskType.Create.GetValue()))
            {
                await CreateAsync(task);
            }
            else if (StringUtils.EqualsIgnoreCase(task.TaskType, TaskType.Ping.GetValue()))
            {
                await PingAsync(task);
            }
            else if (StringUtils.EqualsIgnoreCase(task.TaskType, TaskType.Publish.GetValue()))
            {
                await PublishAsync(task);
            }
            else if (StringUtils.EqualsIgnoreCase(task.TaskType, TaskType.CloudSync.GetValue()))
            {
                await CloudSyncAsync(task);
            }
            else if (StringUtils.EqualsIgnoreCase(task.TaskType, TaskType.CloudBackup.GetValue()))
            {
                await CloudBackupAsync(task);
            }
            else if (!string.IsNullOrEmpty(task.TaskType))
            {
                var plugins = _pluginManager.GetExtensions<IPluginScheduledTask>();
                if (plugins != null)
                {
                    foreach (var plugin in plugins)
                    {
                        if (StringUtils.EqualsIgnoreCase(task.TaskType, plugin.TaskType))
                        {
                            await plugin.ExecuteAsync(task.Settings);
                        }
                    }
                }
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
                        var parameters = new Dictionary<string, string>
                        {
                            { "name", task.Title },
                            { "time", DateUtils.GetTimeString(task.LatestEndDate.Value) }
                        };

                        await _cloudManager.SendSmsAsync(task.NoticeMobile, SmsCodeType.TaskSuccess, parameters);
                    }
                    if (task.IsNoticeMail && StringUtils.IsEmail(task.NoticeMail))
                    {
                        var items = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("任务名称", task.Title),
                            new KeyValuePair<string, string>("开始时间", DateUtils.GetDateAndTimeString(task.LatestStartDate.Value)),
                            new KeyValuePair<string, string>("完成时间", DateUtils.GetDateAndTimeString(task.LatestEndDate.Value))
                        };

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
                        var parameters = new Dictionary<string, string>
                        {
                            { "name", task.Title },
                            { "time", DateUtils.GetTimeString(task.LatestEndDate.Value) }
                        };

                        await _cloudManager.SendSmsAsync(task.NoticeMobile, SmsCodeType.TaskFailure, parameters);
                    }
                    if (task.IsNoticeMail && StringUtils.IsEmail(task.NoticeMail))
                    {
                        var items = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("任务名称", task.Title),
                            new KeyValuePair<string, string>("执行时间", DateUtils.GetDateAndTimeString(task.LatestEndDate.Value)),
                            new KeyValuePair<string, string>("失败原因", task.LatestErrorMessage)
                        };

                        await _cloudManager.SendMailAsync(task.NoticeMail, "任务执行失败", string.Empty, items);
                    }
                }
            }
        }
    }
}
