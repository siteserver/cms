using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public class PluginRepository : IPluginRepository
    {
        private static readonly string CacheKey = StringUtils.GetCacheKey(nameof(PluginRepository));
        private readonly Repository<PluginInfo> _repository;
        private readonly ISettingsManager _settingsManager;
        private readonly ICacheManager _cacheManager;
        public PluginRepository(ISettingsManager settingsManager, ICacheManager cacheManager)
        {
            _repository = new Repository<PluginInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _settingsManager = settingsManager;
            _cacheManager = cacheManager;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string PluginId = nameof(PluginInfo.PluginId);
            public const string IsDisabled = "IsDisabled";
            public const string Taxis = nameof(PluginInfo.Taxis);
        }

        public void DeleteById(string pluginId)
        {
            _repository.Delete(Q.Where(Attr.PluginId, pluginId));
        }

        public void UpdateIsDisabled(string pluginId, bool isDisabled)
        {
            _repository.Update(Q
                .Set(Attr.IsDisabled, isDisabled.ToString())
                .Where(Attr.PluginId, pluginId)
            );
        }

        public void UpdateTaxis(string pluginId, int taxis)
        {
            _repository.Update(Q
                .Set(Attr.Taxis, taxis)
                .Where(Attr.PluginId, pluginId)
            );
        }

        public void SetIsDisabledAndTaxis(string pluginId, out bool isDisabled, out int taxis)
        {
            isDisabled = false;
            taxis = 0;

            var exists = _repository.Exists(Q
                .Where(Attr.PluginId, pluginId));

            if (!exists)
            {
                _repository.Insert(new PluginInfo
                {
                    PluginId = pluginId,
                    IsDisabled = false,
                    Taxis = 0
                });
            }

            var result = _repository.Get<(string IsDisabled, int Taxis)?>(Q
                .Select(Attr.IsDisabled, Attr.Taxis)
                .Where(Attr.PluginId, pluginId));

            if (result == null) return;

            isDisabled = TranslateUtils.ToBool(result.Value.IsDisabled);
            taxis = result.Value.Taxis;
        }
    }
}
