using System.Collections.Generic;
using System.Threading.Tasks;
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
            _repository = new Repository<TableStyleItemInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string TableStyleId = nameof(TableStyleItemInfo.TableStyleId);
        }

        public async Task InsertAllAsync(int tableStyleId, List<TableStyleItemInfo> styleItems)
        {
            if (styleItems == null || styleItems.Count <= 0) return;

            foreach (var itemInfo in styleItems)
            {
                itemInfo.TableStyleId = tableStyleId;
                await _repository.InsertAsync(itemInfo);
            }
        }

        public async Task DeleteAndInsertStyleItemsAsync(int tableStyleId, List<TableStyleItemInfo> styleItems)
        {
            await _repository.DeleteAsync(Q.Where(Attr.TableStyleId, tableStyleId));

            if (styleItems == null || styleItems.Count == 0) return;

            foreach (var itemInfo in styleItems)
            {
                itemInfo.TableStyleId = tableStyleId;
                await _repository.InsertAsync(itemInfo);
            }
        }

        public async Task<Dictionary<int, List<TableStyleItemInfo>>> GetAllTableStyleItemsAsync()
        {
            var allDict = new Dictionary<int, List<TableStyleItemInfo>>();

            var itemInfoList = await _repository.GetAllAsync();
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