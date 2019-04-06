using System;
using System.Threading.Tasks;
using Quartz;

namespace SiteServer.Cli.Core
{
    internal class SchedJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Program.RunExecuteAsync(Program.CommandName, Program.CommandArgs, context);

            if (context.NextFireTimeUtc != null)
            {
                await Console.Out.WriteLineAsync();
                await CliUtils.PrintRowLineAsync();
                await CliUtils.PrintRowAsync("Fire Time", "Next Fire Time");
                await CliUtils.PrintRowLineAsync();
                await CliUtils.PrintRowAsync($"{context.FireTimeUtc.ToLocalTime():yyyy-MM-dd HH:mm:ss}", $"{context.NextFireTimeUtc.Value.ToLocalTime():yyyy-MM-dd HH:mm:ss}");
                await CliUtils.PrintRowLineAsync();
                await Console.Out.WriteLineAsync();
            }
        }
    }
}
