using Microsoft.Extensions.DependencyInjection;

namespace SSCMS.Plugins
{
    public interface IPluginAfterStlParse : IPluginExtension
    {
        void AfterStlParse(IStlParseContext context);
    }
}
