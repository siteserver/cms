namespace SiteServer.Plugin
{
    /// <summary>
    /// 插件可调用的Api集合接口。
    /// </summary>
    public interface IApiCollection
    {
        /// <summary>
        /// 管理员及权限Api接口。
        /// </summary>
        IAdminApi AdminApi { get; }

        /// <summary>
        /// 插件及系统配置Api接口。
        /// </summary>
        IConfigApi ConfigApi { get; }

        /// <summary>
        /// 内容Api接口。
        /// </summary>
        IContentApi ContentApi { get; }

        /// <summary>
        /// 数据库操作Api接口。
        /// </summary>
        IDatabaseApi DatabaseApi { get; }

        /// <summary>
        /// 栏目Api接口。
        /// </summary>
        IChannelApi ChannelApi { get; }

        /// <summary>
        /// STL解析Api接口。
        /// </summary>
        IParseApi ParseApi { get; }

        /// <summary>
        /// 插件Api接口。
        /// </summary>
        IPluginApi PluginApi { get; }

        /// <summary>
        /// 站点Api接口。
        /// </summary>
        ISiteApi SiteApi { get; }

        /// <summary>
        /// 用户Api接口。
        /// </summary>
        IUserApi UserApi { get; }

        /// <summary>
        /// 工具类Api接口。
        /// </summary>
        IUtilsApi UtilsApi { get; }
    }
}
