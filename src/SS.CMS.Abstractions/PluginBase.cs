using System;
using System.Collections.Generic;

namespace SS.CMS.Abstractions
{
    /// <summary>
    /// 插件父类，所有插件必须继承此类并实现Startup方法。
    /// </summary>
    public abstract class PluginBase : Initializer, IPackageMetadata
    {
        /// <summary>
        /// 初始化插件。
        /// 此方法将由 SS CMS 系统载入插件时调用。
        /// </summary>
        /// <param name="metadata">插件元数据接口。</param>
        public sealed override void Initialize(IPackageMetadata metadata)
        {
            Id = metadata.Id;
            Version = metadata.Version;
            IconUrl = metadata.IconUrl;
            ProjectUrl = metadata.ProjectUrl;
            LicenseUrl = metadata.LicenseUrl;
            Copyright = metadata.Copyright;
            Description = metadata.Description;
            ReleaseNotes = metadata.ReleaseNotes;
            RequireLicenseAcceptance = metadata.RequireLicenseAcceptance;
            Summary = metadata.Summary;
            Title = metadata.Title;
            Tags = metadata.Tags;
            Authors = metadata.Authors;
            Owners = metadata.Owners;
            Language = metadata.Language;
        }

        /// <summary>
        /// Startup方法是插件机制的核心，用于定义插件能够提供的各种服务。
        /// </summary>
        /// <param name="service">插件服务注册接口。</param>
        public abstract void Startup(IPluginService service);

        /// <inheritdoc />
        public string Id { get; private set; }

        /// <inheritdoc />
        public string Version { get; private set; }

        /// <inheritdoc />
        public Uri IconUrl { get; private set; }

        /// <inheritdoc />
        public Uri ProjectUrl { get; private set; }

        /// <inheritdoc />
        public Uri LicenseUrl { get; private set; }

        /// <inheritdoc />
        public string Copyright { get; private set; }

        /// <inheritdoc />
        public string Description { get; private set; }

        /// <inheritdoc />
        public string ReleaseNotes { get; private set; }

        /// <inheritdoc />
        public bool RequireLicenseAcceptance { get; private set; }

        /// <inheritdoc />
        public string Summary { get; private set; }

        /// <inheritdoc />
        public string Title { get; private set; }

        /// <inheritdoc />
        public string Tags { get; private set; }

        /// <inheritdoc />
        public List<string> Authors { get; private set; }

        /// <inheritdoc />
        public string Owners { get; private set; }

        /// <inheritdoc />
        public string Language { get; private set; }

        ///// <inheritdoc />
        //public DatabaseType DatabaseType { get; private set; }

        ///// <inheritdoc />
        //public string ConnectionString { get; private set; }

        ///// <inheritdoc />
        //public string AdminDirectory { get; private set; }

        ///// <inheritdoc />
        //public string PhysicalApplicationPath { get; private set; }

        ///// <inheritdoc />
        //public IRequest Request => _environment.Request;

        ///// <inheritdoc />
        //public IAdminApi AdminApi { get; private set; }

        ///// <inheritdoc />
        //public IConfigApi ConfigApi { get; private set; }

        ///// <inheritdoc />
        //public IContentApi ContentApi { get; private set; }

        ///// <inheritdoc />
        //public IDatabaseApi DatabaseApi { get; private set; }

        ///// <inheritdoc />
        //public IChannelApi ChannelApi { get; private set; }

        ///// <inheritdoc />
        //public IParseApi ParseApi { get; private set; }

        ///// <inheritdoc />
        //public IPluginApi PluginApi { get; private set; }

        ///// <inheritdoc />
        //public ISiteApi SiteApi { get; private set; }

        ///// <inheritdoc />
        //public IUserApi UserApi { get; private set; }

        ///// <inheritdoc />
        //public IUtilsApi UtilsApi { get; private set; }
    }
}