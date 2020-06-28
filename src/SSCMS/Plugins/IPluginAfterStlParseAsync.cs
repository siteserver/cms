using System.Threading.Tasks;

namespace SSCMS.Plugins
{
    public interface IPluginAfterStlParseAsync : IPluginExtension
    {
        Task AfterStlParseAsync(IStlParseContext context);
    }
}
