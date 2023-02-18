using System;
using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class ScheduledHostedService
    {
        private async Task PingAsync(ScheduledTask task)
        {
            if (!string.IsNullOrEmpty(task.PingHost))
            {
                var host = PageUtils.RemoveProtocolFromUrl(task.PingHost);
                await _ping.SendPingAsync(task.PingHost, (int)TimeSpan.FromSeconds(5).TotalMilliseconds);
            }
        }
    }
}
