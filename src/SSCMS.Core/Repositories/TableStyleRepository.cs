using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;
using TableColumn = Datory.TableColumn;

namespace SSCMS.Core.Repositories
{
    public partial class TableStyleRepository : ITableStyleRepository
    {
        private readonly Repository<TableStyle> _repository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IUserRepository _userRepository;

        public TableStyleRepository(ISettingsManager settingsManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IUserRepository userRepository)
        {
            _repository = new Repository<TableStyle>(settingsManager.Database, settingsManager.Redis);
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _userRepository = userRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string GetCacheKey(string tableName)
        {
            return CacheUtils.GetListKey(_repository.TableName, tableName);
        }

        private void Sync(TableStyle style)
        {
            if (style?.Items != null)
            {
                style.ItemValues = TranslateUtils.JsonSerialize(style.Items);
            }

            if (style?.Rules != null)
            {
                style.RuleValues = TranslateUtils.JsonSerialize(style.Rules);
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

            await _repository.UpdateAsync(style, Q
                .CachingRemove(GetCacheKey(style.TableName))
            );
        }

        public async Task DeleteAllAsync(string tableName)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(TableStyle.TableName), tableName)
                .CachingRemove(GetCacheKey(tableName))
            );
        }

        public async Task DeleteAllAsync(string tableName, List<int> relatedIdentities)
        {
            if (relatedIdentities == null || relatedIdentities.Count <= 0) return;

            await _repository.DeleteAsync(Q
                .WhereIn(nameof(TableStyle.RelatedIdentity), relatedIdentities)
                .Where(nameof(TableStyle.TableName), tableName)
                .CachingRemove(GetCacheKey(tableName))
            );
        }

        public async Task DeleteAsync(string tableName, int relatedIdentity, string attributeName)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(TableStyle.RelatedIdentity), relatedIdentity)
                .Where(nameof(TableStyle.TableName), tableName)
                .Where(nameof(TableStyle.AttributeName), attributeName)
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
                style.Items = TranslateUtils.JsonDeserialize<List<InputStyleItem>>(style.ItemValues);
                style.Rules = TranslateUtils.JsonDeserialize<List<InputStyleRule>>(style.RuleValues);
            }

            return styles;
        }
    }
}
