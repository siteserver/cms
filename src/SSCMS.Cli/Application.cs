using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Mono.Options;
using Quartz;
using Quartz.Impl;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Dto;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli
{
    public class Application : IApplication
    {
        private bool _isHelp;
        private string _repeat;
        private readonly OptionSet _options;
        public static string CommandName { get; private set; }
        public static string[] CommandArgs { get; private set; }
        public static string[] CommandExtras { get; private set; }

        private readonly ISettingsManager _settingsManager;
        private readonly IConsoleUtils _console;

        public Application(ISettingsManager settingsManager)
        {
            _options = new OptionSet {
                { "r|repeat=", "schedule CRON expression",
                    v => _repeat = v },
                { "h|help",  "Display help",
                    v => _isHelp = v != null }
            };

            _settingsManager = settingsManager;
            _console = new ConsoleUtils(false);
        }

        public async Task RunAsync(string[] args)
        {
            if (!CliUtils.ParseArgs(_options, args)) return;

            var jobServiceCommandNames = GetJobServiceCommandNames();
            var isJobService = false;

            var commandName = string.Empty;
            var commandArgs = new List<string>();
            var commandExtras = new List<string>();
            if (args.Length >= 1)
            {
                var isCommand = true;
                foreach (var arg in args)
                {
                    if (isCommand && !StringUtils.StartsWith(arg, "-"))
                    {
                        if (isJobService)
                        {
                            commandExtras.Add(StringUtils.Trim(arg));
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(commandName))
                            {
                                commandName += " " + StringUtils.Trim(arg);
                            }
                            else
                            {
                                commandName = StringUtils.Trim(arg);
                            }

                            isJobService = ListUtils.ContainsIgnoreCase(jobServiceCommandNames, commandName);
                        }
                    }
                    else
                    {
                        isCommand = false;
                        commandArgs.Add(StringUtils.Trim(arg));
                    }
                }
            }
            else
            {
                isJobService = true;
                commandName = "status";
            }

            CommandName = commandName;
            CommandArgs = commandArgs.ToArray();
            CommandExtras = commandExtras.ToArray();

            using (_console)
            {
                if (!isJobService)
                {
                    await RunHelpAsync(CommandName);
                }
                else if (!string.IsNullOrEmpty(_repeat))
                {
                    await RunRepeatAsync();
                }
                else
                {
                    await RunExecuteAsync(CommandName, CommandArgs, CommandExtras, null);
                }
            }
        }

        private async Task RunHelpAsync(string commandName)
        {
            if (_isHelp || string.IsNullOrEmpty(commandName))
            {
                await _console.WriteLineAsync("Welcome to SSCMS Command Line");
                await _console.WriteLineAsync();

                var services = GetJobServices();
                foreach (var service in services)
                {
                    await _console.WriteRowLineAsync();
                    await _console.WriteRowAsync(service.CommandName);
                    await _console.WriteRowLineAsync();

                    await service.WriteUsageAsync(_console);
                }
            }
            else
            {
                await _console.WriteLineAsync($"'{commandName}' is not a sscms command. See 'sscms --help'");
            }
        }

        private async Task RunRepeatAsync()
        {
            try
            {
                var factory = new StdSchedulerFactory(new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                });
                var scheduler = await factory.GetScheduler();

                await scheduler.Start();

                var job = JobBuilder.Create<SchedulerJob>()
                    .WithIdentity("job1", "group1")
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    .StartNow()
                    .WithCronSchedule(_repeat)
                    .WithPriority(1)
                    .Build();

                await scheduler.ScheduleJob(job, trigger);
                await Task.Delay(-1);
                await scheduler.Shutdown();
            }
            catch (Exception ex)
            {
                await _console.WriteErrorAsync(ex.Message);
            }
        }

        public async Task RunExecuteAsync(string commandName, string[] commandArgs, string[] commandExtras, IJobExecutionContext jobContext)
        {
            try
            {
                var service = GetJobService(commandName);
                if (service != null)
                {
                    var context = new JobContext(commandName, commandArgs, commandExtras, jobContext);
                    await service.ExecuteAsync(context);
                }
            }
            catch (Exception ex)
            {
                await _console.WriteErrorAsync(ex.Message);

                try
                {
                    var errorLogFilePath = CliUtils.DeleteErrorLogFileIfExists(_settingsManager);
                    await FileUtils.AppendErrorLogAsync(errorLogFilePath, new TextLog
                    {
                        DateTime = DateTime.Now,
                        Detail = ex.Message,
                        Exception = ex
                    });
                }
                catch { }
            }
        }

        private IJobService GetJobService(string commandName)
        {
            var provider = _settingsManager.BuildServiceProvider();
            var services = provider.GetServices<IJobService>();
            return services.FirstOrDefault(x => StringUtils.EqualsIgnoreCase(x.CommandName, commandName));
        }

        private List<string> GetJobServiceCommandNames()
        {
            var provider = _settingsManager.BuildServiceProvider();
            var services = provider.GetServices<IJobService>();
            return services.Select(x => x.CommandName).ToList();
        }

        private IEnumerable<IJobService> GetJobServices()
        {
            var provider = _settingsManager.BuildServiceProvider();
            return provider.GetServices<IJobService>();
        }
    }
}