using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Repositories;

namespace SS.CMS.Core.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSettingsManager(this IServiceCollection services, IConfiguration configuration, string contentRootPath, string webRootPath)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.TryAdd(ServiceDescriptor.Singleton<ISettingsManager>(new SettingsManager(configuration, contentRootPath, webRootPath)));

            return services;
        }

        public static IServiceCollection AddCacheManager(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.TryAdd(ServiceDescriptor.Singleton<ICacheManager, CacheManager>());

            return services;
        }

        public static IServiceCollection AddUrlManager(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.TryAdd(ServiceDescriptor.Singleton<IUrlManager, UrlManager>());

            return services;
        }

        public static IServiceCollection AddPathManager(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.TryAdd(ServiceDescriptor.Singleton<IPathManager, PathManager>());

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.TryAdd(ServiceDescriptor.Transient<IAccessTokenRepository, AccessTokenRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IAdministratorRepository, AdministratorRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IAdministratorsInRolesRepository, AdministratorsInRolesRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IAreaRepository, AreaRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IChannelGroupRepository, ChannelGroupRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IChannelRepository, ChannelRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IConfigRepository, ConfigRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IContentCheckRepository, ContentCheckRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IContentGroupRepository, ContentGroupRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IDbCacheRepository, DbCacheRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IDepartmentRepository, DepartmentRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IErrorLogRepository, ErrorLogRepository>());
            services.TryAdd(ServiceDescriptor.Transient<ILogRepository, LogRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IPermissionsInRolesRepository, PermissionsInRolesRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IPluginConfigRepository, PluginConfigRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IPluginRepository, PluginRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IRelatedFieldItemRepository, RelatedFieldItemRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IRelatedFieldRepository, RelatedFieldRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IRoleRepository, RoleRepository>());
            services.TryAdd(ServiceDescriptor.Transient<ISiteLogRepository, SiteLogRepository>());
            services.TryAdd(ServiceDescriptor.Transient<ISitePermissionsRepository, SitePermissionsRepository>());
            services.TryAdd(ServiceDescriptor.Transient<ISiteRepository, SiteRepository>());
            services.TryAdd(ServiceDescriptor.Transient<ISpecialRepository, SpecialRepository>());
            services.TryAdd(ServiceDescriptor.Transient<ITableStyleItemRepository, TableStyleItemRepository>());
            services.TryAdd(ServiceDescriptor.Transient<ITableStyleRepository, TableStyleRepository>());
            services.TryAdd(ServiceDescriptor.Transient<ITagRepository, TagRepository>());
            services.TryAdd(ServiceDescriptor.Transient<ITemplateLogRepository, TemplateLogRepository>());
            services.TryAdd(ServiceDescriptor.Transient<ITemplateRepository, TemplateRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IUserGroupRepository, UserGroupRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IUserLogRepository, UserLogRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IUserMenuRepository, UserMenuRepository>());
            services.TryAdd(ServiceDescriptor.Transient<IUserRepository, UserRepository>());

            return services;
        }

        public static IServiceCollection AddMenuManager(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.TryAdd(ServiceDescriptor.Singleton<IMenuManager, MenuManager>());

            return services;
        }

        public static IServiceCollection AddFileManager(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.TryAdd(ServiceDescriptor.Singleton<IFileManager, FileManager>());

            return services;
        }

        public static IServiceCollection AddCreateManager(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.TryAdd(ServiceDescriptor.Singleton<ICreateManager, CreateManager>());

            return services;
        }

        public static IServiceCollection AddIdentityManager(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.TryAdd(ServiceDescriptor.Scoped<IIdentityManager, IdentityManager>());

            return services;
        }

        public static IServiceCollection AddPluginManager(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.TryAdd(ServiceDescriptor.Singleton<IPluginManager, PluginManager>());

            return services;
        }
    }
}
