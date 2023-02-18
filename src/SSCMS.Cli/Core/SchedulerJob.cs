using System.Threading.Tasks;
using Quartz;

namespace SSCMS.Cli.Core
{
    internal class SchedulerJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            using var console = new ConsoleUtils(false);
            await Program.Application.RunExecuteAsync(Application.CommandName, Application.CommandArgs, Application.CommandExtras, context);

            if (context.NextFireTimeUtc != null)
            {
                await console.WriteLineAsync();
                await console.WriteRowLineAsync();
                await console.WriteRowAsync("Fire Time", "Next Fire Time");
                await console.WriteRowLineAsync();
                await console.WriteRowAsync($"{context.FireTimeUtc.ToLocalTime():yyyy-MM-dd HH:mm:ss}", $"{context.NextFireTimeUtc.Value.ToLocalTime():yyyy-MM-dd HH:mm:ss}");
                await console.WriteRowLineAsync();
                await console.WriteLineAsync();
            }
        }
    }
}