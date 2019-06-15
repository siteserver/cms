using System.Collections.Generic;
using System.Linq;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Common;
using SS.CMS.Data;

namespace SS.CMS.Core.Repositories
{
    public partial class TableStyleRepository : ITableStyleRepository
    {
        private readonly IUserRepository _userRepository;
        private readonly ITableStyleItemRepository _tableStyleItemRepository;
        private readonly Repository<TableStyleInfo> _repository;
        private readonly TableManager _tableManager;

        public TableStyleRepository(ISettingsManager settingsManager, IUserRepository userRepository, ITableStyleItemRepository tableStyleItemRepository)
        {
            _repository = new Repository<TableStyleInfo>(new Db(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _userRepository = userRepository;
            _tableStyleItemRepository = tableStyleItemRepository;
            _tableManager = new TableManager(settingsManager);
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

                var key = GetKey(styleInfo.RelatedIdentity, styleInfo.TableName, styleInfo.AttributeName);

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