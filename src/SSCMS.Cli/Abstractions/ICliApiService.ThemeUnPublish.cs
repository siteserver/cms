using System.Threading.Tasks;

namespace SSCMS.Cli.Abstractions
{
    public partial interface ICliApiService
    {
        Task<(bool success, string failureMessage)> ThemeUnPublishAsync(string name);
    }
}
