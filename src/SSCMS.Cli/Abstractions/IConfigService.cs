using System.Threading.Tasks;
using SSCMS.Cli.Models;

namespace SSCMS.Cli.Abstractions
{
    public interface IConfigService
    {
        public ConfigStatus Status { get; }

        Task SaveStatusAsync(ConfigStatus status);
    }
}
