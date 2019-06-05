using System;
using SS.CMS.Data;

namespace SS.CMS.Plugin
{
    /// <summary>
    /// 封装系统上下文相关信息
    /// </summary>
    public static class Context
    {
        private static IEnvironment _environment;
        public static IApiCollection ApiCollection { get; private set; }
        private static Func<IRequest> _requestFunc;
        private static Func<IResponse> _responseFunc;

        /// <summary>
        /// 初始化上下文
        /// </summary>
        /// <param name="environment">环境变量接口。</param>
        /// <param name="apiCollection">API类集合接口。</param>
        public static void Initialize(IEnvironment environment, IApiCollection apiCollection, Func<IRequest> requestFunc, Func<IResponse> responseFunc)
        {
            if (_environment == null)
            {
                _environment = environment;
            }

            if (ApiCollection == null)
            {
                ApiCollection = apiCollection;
            }

            if (_requestFunc == null)
            {
                _requestFunc = requestFunc;
            }

            if (_responseFunc == null)
            {
                _responseFunc = responseFunc;
            }
        }

        /// <summary>
        /// 系统使用的数据库类型。
        /// </summary>
        public static DatabaseType DatabaseType => _environment.DatabaseType;

        /// <summary>
        /// 系统使用的数据库连接字符串。
        /// </summary>
        public static string ConnectionString => _environment.ConnectionString;

        /// <summary>
        /// 用户中心文件夹名称。
        /// </summary>
        public static string HomeDirectory => _environment.HomeDirectory;

        /// <summary>
        /// 管理后台文件夹名称。
        /// </summary>
        public static string AdminDirectory => _environment.AdminDirectory;

        /// <summary>
        /// 网站根目录文件夹地址。
        /// </summary>
        public static string ContentRootPath => _environment.ContentRootPath;

        public static string WebRootPath => _environment.WebRootPath;

        /// <summary>
        /// 网站根目录访问地址。
        /// </summary>
        public static string ApplicationPath => _environment.ApplicationPath;

        /// <summary>
        /// API访问地址。
        /// </summary>
        public static string ApiUrl => _environment.ApiUrl;

        /// <summary>
        /// 管理员及权限Api接口。
        /// </summary>
        public static IAdminApi AdminApi => ApiCollection.AdminApi;

        /// <summary>
        /// 插件及系统配置Api接口。
        /// </summary>
        public static IConfigApi ConfigApi => ApiCollection.ConfigApi;

        /// <summary>
        /// 内容Api接口。
        /// </summary>
        public static IContentApi ContentApi => ApiCollection.ContentApi;

        /// <summary>
        /// 栏目Api接口。
        /// </summary>
        public static IChannelApi ChannelApi => ApiCollection.ChannelApi;

        /// <summary>
        /// STL解析Api接口。
        /// </summary>
        public static IParseApi ParseApi => ApiCollection.ParseApi;

        /// <summary>
        /// 插件Api接口。
        /// </summary>
        public static IPluginApi PluginApi => ApiCollection.PluginApi;

        /// <summary>
        /// 站点Api接口。
        /// </summary>
        public static ISiteApi SiteApi => ApiCollection.SiteApi;

        /// <summary>
        /// 用户Api接口。
        /// </summary>
        public static IUserApi UserApi => ApiCollection.UserApi;

        /// <summary>
        /// 工具类Api接口。
        /// </summary>
        public static IUtilsApi UtilsApi => ApiCollection.UtilsApi;

        /// <summary>
        /// 当前HTTP请求接口。
        /// </summary>
        public static IRequest Request => _requestFunc == null ? null : _requestFunc();

        public static IResponse Response => _responseFunc == null ? null : _responseFunc();
    }
}
