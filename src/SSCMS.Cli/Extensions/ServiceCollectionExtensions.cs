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
            services.AddScoped<IDataUpdateService, DataUpdateService>();
            services.AddScoped<IDataRestoreService, DataRestoreService>();
        }

        public static void AddCliJobs(this IServiceCollection services)
        {
            services.AddScoped<IJobService, DataBackupJob>();
            services.AddScoped<IJobService, DataRestoreJob>();
            services.AddScoped<IJobService, DataSyncJob>();
            services.AddScoped<IJobService, DataUpdateJob>();
            services.AddScoped<IJobService, InstallDownloadJob>();
            services.AddScoped<IJobService, InstallDatabaseJob>();
            services.AddScoped<IJobService, LoginJob>();
            services.AddScoped<IJobService, LogoutJob>();
            services.AddScoped<IJobService, PluginNewJob>();
            services.AddScoped<IJobService, PluginPackageJob>();
            services.AddScoped<IJobService, PluginPublishJob>();
            services.AddScoped<IJobService, PluginSearchJob>();
            services.AddScoped<IJobService, PluginShowJob>();
            services.AddScoped<IJobService, PluginUnPublishJob>();
            services.AddScoped<IJobService, RegisterJob>();
            services.AddScoped<IJobService, RunJob>();
            services.AddScoped<IJobService, StatusJob>();
            services.AddScoped<IJobService, ThemePackageJob>();
            services.AddScoped<IJobService, ThemePublishJob>();
            services.AddScoped<IJobService, ThemeUnPublishJob>();
            services.AddScoped<IJobService, UpdateJob>();
        }
    }
}