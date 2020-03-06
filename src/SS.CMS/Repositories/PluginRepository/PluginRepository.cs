using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions;

namespace SS.CMS.Repositories
{
    public class PluginRepository : IPluginRepository
    {
        private readonly Repository<Plugin> _repository;

        public PluginRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<Plugin>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task DeleteAsync(string pluginId)
        {
            await _repository.DeleteAsync(Q.Where(nameof(Plugin.PluginId), pluginId));
        }

        public async Task UpdateIsDisabledAsync(string pluginId, bool isDisabled)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Plugin.Disabled), isDisabled)
                .Where(nameof(Plugin.PluginId), pluginId)
            );
        }

        public async Task UpdateTaxisAsync(string pluginId, int taxis)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Plugin.Taxis), taxis)
                .Where(nameof(Plugin.PluginId), pluginId)
            );
        }

        public async Task<(bool IsDisabled, int Taxis)> SetIsDisabledAndTaxisAsync(string pluginId)
        {
            var exists = await _repository.ExistsAsync(Q.Where(nameof(Plugin.PluginId), pluginId));

            if (!exists)
            {
                await _repository.InsertAsync(new Plugin
                {
                    PluginId = pluginId,
                    Disabled = false,
                    Taxis = 0
                });
            }

            var pluginEntity = await _repository.GetAsync(Q.Where(nameof(Plugin.PluginId), pluginId));

            return (pluginEntity.Disabled, pluginEntity.Taxis);
        }
    }
}
