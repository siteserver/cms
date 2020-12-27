using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CacheManager.Core;
using Datory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Senparc.CO2NET.RegisterServices;
using SSCMS.Core.Services;
using SSCMS.Services;

namespace SSCMS.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static ISettingsManager AddSettingsManager(this IServiceCollection services, IConfiguration configuration, string contentRootPath, string webRootPath, Assembly entryAssembly)
        {
            var settingsManager = new SettingsManager(services, configuration, contentRootPath, webRootPath, entryAssembly);
            services.TryAdd(ServiceDescriptor.Singleton<ISettingsManager>(settingsManager));

            return settingsManager;
        }

        public static void AddCache(this IServiceCollection services, string redisConnectionString)
        {
            services.AddCacheManagerConfiguration(async settings =>
            {
                var isRedis = false;
                if (!string.IsNullOrEmpty(redisConnectionString))
                {
                    var redis = new Redis(redisConnectionString);
                    var (isConnectionWorks, _) = await redis.IsConnectionWorksAsync();
                    if (isConnectionWorks)
                    {
                        settings
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
                            .WithRedisCacheHandle("redis");

                        isRedis = true;
                    }
                }

                if (!isRedis)
                {
                    settings
                        .WithMicrosoftMemoryCacheHandle()
                        .WithExpiration(ExpirationMode.None, TimeSpan.Zero);
                }
            });
            services.AddCacheManager();
        }

        public static void AddRepositories(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            var baseType = typeof(IRepository);

            var types = assemblies
                .SelectMany(a => a.DefinedTypes)
                .Select(type => type.AsType())
                .Where(x => x != baseType && baseType.IsAssignableFrom(x)).ToArray();
            var implementTypes = types.Where(x => x.IsClass).ToArray();
            var interfaceTypes = types.Where(x => x.IsInterface).ToArray();
            foreach (var implementType in implementTypes)
            {
                var interfaceType = interfaceTypes.FirstOrDefault(x => x.IsAssignableFrom(implementType));
                if (interfaceType != null)
                {
                    //if (interfaceType == typeof(IContentRepository)) continue;

                    services.AddScoped(interfaceType, implementType);
                }
            }
        }

        public static void AddTaskQueue(this IServiceCollection services)
        {
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<ITaskManager, TaskManager>();
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<ICacheManager, Services.CacheManager>();
            services.AddScoped<IAuthManager, AuthManager>();
            services.AddScoped<IPathManager, PathManager>();
            services.AddScoped<ICreateManager, CreateManager>();
            services.AddScoped<IDatabaseManager, DatabaseManager>();
            services.AddScoped<IParseManager, ParseManager>();
        }

        public static void AddPseudoServices(this IServiceCollection services)
        {
            services.AddScoped<ICensorManager, CensorManager>();
            services.AddScoped<IMailManager, MailManager>();
            services.AddScoped<ISmsManager, SmsManager>();
        }

        public static void AddWxManager(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSenparcGlobalServices(configuration);
            services.AddScoped<IWxManager, WxManager>();
        }
    }
}