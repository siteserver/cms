using System.Linq;
using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Core.Repositories
{
    public partial class ContentRepository
    {
        public async Task<int> GetCountCheckingAsync(Site site)
        {
            var channels = await _channelRepository.GetSummariesAsync(site.Id);
            var count = 0;
            foreach (var channel in channels)
            {
                var summaries = await GetSummariesAsync(site, channel);
                count += summaries.Count(summary => !summary.Checked);
            }

            return count;
        }
    }
}