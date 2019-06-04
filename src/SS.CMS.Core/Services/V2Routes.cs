using Microsoft.Extensions.DependencyInjection;
using SS.CMS.Core.Services.V2;

namespace SS.CMS.Core.Services
{
    public static class V2Routes
    {
        public const string Prefix = "v2";

        public static void AddSingleton(IServiceCollection services)
        {
            services.AddSingleton<CaptchaService>();
        }
    }
}
