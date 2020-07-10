using System.Threading.Tasks;
using SSCMS.Context;

namespace SSCMS.Plugins
{
    public interface IPluginParseAsync : IPluginExtension
    {
        string ElementName { get; }
        Task<string> ParseAsync(IParseStlContext context);
    }
}
