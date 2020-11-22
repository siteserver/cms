using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Cli.Models;
using SSCMS.Cli.Services;

namespace SSCMS.Cli.Abstractions
{
    public interface IApiService
    {
        (ConfigStatus status, string failureMessage) GetStatus();

        Task<(bool success, string failureMessage)> LoginAsync(string account, string password);

        (bool success, string failureMessage) Register(string userName, string mobile, string email, string password);

        (bool success, string failureMessage) PluginPublish(string publisher, string zipPath);

        (bool success, string failureMessage) PluginUnPublish(string packageId);

        (bool success, List<ApiService.PluginAndUser> results, string failureMessage) PluginSearch(string word);

        (bool success, ApiService.PluginAndUser result, string failureMessage) PluginShow(string pluginId);

        (bool success, string failureMessage) ThemePublish(string zipPath);

        (bool success, string failureMessage) ThemeUnPublish(string name);

        (bool success, ApiService.GetReleasesResult result, string failureMessage) GetReleases(string version, List<string> pluginIds);
    }
}
