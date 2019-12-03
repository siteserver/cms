using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public interface IPluginRepository : IRepository
    {
        Task DeleteByIdAsync(string pluginId);

        Task UpdateIsDisabledAsync(string pluginId, bool isDisabled);

        Task UpdateTaxisAsync(string pluginId, int taxis);

        Task<(bool IsDisabled, int Taxis)> SetIsDisabledAndTaxisAsync(string pluginId);
    }
}
