using System.Threading.Tasks;
using SSCMS.Context;

namespace SSCMS.Plugins
{
    public interface IPluginCreateStartAsync : IPluginExtension
    {
        Task ParseAsync(IParseContext context);
    }
}
