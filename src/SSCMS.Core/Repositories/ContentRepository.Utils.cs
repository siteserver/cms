using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SqlKata;
using SSCMS.Core.Utils;
using SSCMS.Models;

namespace SSCMS.Core.Repositories
{
    public partial class ContentRepository
    {
        private const int TaxisIsTopStartValue = 2000000000;

        private static readonly ConcurrentDictionary<string, Repository<Content>> TableNameRepositories = new ConcurrentDictionary<string, Repository<Content>>();

        private async Task<Repository<Content>> GetRepositoryAsync(Site site, IChannelSummary channel)
        {
            var tableName = _channelRepository.GetTableName(site, channel);
            return await GetRepositoryAsync(tableName);
        }

        private Query GetQuery(int siteId, int channelId = 0)
        {
            return GetQuery(Q.NewQuery(), siteId, channelId);
        }

        private Query GetQuery(Query query, int siteId, int channelId = 0)
        {
            if (query == null)
            {
                query = Q.NewQuery();
            }
            query
                .Where(nameof(Content.SiteId), siteId)
                .WhereNot(nameof(Content.SourceId), SourceManager.Preview);

            if (channelId > 0)
            {
                query.Where(nameof(Content.ChannelId), channelId);
            }

            return query;
        }
    }
}