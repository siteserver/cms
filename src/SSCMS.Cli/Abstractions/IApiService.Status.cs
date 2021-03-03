using System.Threading.Tasks;
using SSCMS.Cli.Models;

namespace SSCMS.Cli.Abstractions
{
  public partial interface IApiService
    {
        Task<(ConfigStatus status, string failureMessage)> GetStatusAsync();
    }
}
