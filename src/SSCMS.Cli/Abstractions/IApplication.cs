using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace SSCMS.Cli.Abstractions
{
    public interface IApplication
    {
        Task RunAsync(string[] args);

        Task RunExecuteAsync(string commandName, string[] commandArgs, string[] commandExtras,
            IJobExecutionContext jobContext);
    }
}
