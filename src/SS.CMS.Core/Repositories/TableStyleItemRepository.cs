using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.Repositories
{
    public class TableStyleItemRepository : ITableStyleItemRepository
    {
        private readonly Repository<TableStyleItemInfo> _repository;

        public TableStyleItemRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<TableStyleItemInfo>(new Db(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDb Db => _repository.Db;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string TableStyleId = nameof(TableStyleItemInfo.TableStyleId);
        }

        public void Insert(int tableStyleId, List<TableStyleItemInfo> styleItems)
        {
            if (styleItems == null || styleItems.Count <= 0) return;

            foreach (var itemInfo in styleItems)
            {
                itemInfo.TableStyleId = tableStyleId;
                _repository.Insert(itemInfo);
            }
        }

        public void DeleteAndInsertStyleItems(int tableStyleId, List<TableStyleItemInfo> styleItems)
        {
            _repository.Delete(Q.Where(Attr.TableStyleId, tableStyleId));

            if (styleItems == null || styleItems.Count == 0) return;

            foreach (var itemInfo in styleItems)
            {
                itemInfo.TableStyleId = tableStyleId;
                _repository.Insert(itemInfo);
            }
        }

        public Dictionary<int, List<TableStyleItemInfo>> GetAllTableStyleItems()
        {
            var allDict = new Dictionary<int, List<TableStyleItemInfo>>();

            var itemInfoList = _repository.GetAll();
            foreach (var itemInfo in itemInfoList)
            {
                allDict.TryGetValue(itemInfo.TableStyleId, out var list);

                if (list == null)
                {
                    list = new List<TableStyleItemInfo>();
                }

                list.Add(itemInfo);

                allDict[itemInfo.TableStyleId] = list;
            }

            return allDict;
        }
    }
}