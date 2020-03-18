using System.Threading.Tasks;
using SSCMS.Abstractions;

namespace SSCMS.Cli.Core
{
    public interface IJobService
    {
        string CommandName { get; }
        void PrintUsage();

        Task ExecuteAsync(IJobContext context);
    }
}
