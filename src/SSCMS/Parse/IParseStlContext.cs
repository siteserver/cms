using System.Collections.Specialized;

namespace SSCMS.Parse
{
    /// <summary>
    /// STL解析上下文。
    /// </summary>
    public interface IParseStlContext : IParseContext
    {
        /// <summary>
        /// 当前解析的STL标签的属性键值集合。
        /// </summary>
        NameValueCollection StlAttributes { get; }

        /// <summary>
        /// 当前解析的STL标签的完整代码，而不仅限于标签内部的内容。
        /// </summary>
        string StlOuterHtml { get; }

        /// <summary>
        /// 当前解析的STL标签内部的内容。
        /// </summary>
        string StlInnerHtml { get; }

        /// <summary>
        /// 判断当前解析的STL标签是STL元素还是STL实体，如果是元素，则返回true；如果是实体，则返回false。
        /// </summary>
        bool IsStlEntity { get; }
    }
}