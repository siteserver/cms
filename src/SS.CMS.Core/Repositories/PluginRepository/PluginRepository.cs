using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.Repositories
{
    public class PluginRepository : IPluginRepository
    {
        private readonly Repository<PluginInfo> _repository;
        private readonly ISettingsManager _settingsManager;

        public PluginRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<PluginInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _settingsManager = settingsManager;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string PluginId = nameof(PluginInfo.PluginId);
            public const string IsDisabled = nameof(PluginInfo.IsDisabled);
            public const string Taxis = nameof(PluginInfo.Taxis);
        }

        public async Task DeleteByIdAsync(string pluginId)
        {
            await _repository.DeleteAsync(Q.Where(Attr.PluginId, pluginId));
        }

        public async Task UpdateIsDisabledAsync(string pluginId, bool isDisabled)
        {
            await _repository.UpdateAsync(Q
                .Set(Attr.IsDisabled, isDisabled.ToString())
                .Where(Attr.PluginId, pluginId)
            );
        }

        public async Task UpdateTaxisAsync(string pluginId, int taxis)
        {
            await _repository.UpdateAsync(Q
                .Set(Attr.Taxis, taxis)
                .Where(Attr.PluginId, pluginId)
            );
        }

        public async Task<(bool IsDisabled, int Taxis)> SetIsDisabledAndTaxisAsync(string pluginId)
        {
            var exists = await _repository.ExistsAsync(Q
                .Where(Attr.PluginId, pluginId));

            if (!exists)
            {
                await _repository.InsertAsync(new PluginInfo
                {
                    PluginId = pluginId,
                    IsDisabled = false,
                    Taxis = 0
                });
            }

            var result = await _repository.GetAsync<(bool IsDisabled, int Taxis)?>(Q
                .Select(Attr.IsDisabled, Attr.Taxis)
                .Where(Attr.PluginId, pluginId));

            if (result != null)
            {
                return result.Value;
            }

            return (false, 0);
        }
    }
}
