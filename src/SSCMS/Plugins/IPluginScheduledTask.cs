using System.Threading.Tasks;

namespace SSCMS.Plugins
{
    public interface IPluginScheduledTask : IPluginExtension
    {
        string TaskType { get; }
        Task ExecuteAsync(string settings);
    }
}
