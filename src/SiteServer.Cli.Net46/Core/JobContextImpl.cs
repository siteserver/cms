using System;
using Quartz;
using SiteServer.Plugin;

namespace SiteServer.Cli.Core
{
    public class JobContextImpl : IJobContext
    {
        public JobContextImpl(string command, string[] args, IJobExecutionContext context)
        {
            Command = command;
            Args = args;
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
        public DateTime FireTime { get; }
        public DateTime? ScheduledFireTime { get; }
        public DateTime? PreviousFireTime { get; }
        public DateTime? NextFireTime { get; }
        public TimeSpan JobRunTime { get; }
    }
}
