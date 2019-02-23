using System.Collections.Generic;

namespace SiteServer.Plugin
{
    /// <summary>
    /// STL解析Api接口。
    /// </summary>
    public interface IParseApi
    {
        /// <summary>
        /// 获取html中的指定STL标签。
        /// </summary>
        /// <param name="html">Html代码。</param>
        /// <param name="stlElementNames">需要获取的STL标签名称列表。</param>
        /// <returns>返回STL标签名称与STL标签组成的数据字典。</returns>
        Dictionary<string, string> GetStlElements(string html, List<string> stlElementNames);

        /// <summary>
        /// 解析HTML中的STL标签。
        /// </summary>
        /// <param name="html">带有STL标签的Html代码。</param>
        /// <param name="context">STL解析上下文。</param>
        /// <returns>返回将STL标签解析完毕后的Html代码。</returns>
        string Parse(string html, IParseContext context);

        /// <summary>
        /// 解析Html标签属性中的STL实体。
        /// </summary>
        /// <param name="attributeValue">属性值。</param>
        /// <param name="context">STL解析上下文。</param>
        /// <returns>返回将STL实体解析完毕后的Html属性值。</returns>
        string ParseAttributeValue(string attributeValue, IParseContext context);

        /// <summary>
        /// 获取当前生成的静态页面的地址。
        /// </summary>
        /// <param name="context">STL解析上下文。</param>
        /// <returns>当前生成的静态页面的地址。</returns>
        string GetCurrentUrl(IParseContext context);
    }
}