using Microsoft.Extensions.DependencyInjection;

namespace SSCMS.Plugins
{
    public interface IPluginConfigureServices : IPluginExtension
    {
        void ConfigureServices(IServiceCollection services);
    }
}
