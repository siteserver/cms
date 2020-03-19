using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

namespace SSCMS
{
    /// <summary>
    /// 插件服务注册接口。
    /// 插件服务注册接口是插件机制的核心，用于定义插件能够提供的各种服务，一个方法对应一个事件或者一个功能。
    /// </summary>
    public interface IPlugin : IPackageMetadata
    {
        /// <summary>
        /// 内容添加完成后的触发事件。
        /// </summary>
        event EventHandler<ContentEventArgs> ContentAddCompleted;

        /// <summary>
        /// 内容删除完成后的触发事件。
        /// </summary>
        event EventHandler<ContentEventArgs> ContentDeleteCompleted;

        /// <summary>
        /// 内容转移完成后的触发事件。
        /// </summary>
        event EventHandler<ContentTranslateEventArgs> ContentTranslateCompleted;

        /// <summary>
        /// 内容表单提交时的触发事件。
        /// </summary>
        event EventHandler<ContentFormSubmitEventArgs> ContentFormSubmit;

        /// <summary>
        /// 内容表单载入时的触发事件。
        /// </summary>
        event ContentFormLoadEventHandler ContentFormLoad;

        /// <summary>
        /// STL解析前的触发事件。
        /// </summary>
        event EventHandler<ParseEventArgs> BeforeStlParse;

        /// <summary>
        /// STL解析后的触发事件。
        /// </summary>
        event EventHandler<ParseEventArgs> AfterStlParse;

        /// <summary>
        /// 设置管理员登录后台后默认显示的页面地址。
        /// </summary>
        /// <param name="pageUrl">默认页面地址。</param>
        /// <returns>返回插件服务注册实例。</returns>
        IPlugin SetSystemDefaultPage(string pageUrl);

        /// <summary>
        /// 设置用户登录用户中心后默认显示的页面地址。
        /// </summary>
        /// <param name="pageUrl">默认页面地址。</param>
        /// <returns>返回插件服务注册实例。</returns>
        IPlugin SetHomeDefaultPage(string pageUrl);

        /// <summary>
        /// 添加插件的内容模型，包含内容存储的表名称以及内容表的字段列表。
        /// </summary>
        /// <param name="tableName">内容表名称。</param>
        /// <param name="tableColumns">内容表字段列表。</param>
        /// /// <param name="inputStyles">内容表单样式。</param>
        /// <returns>返回插件服务注册实例。</returns>
        IPlugin AddContentModel(string tableName, List<TableColumn> tableColumns, List<InputStyle> inputStyles);
        /// <summary>
        /// 添加插件的数据库表，包含表名称以及表字段列表。
        /// 此方法可以多次调用，系统将为此插件创建指定的数据库表结构。
        /// </summary>
        /// <param name="tableName">表名称。</param>
        /// <param name="tableColumns">表字段列表。</param>
        /// <returns>返回插件服务注册实例。</returns>
        IPlugin AddDatabaseTable(string tableName, List<TableColumn> tableColumns);

        /// <summary>
        /// 添加插件的内容列表显示项。
        /// </summary>
        /// <param name="columnName">内容列表显示项名称。</param>
        /// <param name="columnFunc">插件内容列表显示项生成方法，可以根据第一个参数IContentContext（内容上下文）计算并返回显示项的值。</param>
        /// <returns>返回插件服务注册实例。</returns>
        IPlugin AddContentColumn(string columnName, Func<IContentContext, string> columnFunc);

        /// <summary>
        /// 添加STL元素解析器。
        /// </summary>
        /// <param name="elementName">STL元素名称。</param>
        /// <param name="parse">STL元素解析方法，可以根据第一个参数IParseContext（STL解析上下文）计算并返回解析后的Html。</param>
        /// <returns>返回插件服务注册实例。</returns>
        IPlugin AddStlElementParser(string elementName, Func<IParseContext, string> parse);

        /// <summary>
        /// 添加REST Api插件授权。
        /// </summary>
        /// <returns>返回插件服务注册实例。</returns>
        IPlugin AddApiAuthorization();
        /// <summary>
        /// 添加SiteServer Cli命令行可以执行的任务。
        /// 实现此方法的插件将能够在SiteServer Cli命令行中运行任务。
        /// </summary>
        /// <param name="command">命令行命令。</param>
        /// <param name="job">可以执行的任务，可以根据第一个参数IJobContext（任务执行上下文）执行任务。</param>
        /// <returns>返回插件服务注册实例。</returns>
        IPlugin AddJob(string command, Func<IJobContext, Task> job);

        // added

        string SystemDefaultPageUrl { get; }
        string HomeDefaultPageUrl { get; }

