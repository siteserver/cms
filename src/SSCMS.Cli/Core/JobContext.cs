using System;
using Quartz;
using SSCMS.Plugins;

namespace SSCMS.Cli.Core
{
    public class JobContext : IPluginJobContext
    {
        public JobContext(string command, string[] args, string[] extras, IJobExecutionContext context)
        {
            Command = command;
            Args = args;
            Extras = extras;
            if (context != null)
            {
                FireTime = context.FireTimeUtc.LocalDateTime;
                if (context.ScheduledFireTimeUtc != null)
                    ScheduledFireTime = context.ScheduledFireTimeUtc.Value.LocalDateTime;
                if (context.PreviousFireTimeUtc != null) PreviousFireTime = context.PreviousFireTimeUtc.Value.LocalDateTime;
                if (context.NextFireTimeUtc != null) NextFireTime = context.NextFireTimeUtc.Value.LocalDateTime;
                JobRunTime = context.JobRunTime;
            }
            else
            {
                FireTime = DateTime.Now;
                JobRunTime = new TimeSpan(0);
            }
        }

        public string Command { get; }
        public string[] Args { get; }
        public string[] Extras { get; }
        public DateTime FireTime { get; }
        public DateTime? ScheduledFireTime { get; }
        public DateTime? PreviousFireTime { get; }
        public DateTime? NextFireTime { get; }
        public TimeSpan JobRunTime { get; }
    }
}
