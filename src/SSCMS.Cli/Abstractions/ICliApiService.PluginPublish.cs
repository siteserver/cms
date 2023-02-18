using System.Threading.Tasks;

namespace SSCMS.Cli.Abstractions
{
    public partial interface ICliApiService
    {
        Task<(bool success, string failureMessage)> PluginPublishAsync(string publisher, string zipPath);
    }
}