        Dictionary<string, Func<IParseContext, string>> StlElementsToParse { get; }

        Dictionary<string, Func<IJobContext, Task>> Jobs { get; }

        Dictionary<string, Func<IContentContext, string>> ContentColumns { get; }

        /// <summary>
        /// 添加系统菜单。
        /// 系统菜单位于系统头部的插件管理下拉菜单中。
        /// <seealso cref="Menu"/>
        /// </summary>
        /// <example> 
        /// 下面的例子显示如何添加系统菜单。
        /// <code>
        /// public class Main : PluginBase
        /// {
        ///     public override void Startup(IService service)
        ///     {
        ///         service.AddSystemMenu(new Menu
        ///         {
        ///             Text = "插件菜单",
        ///             Href = "page.html"
        ///         });
        ///     }
        /// }
        /// </code>
        /// 下面的例子显示如何添加带有下级菜单的系统菜单。
        /// <code>
        /// public class Main : PluginBase
        /// {
        ///     public override void Startup(IService service)
        ///     {
        ///         service.AddSystemMenu(new Menu
        ///         {
        ///             Text = "插件菜单",
        ///             Href = "page.html",
        ///             Menus = new List&lt;Menu&gt;
        ///             {
        ///                 new Menu
        ///                 {
        ///                     Text = "下级菜单1",
        ///                     Href = "page1.html"
        ///                 },
        ///                 new Menu
        ///                 {
        ///                     Text = "下级菜单1",
        ///                     Href = "page2.html"
        ///                 }
        ///             }
        ///         });
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <returns>返回插件服务注册实例。</returns>
        List<Menu> GetSystemMenus();

        Task<List<Menu>> GetSystemMenusAsync();

        /// <summary>
        /// 添加站点菜单。
        /// 站点菜单位于系统左侧的插件管理菜单中。
        /// 此菜单的Url地址将自动加上对应的站点Id。
        /// </summary>
        /// <returns>返回插件菜单生成方法，可以根据第一个参数siteId（站点Id）计算并返回菜单</returns>
        List<Menu> GetSiteMenus(int siteId);

        Task<List<Menu>> GetSiteMenusAsync(int siteId);

        /// <summary>
        /// 添加用户中心菜单。
        /// <seealso cref="Menu"/>
        /// </summary>
        /// <example> 
        /// 下面的例子显示如何添加用户中心菜单。
        /// <code>
        /// public class Main : PluginBase
        /// {
        ///     public override void Startup(IService service)
        ///     {
        ///         service.AddHomeMenu(new Menu
        ///         {
        ///             Text = "用户中心菜单",
        ///             Href = "page.html"
        ///         });
        ///     }
        /// }
        /// </code>
        /// 下面的例子显示如何添加带有下级菜单的系统菜单。
        /// <code>
        /// public class Main : PluginBase
        /// {
        ///     public override void Startup(IService service)
        ///     {
        ///         service.AddHomeMenu(new Menu
        ///         {
        ///             Text = "用户中心菜单",
        ///             Href = "page.html",
        ///             Menus = new List&lt;Menu&gt;
        ///             {
        ///                 new Menu
        ///                 {
        ///                     Text = "下级菜单1",
        ///                     Href = "page1.html"
        ///                 },
        ///                 new Menu
        ///                 {
        ///                     Text = "下级菜单1",
        ///                     Href = "page2.html"
        ///                 }
        ///             }
        ///         });
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <returns>返回插件服务注册实例。</returns>
        List<Menu> GetHomeMenus();

        Task<List<Menu>> GetHomeMenusAsync();

        /// <summary>
        /// 添加内容菜单。
        /// 内容菜单位于内容管理的内容列表中。
        /// </summary>
        /// <returns>返回插件菜单生成方法，可以根据内容上下文计算并返回菜单。</returns>
        List<Menu> GetContentMenus(Content content);

        Task<List<Menu>> GetContentMenusAsync(Content content);

        string ContentTableName { get; }
        bool IsApiAuthorization { get; }

        List<TableColumn> ContentTableColumns { get; }

        List<InputStyle> ContentInputStyles { get; }

        Dictionary<string, List<TableColumn>> DatabaseTables { get; }

        void OnContentAddCompleted(ContentEventArgs e);

        void OnContentDeleteCompleted(ContentEventArgs e);

        void OnContentTranslateCompleted(ContentTranslateEventArgs e);

        string OnContentFormLoad(ContentFormLoadEventArgs e);

        void OnContentFormSubmit(ContentFormSubmitEventArgs e);

        void OnBeforeStlParse(ParseEventArgs e);

        void OnAfterStlParse(ParseEventArgs e);
    }
}