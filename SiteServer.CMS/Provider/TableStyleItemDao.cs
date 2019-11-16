using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class TableStyleItemDao : IRepository
    {
        private readonly Repository<TableStyleItem> _repository;

        public TableStyleItemDao()
        {
            _repository = new Repository<TableStyleItem>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task InsertAsync(int tableStyleId, List<TableStyleItem> styleItems)
        {
            if (styleItems == null || styleItems.Count <= 0) return;

            foreach (var item in styleItems)
            {
                await _repository.InsertAsync(item);
            }

            TableStyleManager.ClearCache();
        }

        public async Task DeleteAllAsync(int tableStyleId)
        {
            await _repository.DeleteAsync(Q.Where(nameof(TableStyleItem.TableStyleId), tableStyleId));

            TableStyleManager.ClearCache();
        }

        public async Task DeleteAndInsertStyleItemsAsync(int tableStyleId, List<TableStyleItem> styleItems)
        {
            await _repository.DeleteAsync(Q.Where(nameof(TableStyleItem.TableStyleId), tableStyleId));

            if (styleItems == null || styleItems.Count == 0) return;

            foreach (var item in styleItems)
            {
                await _repository.InsertAsync(item);
            }

            TableStyleManager.ClearCache();
        }

        public async Task<Dictionary<int, List<TableStyleItem>>> GetAllTableStyleItemsAsync()
        {
            var allDict = new Dictionary<int, List<TableStyleItem>>();

            var infoList = await _repository.GetAllAsync();
            foreach (var item in infoList)
            {
                allDict.TryGetValue(item.TableStyleId, out var list);

                if (list == null)
                {
                    list = new List<TableStyleItem>();
                }

                list.Add(item);

                allDict[item.TableStyleId] = list;
            }

            return allDict;
        }
    }
}
