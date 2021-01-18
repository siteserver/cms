using System.Collections.Concurrent;
using System.Collections.Generic;
using Datory;
using Senparc.Weixin;
using SqlKata;
using SSCMS.Core.Utils;
using SSCMS.Models;

namespace SSCMS.Core.Repositories
{
    public partial class ContentRepository
    {
        private const int TaxisIsTopStartValue = 2000000000;

        public List<TableColumn> GetTableColumns(string tableName)
        {
            var repository = GetRepository(tableName);
            return repository.TableColumns;
        }

        private static readonly ConcurrentDictionary<string, Repository<Content>> TableNameRepositories = new ConcurrentDictionary<string, Repository<Content>>();

        private Repository<Content> GetRepository(Site site, IChannelSummary channel)
        {
            var tableName = _channelRepository.GetTableName(site, channel);
            return GetRepository(tableName);
        }

        private Repository<Content> GetRepository(string tableName)
        {
            if (TableNameRepositories.TryGetValue(tableName, out var repository))
            {
                return repository;
            }

            repository = new Repository<Content>(_settingsManager.Database, tableName, _settingsManager.Redis);

            TableNameRepositories[tableName] = repository;
            return repository;
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