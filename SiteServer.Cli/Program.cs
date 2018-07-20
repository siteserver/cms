using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;
using Quartz;
using Quartz.Impl;
using SiteServer.Cli.Core;
using SiteServer.Cli.Jobs;
using SiteServer.Utils;

namespace SiteServer.Cli
{
    internal static class Program
    {
        private static bool IsHelp { get; set; }
        private static string Schedule { get; set; }
        public static string CommandName { get; private set; }
        public static string[] CommandArgs { get; private set; }

        private static readonly OptionSet Options = new OptionSet {
            { "s|schedule=", "schedule CRON expression",
                v => Schedule = v },
            { "h|help",  "show this message and exit",
                v => IsHelp = v != null }
        };

        private static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            if (!CliUtils.ParseArgs(Options, args)) return;

            var commandNames = new List<string>();
            var commandArgs = new List<string>();
            if (args.Length >= 1)
            {
                var isCommand = true;
                foreach (var arg in args)
                {
                    if (isCommand && !StringUtils.StartsWith(arg, "-"))
                    {
                        commandNames.Add(StringUtils.Trim(arg));
                    }
                    else
                    {
                        isCommand = false;
                        commandArgs.Add(StringUtils.Trim(arg));
                    }
                }
            }
            CommandName = string.Join(" ", commandNames);
            CommandArgs = commandArgs.ToArray();

            Console.WriteLine("Welcome to SiteServer Cli Tool");
            Console.WriteLine();

            if (!IsRunnable(CommandName))
            {
                if (IsHelp)
                {
                    CliUtils.PrintRowLine();
                    CliUtils.PrintRow("Usage");
                    CliUtils.PrintRowLine();
                    BackupJob.PrintUsage();
                    RestoreJob.PrintUsage();
                    UpdateJob.PrintUsage();
                    VersionJob.PrintUsage();
                    CliUtils.PrintRowLine();
                    CliUtils.PrintRow("https://www.siteserver.cn/docs/cli");
                    CliUtils.PrintRowLine();
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine($"'{CommandName}' is not a siteserver command. See 'sitserver --help'");
                }
                return;
            }

            if (!string.IsNullOrEmpty(Schedule))
            {
                RunScheduleAsync(Schedule).GetAwaiter().GetResult();
            }
            else
            {
                RunExecuteAsync(CommandName, CommandArgs, null).GetAwaiter().GetResult();
            }
        }

        private static async Task RunScheduleAsync(string schedule)
        {
            try
            {
                var factory = new StdSchedulerFactory(new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                });
                var sched = await factory.GetScheduler();

                await sched.Start();

                var job = JobBuilder.Create<SchedJob>()
                    .WithIdentity("job1", "group1")
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    .StartNow()
                    .WithCronSchedule(schedule)
                    .WithPriority(1)
                    .Build();

                await sched.ScheduleJob(job, trigger);
                await Task.Delay(-1);
                await sched.Shutdown();
            }
            catch (Exception ex)
            {
                await CliUtils.PrintErrorAsync(ex.Message);
            }
        }

        private static bool IsRunnable(string commandName)
        {
            return commandName == BackupJob.CommandName || commandName == RestoreJob.CommandName || commandName == UpdateJob.CommandName || commandName == VersionJob.CommandName || commandName == TestJob.CommandName;
        }

        public static async Task RunExecuteAsync(string commandName, string[] commandArgs, IJobExecutionContext jobContext)
        {
            try
            {
                Plugin.IJob job = null;

                if (StringUtils.EqualsIgnoreCase(commandName, BackupJob.CommandName))
                {
                    job = new BackupJob();
                }
                else if (StringUtils.EqualsIgnoreCase(commandName, RestoreJob.CommandName))
                {
                    job = new RestoreJob();
                }
                else if (StringUtils.EqualsIgnoreCase(commandName, UpdateJob.CommandName))
                {
                    job = new UpdateJob();
                }
                else if (StringUtils.EqualsIgnoreCase(commandName, VersionJob.CommandName))
                {
                    job = new VersionJob();
                }
                else if (StringUtils.EqualsIgnoreCase(commandName, TestJob.CommandName))
                {
                    job = new TestJob();
                }

                if (job != null)
                {
                    var context = new Core.JobExecutionContextImpl(commandName, commandArgs, job, jobContext);
                    await context.JobInstance.Execute(context);
                }
            }
            catch (Exception ex)
            {
                await CliUtils.PrintErrorAsync(ex.Message);

                var errorLogFilePath = CliUtils.CreateErrorLogFile("siteserver");

                await CliUtils.AppendErrorLogsAsync(errorLogFilePath, new List<TextLogInfo>
                {
                    new TextLogInfo
                    {
                        DateTime = DateTime.Now,
                        Detail = "Console Error",
                        Exception = ex
                    }
                });
            }
        }
    }
}
