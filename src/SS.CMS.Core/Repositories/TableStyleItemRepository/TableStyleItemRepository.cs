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
        private readonly Repository<TableStyleItem> _repository;

        public TableStyleItemRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<TableStyleItem>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string TableStyleId = nameof(TableStyleItem.TableStyleId);
        }

        public async Task InsertAllAsync(int tableStyleId, List<TableStyleItem> styleItems)
        {
            if (styleItems == null || styleItems.Count <= 0) return;

            foreach (var itemInfo in styleItems)
            {
                itemInfo.TableStyleId = tableStyleId;
                await _repository.InsertAsync(itemInfo);
            }
        }

        public async Task DeleteAndInsertStyleItemsAsync(int tableStyleId, List<TableStyleItem> styleItems)
        {
            await _repository.DeleteAsync(Q.Where(Attr.TableStyleId, tableStyleId));

            if (styleItems == null || styleItems.Count == 0) return;

            foreach (var itemInfo in styleItems)
            {
                itemInfo.TableStyleId = tableStyleId;
                await _repository.InsertAsync(itemInfo);
            }
        }

        public async Task<Dictionary<int, List<TableStyleItem>>> GetAllTableStyleItemsAsync()
        {
            var allDict = new Dictionary<int, List<TableStyleItem>>();

            var itemInfoList = await _repository.GetAllAsync();
            foreach (var itemInfo in itemInfoList)
            {
                allDict.TryGetValue(itemInfo.TableStyleId, out var list);

                if (list == null)
                {
                    list = new List<TableStyleItem>();
                }

                list.Add(itemInfo);

                allDict[itemInfo.TableStyleId] = list;
            }

            return allDict;
        }
    }
}