using System.Threading.Tasks;

namespace SSCMS.Cli.Abstractions
{
    public interface IJobService
    {
        string CommandName { get; }
        void PrintUsage();
        Task ExecuteAsync(IJobContext context);
    }
}
