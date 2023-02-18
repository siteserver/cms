using System.Threading.Tasks;
using static SSCMS.Cli.Services.CliApiService;

namespace SSCMS.Cli.Abstractions
{
    public partial interface ICliApiService
    {
        Task<(bool success, PluginAndUser result, string failureMessage)> PluginShowAsync(string pluginId);
    }
}
