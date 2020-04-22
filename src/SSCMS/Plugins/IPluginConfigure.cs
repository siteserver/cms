using Microsoft.AspNetCore.Builder;

namespace SSCMS.Plugins
{
    public interface IPluginConfigure : IPluginExtension
    {
        void Configure(IApplicationBuilder app);
    }
}
