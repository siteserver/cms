using System.Threading.Tasks;
using SSCMS.Parse;

namespace SSCMS.Plugins
{
    public interface IPluginCreateStartAsync : IPluginExtension
    {
        Task ParseAsync(IParseContext context);
    }
}
