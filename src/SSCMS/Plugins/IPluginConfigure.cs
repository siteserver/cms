using Microsoft.AspNetCore.Builder;

namespace SSCMS.Plugins
{
    public interface IPluginConfigure
    {
        void Configure(IApplicationBuilder app);
    }
}
