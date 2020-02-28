using System.Threading.Tasks;
using SS.CMS.Abstractions;

namespace SS.CMS.Cli.Core
{
    public interface IJobService
    {
        string CommandName { get; }
        void PrintUsage();

        Task ExecuteAsync(IJobContext context);
    }
}
