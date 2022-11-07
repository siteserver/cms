using System.Threading.Tasks;

namespace SSCMS.Cli.Abstractions
{
    public partial interface ICliApiService
    {
        Task<(bool success, string failureMessage)> LoginAsync(string account, string password);
    }
}
