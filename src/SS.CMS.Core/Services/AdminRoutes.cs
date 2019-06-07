using Microsoft.Extensions.DependencyInjection;
using SS.CMS.Core.Services.Admin;
using SS.CMS.Core.Services.Admin.Cms;

namespace SS.CMS.Core.Services
{
    public static class AdminRoutes
    {
        public const string Prefix = "admin";
        public const string PrefixSettingsAdmin = "admin/settings/admin";

        public static void AddSingleton(IServiceCollection services)
        {
            services.AddSingleton<RootService>();
            services.AddSingleton<CreateStatusService>();
            services.AddSingleton<IndexService>();
            services.AddSingleton<LoginService>();
            services.AddSingleton<SyncService>();
        }
    }
}
