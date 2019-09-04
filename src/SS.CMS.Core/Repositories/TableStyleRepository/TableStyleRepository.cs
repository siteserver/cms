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
        private readonly IDatabaseRepository _databaseRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITableStyleItemRepository _tableStyleItemRepository;
        private readonly Repository<TableStyle> _repository;

        public TableStyleRepository(IDistributedCache cache, ISettingsManager settingsManager, IDatabaseRepository databaseRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, IUserRepository userRepository, ITableStyleItemRepository tableStyleItemRepository, IErrorLogRepository errorLogRepository)
        {
            _cache = cache;
            _cacheKey = _cache.GetKey(nameof(TableStyleRepository));
            _databaseRepository = databaseRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _userRepository = userRepository;
            _tableStyleItemRepository = tableStyleItemRepository;
            _repository = new Repository<TableStyle>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(TableStyle.Id);
            public const string RelatedIdentity = nameof(TableStyle.RelatedIdentity);
            public const string TableName = nameof(TableStyle.TableName);
            public const string AttributeName = nameof(TableStyle.AttributeName);
            public const string Taxis = nameof(TableStyle.Taxis);
        }

        public async Task<bool> IsExistsAsync(int relatedIdentity, string tableName, string attributeName)
        {
            var key = GetKey(relatedIdentity, tableName, attributeName);
            var entries = await GetAllTableStylesAsync();
            return entries.Any(x => x.Key == key);
        }

        public async Task<int> InsertAsync(TableStyle styleInfo)
        {
            var id = await _repository.InsertAsync(styleInfo);
            await _tableStyleItemRepository.InsertAllAsync(id, styleInfo.StyleItems);

            await _cache.RemoveAsync(_cacheKey);

            return id;
        }

        public async Task UpdateAsync(TableStyle info, bool deleteAndInsertStyleItems = true)
        {
            await _repository.UpdateAsync(info);
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