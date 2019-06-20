using System.Collections.Generic;
using System.Linq;
using SS.CMS.Core.Services;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.Repositories
{
    public partial class TableStyleRepository : ITableStyleRepository
    {
        private readonly ICacheManager _cacheManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITableStyleItemRepository _tableStyleItemRepository;
        private readonly Repository<TableStyleInfo> _repository;

        public TableStyleRepository(ISettingsManager settingsManager, ICacheManager cacheManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IUserRepository userRepository, ITableStyleItemRepository tableStyleItemRepository, IErrorLogRepository errorLogRepository)
        {
            _cacheManager = cacheManager;
            _repository = new Repository<TableStyleInfo>(new Db(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _userRepository = userRepository;
            _tableStyleItemRepository = tableStyleItemRepository;
        }

        public IDb Db => _repository.Db;
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

        public bool IsExists(int relatedIdentity, string tableName, string attributeName)
        {
            var key = TableManager.GetKey(relatedIdentity, tableName, attributeName);
            var entries = GetAllTableStyles();
            return entries.Any(x => x.Key == key);
        }

        public int Insert(TableStyleInfo styleInfo)
        {
            var id = _repository.Insert(styleInfo);
            _tableStyleItemRepository.Insert(id, styleInfo.StyleItems);

            ClearCache();

            return id;
        }

        public void Update(TableStyleInfo info, bool deleteAndInsertStyleItems = true)
        {
            _repository.Update(info);
            if (deleteAndInsertStyleItems)
            {
                _tableStyleItemRepository.DeleteAndInsertStyleItems(info.Id, info.StyleItems);
            }

            ClearCache();
        }

        public void Delete(int relatedIdentity, string tableName, string attributeName)
        {
            _repository.Delete(Q
                .Where(Attr.RelatedIdentity, relatedIdentity)
                .Where(Attr.TableName, tableName)
                .Where(Attr.AttributeName, attributeName));

            ClearCache();
        }

        public void Delete(List<int> relatedIdentities, string tableName)
        {
            if (relatedIdentities == null || relatedIdentities.Count <= 0) return;

            _repository.Delete(Q
                .WhereIn(Attr.RelatedIdentity, relatedIdentities)
                .Where(Attr.TableName, tableName));

            ClearCache();
        }

        private List<KeyValuePair<string, TableStyleInfo>> GetAllTableStylesToCache()
        {
            var pairs = new List<KeyValuePair<string, TableStyleInfo>>();

            var allItemsDict = _tableStyleItemRepository.GetAllTableStyleItems();

            var styleInfoList = _repository.GetAll(Q.OrderByDesc(Attr.Taxis, Attr.Id));
            foreach (var styleInfo in styleInfoList)
            {
                allItemsDict.TryGetValue(styleInfo.Id, out var items);
                styleInfo.StyleItems = items;

                var key = TableManager.GetKey(styleInfo.RelatedIdentity, styleInfo.TableName, styleInfo.AttributeName);

                if (pairs.All(pair => pair.Key != key))
                {
                    var pair = new KeyValuePair<string, TableStyleInfo>(key, styleInfo);
                    pairs.Add(pair);
                }
            }

            return pairs;
        }
    }
}