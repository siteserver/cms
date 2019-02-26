namespace SiteServer.Plugin
{
    /// <summary>
    /// 插件Api接口。
    /// </summary>
    public interface IPluginApi
    {
        /// <summary>
        /// 获取当前插件文件夹下的目录/文件路径。
        /// </summary>
        /// <param name="pluginId">插件Id。</param>
        /// <param name="relatedPath">相对路径。</param>
        /// <returns>当前插件文件夹下的目录/文件绝对路径。</returns>
        string GetPluginPath(string pluginId, string relatedPath = "");

        /// <summary>
        /// 获取当前插件文件夹下的目录/文件访问Url地址。
        /// </summary>
        /// <param name="pluginId">插件Id。</param>
        /// <param name="relatedUrl">相对地址。</param>
        /// <returns>当前插件文件夹下的目录/文件访问Url地址。</returns>
        string GetPluginUrl(string pluginId, string relatedUrl = "");

        /// <summary>
        /// 当前插件的Rest Api访问Url地址。
        /// </summary>
        string GetPluginApiUrl(string pluginId);

        /// <summary>
        /// 通过插件Id获取插件实例。
        /// </summary>
        /// <typeparam name="T">插件类。</typeparam>
        /// <returns>插件实例。</returns>
        T GetPlugin<T>() where T : PluginBase;
    }
}
