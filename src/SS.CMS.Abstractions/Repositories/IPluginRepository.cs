using System.Threading.Tasks;
using SS.CMS.Data;

namespace SS.CMS.Repositories
{
    public interface IPluginRepository : IRepository
    {
        Task DeleteByIdAsync(string pluginId);

        void UpdateIsDisabled(string pluginId, bool isDisabled);

        void UpdateTaxis(string pluginId, int taxis);

        void SetIsDisabledAndTaxis(string pluginId, out bool isDisabled, out int taxis);
    }
}
