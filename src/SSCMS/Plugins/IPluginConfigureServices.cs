using Microsoft.Extensions.DependencyInjection;

namespace SSCMS.Plugins
{
    public interface IPluginConfigureServices
    {
        void ConfigureServices(IServiceCollection services);
    }
}
