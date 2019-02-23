namespace SiteServer.Plugin
{
    /// <summary>
    /// 插件运行环境接口。
    /// </summary>
    public interface IEnvironment
    {
        /// <summary>
        /// 系统使用的数据库类型。
        /// </summary>
        DatabaseType DatabaseType { get; }

        /// <summary>
        /// 系统使用的数据库连接字符串。
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// 网站根目录文件夹地址。
        /// </summary>
        string PhysicalApplicationPath { get; }
    }
}
