using System.Threading.Tasks;
using SSCMS.Plugins;

namespace SSCMS.Cli.Abstractions
{
    public interface IJobService
    {
        string CommandName { get; }
        void PrintUsage();
        Task ExecuteAsync(IPluginJobContext context);
    }
}
