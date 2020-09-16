using System.Threading.Tasks;
using SSCMS.Parse;

namespace SSCMS.Plugins
{
    public interface IPluginCreateEndAsync : IPluginExtension
    {
        Task ParseAsync(IParseContext context);
    }
}
