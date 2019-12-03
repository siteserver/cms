using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Repositories
{
    public class PluginRepository : IRepository
    {
        private readonly Repository<Abstractions.Plugin> _repository;

        public PluginRepository()
        {
            _repository = new Repository<Abstractions.Plugin>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task DeleteAsync(string pluginId)
        {
            await _repository.DeleteAsync(Q.Where(nameof(Abstractions.Plugin.PluginId), pluginId));
        }

        public async Task UpdateIsDisabledAsync(string pluginId, bool isDisabled)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Abstractions.Plugin.IsDisabled), isDisabled.ToString())
                .Where(nameof(Abstractions.Plugin.PluginId), pluginId)
            );
        }

        public async Task UpdateTaxisAsync(string pluginId, int taxis)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Abstractions.Plugin.Taxis), taxis)
                .Where(nameof(Abstractions.Plugin.PluginId), pluginId)
            );
        }

        public async Task<(bool IsDisabled, int Taxis)> SetIsDisabledAndTaxisAsync(string pluginId)
        {
            var exists = await _repository.ExistsAsync(Q.Where(nameof(Abstractions.Plugin.PluginId), pluginId));

            if (!exists)
            {
                await _repository.InsertAsync(new Abstractions.Plugin
                {
                    PluginId = pluginId,
                    Disabled = false,
                    Taxis = 0
                });
            }

            var pluginEntity = await _repository.GetAsync(Q.Where(nameof(Abstractions.Plugin.PluginId), pluginId));

            return (pluginEntity.Disabled, pluginEntity.Taxis);
        }
    }
}
