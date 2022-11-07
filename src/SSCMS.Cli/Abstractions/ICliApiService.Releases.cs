using System.Collections.Generic;
using System.Threading.Tasks;
using static SSCMS.Cli.Services.CliApiService;

namespace SSCMS.Cli.Abstractions
{
    public partial interface ICliApiService
    {
        Task<(bool success, GetReleasesResult result, string failureMessage)> GetReleasesAsync(string version, List<string> pluginIds);
    }
}
