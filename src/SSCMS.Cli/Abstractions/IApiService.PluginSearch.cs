using System.Collections.Generic;
using System.Threading.Tasks;
using static SSCMS.Cli.Services.ApiService;

namespace SSCMS.Cli.Abstractions
{
  public partial interface IApiService
    {
        Task<(bool success, List<PluginAndUser> results, string failureMessage)> PluginSearchAsync(string word);
    }
}
