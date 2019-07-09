using System.Threading.Tasks;
using SS.CMS.Data;

namespace SS.CMS.Repositories
{
    public interface IPluginRepository : IRepository
    {
        Task DeleteByIdAsync(string pluginId);

        Task UpdateIsDisabledAsync(string pluginId, bool isDisabled);

        Task UpdateTaxisAsync(string pluginId, int taxis);

        Task<(bool IsDisabled, int Taxis)> SetIsDisabledAndTaxisAsync(string pluginId);
    }
}
