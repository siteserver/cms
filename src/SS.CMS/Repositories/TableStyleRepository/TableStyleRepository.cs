using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions;
using SS.CMS;
using SS.CMS.Core;

namespace SS.CMS.Repositories
{
    public partial class TableStyleRepository : ITableStyleRepository
    {
        private readonly Repository<TableStyle> _repository;

        public TableStyleRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<TableStyle>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string GetCacheKey(string tableName)
        {
            return Caching.GetListKey(_repository.TableName, tableName);
        }

        private void Sync(TableStyle style)
        {
            if (style?.Items != null)
            {
                style.ItemValues = TranslateUtils.JsonSerialize(style.Items);
            }
        }

        public async Task<int> InsertAsync(List<int> relatedIdentities, TableStyle style)
        {
            Sync(style);

            if (style.Taxis == 0)
            {
                style.Taxis =
                    await GetMaxTaxisAsync(style.TableName,
                        relatedIdentities) + 1;
            }

            var styleId = await _repository.InsertAsync(style, Q
                .CachingRemove(GetCacheKey(style.TableName))
            );

            return styleId;
        }

        public async Task UpdateAsync(TableStyle style)
        {
            Sync(style);

            await _repository.UpdateAsync(style, Q.CachingRemove(GetCacheKey(style.TableName)));
        }

        public async Task DeleteAllAsync(string tableName)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(TableStyle.TableName), tableName)
                .CachingRemove(GetCacheKey(tableName))
            );
        }

        public async Task DeleteAsync(int relatedIdentity, string tableName, string attributeName)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(TableStyle.RelatedIdentity), relatedIdentity)
                .Where(nameof(TableStyle.TableName), tableName)
                .Where(nameof(TableStyle.AttributeName), attributeName)
                .CachingRemove(GetCacheKey(tableName))
            );
        }

        public async Task DeleteAsync(List<int> relatedIdentities, string tableName)
        {
            if (relatedIdentities == null || relatedIdentities.Count <= 0) return;

            await _repository.DeleteAsync(Q
                .WhereIn(nameof(TableStyle.RelatedIdentity), relatedIdentities)
                .Where(nameof(TableStyle.TableName), tableName)
                .CachingRemove(GetCacheKey(tableName))
            );
        }

        private async Task<List<TableStyle>> GetAllAsync(string tableName)
        {
            var styles = await _repository.GetAllAsync(Q
                .Where(nameof(TableStyle.TableName), tableName)
                .OrderByDesc(nameof(TableStyle.Taxis), nameof(TableStyle.Id))
                .CachingGet(GetCacheKey(tableName))
            );
            foreach (var style in styles)
            {
                style.Items = TranslateUtils.JsonDeserialize<List<TableStyleItem>>(style.ItemValues);
            }

            return styles;
        }
    }
}
