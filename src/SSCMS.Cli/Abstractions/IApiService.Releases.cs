using System.Collections.Generic;
using System.Threading.Tasks;
using static SSCMS.Cli.Services.ApiService;

namespace SSCMS.Cli.Abstractions
{
  public partial interface IApiService
    {
        Task<(bool success, GetReleasesResult result, string failureMessage)> GetReleasesAsync(string version, List<string> pluginIds);
    }
}
