using System.Threading.Tasks;

namespace SSCMS.Cli.Abstractions
{
  public partial interface IApiService
    {
        Task<(bool success, string failureMessage)> ThemePublishAsync(string zipPath);
    }
}
