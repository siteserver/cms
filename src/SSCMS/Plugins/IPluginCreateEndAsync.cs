using System.Threading.Tasks;
using SSCMS.Context;

namespace SSCMS.Plugins
{
    public interface IPluginCreateEndAsync : IPluginExtension
    {
        Task ParseAsync(IParseContext context);
    }
}
