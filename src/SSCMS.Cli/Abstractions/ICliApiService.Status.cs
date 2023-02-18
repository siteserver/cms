using System.Threading.Tasks;
using SSCMS.Cli.Models;

namespace SSCMS.Cli.Abstractions
{
    public partial interface ICliApiService
    {
        Task<(ConfigStatus status, string failureMessage)> GetStatusAsync();
    }
}
