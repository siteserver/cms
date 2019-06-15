using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface IPluginRepository : IRepository
    {
        void DeleteById(string pluginId);

        void UpdateIsDisabled(string pluginId, bool isDisabled);

        void UpdateTaxis(string pluginId, int taxis);

        void SetIsDisabledAndTaxis(string pluginId, out bool isDisabled, out int taxis);
    }
}
