using System.Threading.Tasks;

namespace SSCMS.Plugins
{
    public interface IPluginBeforeStlParseAsync : IPluginExtension
    {
        Task BeforeStlParseAsync(IStlParseContext context);
    }
}
