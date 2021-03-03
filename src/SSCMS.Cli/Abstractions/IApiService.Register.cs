using System.Threading.Tasks;

namespace SSCMS.Cli.Abstractions
{
  public partial interface IApiService
    {
        Task<(bool success, string failureMessage)> RegisterAsync(string userName, string mobile, string email, string password);
    }
}
