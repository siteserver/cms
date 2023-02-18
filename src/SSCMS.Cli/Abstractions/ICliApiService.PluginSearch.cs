using System.Collections.Generic;
using System.Threading.Tasks;
using static SSCMS.Cli.Services.CliApiService;

namespace SSCMS.Cli.Abstractions
{
    public partial interface ICliApiService
    {
        Task<(bool success, List<PluginAndUser> results, string failureMessage)> PluginSearchAsync(string word);
    }
}
