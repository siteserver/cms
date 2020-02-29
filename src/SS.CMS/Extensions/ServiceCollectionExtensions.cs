using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CacheManager.Core;
using Datory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SS.CMS.Abstractions;
using SS.CMS.Services;

namespace SS.CMS.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static ISettingsManager AddSettingsManager(this IServiceCollection services, IConfiguration configuration, string contentRootPath, string webRootPath)
        {
            var settingsManager = new SettingsManager(configuration, contentRootPath, webRootPath);
            services.TryAdd(ServiceDescriptor.Singleton<ISettingsManager>(settingsManager));

            return settingsManager;
        }

        public static IServiceCollection AddCache(this IServiceCollection services, string redisConnectionString)
        {
            services.AddCacheManagerConfiguration(async settings =>
            {
                var isBackPlane = false;
                if (!string.IsNullOrEmpty(redisConnectionString))
                {
                    var redis = new Redis(redisConnectionString);
                    var (isConnectionWorks, _) = await redis.IsConnectionWorksAsync();
                    if (isConnectionWorks)
                    {
                        settings
                            .WithMicrosoftMemoryCacheHandle()
                            .WithExpiration(ExpirationMode.None, TimeSpan.Zero)
                            .And
                            .WithRedisConfiguration("redis", config =>
                            {
                                if (!string.IsNullOrEmpty(redis.Password))
                                {
                                    config.WithPassword(redis.Password);
                                }
                                if (redis.AllowAdmin)
                                {
                                    config.WithAllowAdmin();
                                }

                                config
                                    .WithDatabase(redis.Database)
                                    .WithEndpoint(redis.Host, redis.Port);
                            })
                            .WithMaxRetries(1000)
                            .WithRetryTimeout(100)
                            .WithJsonSerializer()
                            .WithRedisBackplane("redis")
                            .WithRedisCacheHandle("redis", true);

                        isBackPlane = true;
                    }
                }

                if (!isBackPlane)
                {
                    settings
                        .WithMicrosoftMemoryCacheHandle()
                        .WithExpiration(ExpirationMode.None, TimeSpan.Zero);
                }
            });
            services.AddCacheManager();
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            var baseType = typeof(IRepository);

            var path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var referencedAssemblies = Directory.GetFiles(path, "SS.*.dll").Select(Assembly.LoadFrom).ToArray();
            var types = referencedAssemblies
                .SelectMany(a => a.DefinedTypes)
                .Select(type => type.AsType())
                .Where(x => x != baseType && baseType.IsAssignableFrom(x)).ToArray();
            var implementTypes = types.Where(x => x.IsClass).ToArray();
            var interfaceTypes = types.Where(x => x.IsInterface).ToArray();
            foreach (var implementType in implementTypes)
            {
                var interfaceType = interfaceTypes.FirstOrDefault(x => x.IsAssignableFrom(implementType));
                if (interfaceType != null)
                    services.AddScoped(interfaceType, implementType);
            }

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthManager, AuthManager>();
            services.AddScoped<IPathManager, PathManager>();
            services.AddScoped<ICreateManager, CreateManager>();
            services.AddScoped<IDatabaseManager, DatabaseManager>();
            services.AddScoped<IParseManager, ParseManager>();
            services.AddScoped<IPluginManager, PluginManager>();

            return services;
        }

        //public static IServiceCollection AddDatabase(this IServiceCollection services, DatabaseType databaseType, string connectionString)
        //{
        //    services.TryAdd(ServiceDescriptor.Singleton<IDatabase>(sp =>
        //    {
        //        return new Database(databaseType, connectionString);
        //    }));

        //    return services;
        //}





        //public static IServiceCollection AddPathManager(this IServiceCollection services)
        //{
        //    services.TryAdd(ServiceDescriptor.Singleton<IPathManager, PathManager>());

        //    return services;
        //}

        //public static IServiceCollection AddRepositories(this IServiceCollection services)
        //{
        //    services.TryAdd(ServiceDescriptor.Transient<IAccessTokenRepository, AccessTokenRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IAreaRepository, AreaRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IChannelGroupRepository, ChannelGroupRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IChannelRepository, ChannelRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IConfigRepository, ConfigRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IContentCheckRepository, ContentCheckRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IContentGroupRepository, ContentGroupRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IDatabaseRepository, DatabaseRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IDbCacheRepository, DbCacheRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IDepartmentRepository, DepartmentRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IErrorLogRepository, ErrorLogRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<ILogRepository, LogRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IPermissionRepository, PermissionRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IPluginConfigRepository, PluginConfigRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IPluginRepository, PluginRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IRelatedFieldItemRepository, RelatedFieldItemRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IRelatedFieldRepository, RelatedFieldRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IRoleRepository, RoleRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<ISiteLogRepository, SiteLogRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<ISiteRepository, SiteRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<ISpecialRepository, SpecialRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<ITableStyleItemRepository, TableStyleItemRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<ITableStyleRepository, TableStyleRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<ITagRepository, TagRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<ITemplateLogRepository, TemplateLogRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<ITemplateRepository, TemplateRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IUserGroupRepository, UserGroupRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IUserLogRepository, UserLogRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IUserMenuRepository, UserMenuRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IUserRepository, UserRepository>());
        //    services.TryAdd(ServiceDescriptor.Transient<IUserRoleRepository, UserRoleRepository>());

        //    return services;
        //}

        //public static IServiceCollection AddFileManager(this IServiceCollection services)
        //{
        //    services.TryAdd(ServiceDescriptor.Singleton<IFileManager, FileManager>());

        //    return services;
        //}

        //public static IServiceCollection AddCreateManager(this IServiceCollection services)
        //{
        //    services.TryAdd(ServiceDescriptor.Singleton<ICreateManager, CreateManager>());

        //    return services;
        //}

        //public static IServiceCollection AddPluginManager(this IServiceCollection services)
        //{
        //    services.TryAdd(ServiceDescriptor.Singleton<IPluginManager, PluginManager>());

        //    return services;
        //}
    }
}