using System.Threading.Tasks;
using SSCMS.Plugins;
using SSCMS.Utils;

namespace SSCMS.Cli.Abstractions
{
    public interface IJobService
    {
        string CommandName { get; }
        Task WriteUsageAsync(IConsoleUtils console);
        Task ExecuteAsync(IPluginJobContext context);
    }
}
