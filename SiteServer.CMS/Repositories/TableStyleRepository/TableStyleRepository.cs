using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.DataCache;

namespace SiteServer.CMS.Repositories
{
    public class TableStyleRepository : IRepository
    {
        private readonly Repository<TableStyle> _repository;

        public TableStyleRepository()
        {
            _repository = new Repository<TableStyle>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(TableStyle style)
        {
            var styleId = await _repository.InsertAsync(style);

            await DataProvider.TableStyleItemRepository.InsertAsync(styleId, style.StyleItems);

            TableStyleManager.ClearCache();

            return styleId;
        }

        public async Task UpdateAsync(TableStyle style, bool deleteAndInsertStyleItems = true)
        {
            await _repository.UpdateAsync(style);

            if (deleteAndInsertStyleItems)
            {
                await DataProvider.TableStyleItemRepository.DeleteAndInsertStyleItemsAsync(style.Id, style.StyleItems);
            }

            TableStyleManager.ClearCache();
        }

        public async Task DeleteAsync(int relatedIdentity, string tableName, string attributeName)
        {
            var styleId = await _repository.GetAsync<int>(Q
                .Select(nameof(TableStyle.Id))
                .Where(nameof(TableStyle.RelatedIdentity), relatedIdentity)
                .Where(nameof(TableStyle.TableName), tableName)
                .Where(nameof(TableStyle.AttributeName), attributeName)
            );

            if (styleId > 0)
            {
                await _repository.DeleteAsync(styleId);
                await DataProvider.TableStyleItemRepository.DeleteAllAsync(styleId);
            }

            TableStyleManager.ClearCache();
        }

        public async Task DeleteAsync(List<int> relatedIdentities, string tableName)
        {
            if (relatedIdentities == null || relatedIdentities.Count <= 0) return;

            var styleIdList = await _repository.GetAllAsync<int>(Q
                .Select(nameof(TableStyle.Id))
                .WhereIn(nameof(TableStyle.RelatedIdentity), relatedIdentities)
                .Where(nameof(TableStyle.TableName), tableName)
            );

            if (styleIdList != null)
            {
                foreach (var styleId in styleIdList)
                {
                    await _repository.DeleteAsync(styleId);
                    await DataProvider.TableStyleItemRepository.DeleteAllAsync(styleId);
                }
            }

            TableStyleManager.ClearCache();
        }

        public async Task<List<KeyValuePair<string, TableStyle>>> GetAllTableStylesAsync()
        {
            var pairs = new List<KeyValuePair<string, TableStyle>>();

            var allItemsDict = await DataProvider.TableStyleItemRepository.GetAllTableStyleItemsAsync();

            var list = await _repository.GetAllAsync(Q
                .OrderByDesc(nameof(TableStyle.Taxis), nameof(TableStyle.Id))
            );

            foreach (var style in list)
            {
                allItemsDict.TryGetValue(style.Id, out var items);
                style.StyleItems = items;

                var key = TableStyleManager.GetKey(style.RelatedIdentity, style.TableName, style.AttributeName);

                if (pairs.All(pair => pair.Key != key))
                {
                    var pair = new KeyValuePair<string, TableStyle>(key, style);
                    pairs.Add(pair);
                }
            }

            return pairs;
        }
    }
}
