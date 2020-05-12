using System;
using System.Threading.Tasks;
using Quartz;

namespace SSCMS.Cli.Core
{
    internal class SchedulerJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var application = CliUtils.GetApplication();
            await application.RunExecuteAsync(Application.CommandName, Application.CommandArgs, Application.CommandExtras, context);

            if (context.NextFireTimeUtc != null)
            {
                await Console.Out.WriteLineAsync();
                await WriteUtils.PrintRowLineAsync();
                await WriteUtils.PrintRowAsync("Fire Time", "Next Fire Time");
                await WriteUtils.PrintRowLineAsync();
                await WriteUtils.PrintRowAsync($"{context.FireTimeUtc.ToLocalTime():yyyy-MM-dd HH:mm:ss}", $"{context.NextFireTimeUtc.Value.ToLocalTime():yyyy-MM-dd HH:mm:ss}");
                await WriteUtils.PrintRowLineAsync();
                await Console.Out.WriteLineAsync();
            }
        }
    }
}