using Microsoft.Extensions.DependencyInjection;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Jobs;
using SSCMS.Cli.Services;

namespace SSCMS.Cli.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCliServices(this IServiceCollection services)
        {
            services.AddScoped<IApiService, ApiService>();
            services.AddScoped<IConfigService, ConfigService>();
            services.AddScoped<IUpdateService, UpdateService>();
            services.AddScoped<IRestoreService, RestoreService>();
        }

        public static void AddCliJobs(this IServiceCollection services)
        {
            services.AddScoped<IJobService, BackupJob>();
            services.AddScoped<IJobService, InstallJob>();
            services.AddScoped<IJobService, LoginJob>();
            services.AddScoped<IJobService, LogoutJob>();
            services.AddScoped<IJobService, PluginPackageJob>();
            services.AddScoped<IJobService, PluginPublishJob>();
            services.AddScoped<IJobService, PluginSearchJob>();
            services.AddScoped<IJobService, PluginShowJob>();
            services.AddScoped<IJobService, PluginUnPublishJob>();
            services.AddScoped<IJobService, RegisterJob>();
            services.AddScoped<IJobService, RestoreJob>();
            services.AddScoped<IJobService, StatusJob>();
            services.AddScoped<IJobService, SyncJob>();
            services.AddScoped<IJobService, UpdateJob>();
        }
    }
}