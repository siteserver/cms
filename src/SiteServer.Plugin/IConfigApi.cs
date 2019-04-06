namespace SiteServer.Plugin
{
    /// <summary>
    /// 插件及系统配置Api接口。
    /// </summary>
    public interface IConfigApi
    {
#pragma warning disable CS1573 // 参数“pluginId”在“IConfigApi.SetConfig(string, int, string, object)”的 XML 注释中没有匹配的 param 标记(但其他参数有)
        /// <summary>
        /// 存储当前插件的配置信息。
        /// </summary>
        /// <param name="siteId">
        /// 站点Id。
        /// 如果插件的配置信息与站点无关，可以将siteId设置为 0。
        /// </param>
        /// <param name="key">需要存储的配置信息键。</param>
        /// <param name="value">需要存储的配置信息值。</param>
        /// <returns>
        /// 如果设置成功，则为true；否则为false。
        /// </returns>
        bool SetConfig(string pluginId, int siteId, string key, object value);
#pragma warning restore CS1573 // 参数“pluginId”在“IConfigApi.SetConfig(string, int, string, object)”的 XML 注释中没有匹配的 param 标记(但其他参数有)

#pragma warning disable CS1573 // 参数“pluginId”在“IConfigApi.SetConfig(string, int, object)”的 XML 注释中没有匹配的 param 标记(但其他参数有)
        /// <summary>
        /// 存储当前插件的配置信息。
        /// </summary>
        /// <param name="siteId">
        /// 站点Id。
        /// 如果插件的配置信息与站点无关，可以将siteId设置为 0。
        /// </param>
        /// <param name="value">需要存储的配置信息值。</param>
        /// <returns>
        /// 如果设置成功，则为true；否则为false。
        /// 等同于SetConfig(siteId, string.Empty, value);
        /// </returns>
        bool SetConfig(string pluginId, int siteId, object value);
#pragma warning restore CS1573 // 参数“pluginId”在“IConfigApi.SetConfig(string, int, object)”的 XML 注释中没有匹配的 param 标记(但其他参数有)

#pragma warning disable CS1573 // 参数“pluginId”在“IConfigApi.GetConfig<T>(string, int, string)”的 XML 注释中没有匹配的 param 标记(但其他参数有)
        /// <summary>
        /// 获取当前插件的配置信息。
        /// </summary>
        /// <param name="siteId">
        /// 站点Id。
        /// 如果插件的配置信息与站点无关，可以将siteId设置为 0。
        /// </param>
        /// <param name="key">需要获取的配置信息键。</param>
        /// <typeparam name="T">配置字典中的值的类型。</typeparam>
        /// <returns>
        /// 如果找到指定键，则包含与该键相关的值；否则包含参数类型T的默认值。
        /// </returns>
        T GetConfig<T>(string pluginId, int siteId, string key = "");
#pragma warning restore CS1573 // 参数“pluginId”在“IConfigApi.GetConfig<T>(string, int, string)”的 XML 注释中没有匹配的 param 标记(但其他参数有)

#pragma warning disable CS1573 // 参数“pluginId”在“IConfigApi.RemoveConfig(string, int, string)”的 XML 注释中没有匹配的 param 标记(但其他参数有)
        /// <summary>
        /// 删除当前插件的配置信息。
        /// </summary>
        /// <param name="siteId">
        /// 站点Id。
        /// 如果插件的配置信息与站点无关，可以将siteId设置为 0。
        /// </param>
        /// <param name="key">需要删除的配置信息键。</param>
        /// <returns>
        /// 如果删除成功，则为true；否则为false。
        /// </returns>
        bool RemoveConfig(string pluginId, int siteId, string key = "");
#pragma warning restore CS1573 // 参数“pluginId”在“IConfigApi.RemoveConfig(string, int, string)”的 XML 注释中没有匹配的 param 标记(但其他参数有)
    }
}
