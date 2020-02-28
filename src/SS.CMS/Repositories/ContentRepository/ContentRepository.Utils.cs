using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions;
using SqlKata;
using SS.CMS.Core;

namespace SS.CMS.Repositories
{
    public partial class ContentRepository
    {
        private const int TaxisIsTopStartValue = 2000000000;

        public static string GetContentTableName(int siteId)
        {
            return $"siteserver_Content_{siteId}";
        }

        public List<TableColumn> GetTableColumns(string tableName)
        {
            var repository = GetRepository(tableName);
            return repository.TableColumns;
        }

        public List<TableColumn> GetDefaultTableColumns(string tableName)
        {
            var tableColumns = new List<TableColumn>();
            tableColumns.AddRange(GetTableColumns(tableName));
            tableColumns.Add(new TableColumn
            {
                AttributeName = ContentAttribute.SubTitle,
                DataType = DataType.VarChar,
                DataLength = 255
            });
            tableColumns.Add(new TableColumn
            {
                AttributeName = ContentAttribute.ImageUrl,
                DataType = DataType.VarChar,
                DataLength = 200
            });
            tableColumns.Add(new TableColumn
            {
                AttributeName = ContentAttribute.VideoUrl,
                DataType = DataType.VarChar,
                DataLength = 200
            });
            tableColumns.Add(new TableColumn
            {
                AttributeName = ContentAttribute.FileUrl,
                DataType = DataType.VarChar,
                DataLength = 200
            });
            tableColumns.Add(new TableColumn
            {
                AttributeName = nameof(ContentAttribute.Content),
                DataType = DataType.Text
            });
            tableColumns.Add(new TableColumn
            {
                AttributeName = nameof(ContentAttribute.Summary),
                DataType = DataType.Text
            });
            tableColumns.Add(new TableColumn
            {
                AttributeName = nameof(ContentAttribute.Author),
                DataType = DataType.VarChar,
                DataLength = 255
            });
            tableColumns.Add(new TableColumn
            {
                AttributeName = nameof(ContentAttribute.Source),
                DataType = DataType.VarChar,
                DataLength = 255
            });

            return tableColumns;
        }

        private static readonly ConcurrentDictionary<string, Repository<Content>> TableNameRepositories = new ConcurrentDictionary<string, Repository<Content>>();

        private async Task<Repository<Content>> GetRepositoryAsync(Site site, IChannelSummary channel)
        {
            var tableName = await _channelRepository.GetTableNameAsync(site, channel);
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
            var query = Q
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