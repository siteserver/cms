using System.Threading.Tasks;
using SSCMS;

namespace SSCMS.Core.Repositories.ContentRepository
{
    public partial class ContentRepository
    {
#pragma warning disable CS1998 // 此异步方法缺少 "await" 运算符，将以同步方式运行。请考虑使用 "await" 运算符等待非阻止的 API 调用，或者使用 "await Task.Run(...)" 在后台线程上执行占用大量 CPU 的工作。
        public async Task<int> GetCountCheckingAsync(Site site)
#pragma warning restore CS1998 // 此异步方法缺少 "await" 运算符，将以同步方式运行。请考虑使用 "await" 运算符等待非阻止的 API 调用，或者使用 "await Task.Run(...)" 在后台线程上执行占用大量 CPU 的工作。
        {
            //var channels = await _channelRepository.GetAllSummaryAsync(site.Id);
            var count = 0;
            //foreach (var channel in channels)
            //{
            //    var summaries = await GetSummariesAsync(site, channel);
            //    count += summaries.Count(x => !x.Checked);
            //}

            return count;
        }
    }
}