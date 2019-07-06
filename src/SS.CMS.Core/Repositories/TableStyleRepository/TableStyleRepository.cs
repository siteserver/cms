using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Core.Services;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.Repositories
{
    public partial class TableStyleRepository : ITableStyleRepository
    {
        private readonly IDistributedCache _cache;
        private readonly string _cacheKey;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITableStyleItemRepository _tableStyleItemRepository;
        private readonly Repository<TableStyleInfo> _repository;

        public TableStyleRepository(IDistributedCache cache, ISettingsManager settingsManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IUserRepository userRepository, ITableStyleItemRepository tableStyleItemRepository, IErrorLogRepository errorLogRepository)
        {
            _cache = cache;
            _cacheKey = _cache.GetKey(nameof(TableStyleRepository));
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _userRepository = userRepository;
            _tableStyleItemRepository = tableStyleItemRepository;
            _repository = new Repository<TableStyleInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(TableStyleInfo.Id);
            public const string RelatedIdentity = nameof(TableStyleInfo.RelatedIdentity);
            public const string TableName = nameof(TableStyleInfo.TableName);
            public const string AttributeName = nameof(TableStyleInfo.AttributeName);
            public const string Taxis = nameof(TableStyleInfo.Taxis);
        }

        public async Task<bool> IsExistsAsync(int relatedIdentity, string tableName, string attributeName)
        {
            var key = TableManager.GetKey(relatedIdentity, tableName, attributeName);
            var entries = await GetAllTableStylesAsync();
            return entries.Any(x => x.Key == key);
        }

        public async Task<int> InsertAsync(TableStyleInfo styleInfo)
        {
            var id = await _repository.InsertAsync(styleInfo);
            _tableStyleItemRepository.Insert(id, styleInfo.StyleItems);

            await _cache.RemoveAsync(_cacheKey);

            return id;
        }

        public async Task UpdateAsync(TableStyleInfo info, bool deleteAndInsertStyleItems = true)
        {
            _repository.Update(info);
            if (deleteAndInsertStyleItems)
            {
                await _tableStyleItemRepository.DeleteAndInsertStyleItemsAsync(info.Id, info.StyleItems);
            }

            await _cache.RemoveAsync(_cacheKey);
        }

        public async Task DeleteAsync(int relatedIdentity, string tableName, string attributeName)
        {
            await _repository.DeleteAsync(Q
                .Where(Attr.RelatedIdentity, relatedIdentity)
                .Where(Attr.TableName, tableName)
                .Where(Attr.AttributeName, attributeName));

            await _cache.RemoveAsync(_cacheKey);
        }

        public async Task DeleteAsync(List<int> relatedIdentities, string tableName)
        {
            if (relatedIdentities == null || relatedIdentities.Count <= 0) return;

            await _repository.DeleteAsync(Q
                .WhereIn(Attr.RelatedIdentity, relatedIdentities)
                .Where(Attr.TableName, tableName));

            await _cache.RemoveAsync(_cacheKey);
        }
    }
}