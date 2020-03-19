using System;

namespace SSCMS
{
    /// <summary>
    /// 包含SiteServer Cli命令行执行任务时的上下文信息。
    /// </summary>
    public interface IJobContext
    {
        /// <summary>
        /// 当前所执行的命令。
        /// </summary>
        string Command { get; }

        /// <summary>
        /// 执行命令行时用户传递的参数。
        /// </summary>
        string[] Args { get; }

        /// <summary>
        /// 任务的实际执行时间。
        /// 例如，计划的时间可能是10:00:00，但是如果调度程序太忙，实际的执行时间可能是10:00:03。
        /// </summary>
        DateTime FireTime { get; }

        /// <summary>
        /// 任务的计划执行时间。
        /// 例如，计划的时间可能是10:00:00，但是如果调度程序太忙，实际的执行时间可能是10:00:03。
        /// </summary>
        DateTime? ScheduledFireTime { get; }


        /// <summary>
        /// 任务上一次执行的时间。
        /// </summary>
        DateTime? PreviousFireTime { get; }


        /// <summary>
        /// 任务下一次执行的时间。
        /// </summary>
        DateTime? NextFireTime { get; }

        /// <summary>
        /// 任务执行时间。
        /// 返回的值将一直到任务实际完成(或抛出异常)，因此通常只用于任务结束后获取。
        /// </summary>
        TimeSpan JobRunTime { get; }
    }
}