using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Enums;
using SSCMS.Services;

namespace SSCMS.Parse
{
    /// <summary>
    /// STL解析上下文。
    /// </summary>
    public interface IParseContext
    {
        /// <summary>
        /// 站点Id。
        /// </summary>
        int SiteId { get; }

        /// <summary>
        /// 栏目Id。
        /// </summary>
        int ChannelId { get; }

        /// <summary>
        /// 内容Id。
        /// </summary>
        int ContentId { get; }

        /// <summary>
        /// 模板类型。
        /// </summary>
        TemplateType TemplateType { get; }

        /// <summary>
        /// 模板Id。
        /// </summary>
        int TemplateId { get; }

        /// <summary>
        /// 生成的Html页面中包含在head标签内的代码。
        /// </summary>
        SortedDictionary<string, string> HeadCodes { get; }

        /// <summary>
        /// 生成的Html页面中包含在body标签内的代码。
        /// </summary>
        SortedDictionary<string, string> BodyCodes { get; }

        /// <summary>
        /// 生成的Html页面中包含在页面最底部的代码。
        /// </summary>
        SortedDictionary<string, string> FootCodes { get; }

        /// <summary>
        /// 将键/值对放入STL解析上下文中，对包含下级标签STL解析情况下共享数据有用。
        /// 注意：该数据是不稳定的 —— 它在当前STL解析完成后将丢失。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="objectValue">值。</param>
        void Set<T>(string key, T objectValue);


        /// <summary>
        /// 从STL解析上下文中获取指定键的值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>如果STL解析上下文中存在指定的键，则返回对应的值；否则返回类型T的默认值。</returns>
        T Get<T>(string key);

        Task<string> ParseAsync(string template);

        Task<string> GetCurrentUrlAsync();

        IParseManager ParseManager { get; }
    }
}