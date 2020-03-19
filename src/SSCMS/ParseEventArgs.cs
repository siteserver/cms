using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMS
{
    /// <summary>
    /// 为STL解析事件提供数据。
    /// </summary>
    public class ParseEventArgs : EventArgs
    {
        /// <summary>
        /// 初始化 <see cref="T:SSCMS.ParseEventArgs" /> 类的新实例。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelId">栏目Id。</param>
        /// <param name="contentId">内容Id。</param>
        /// <param name="contentInfo">内容实体。</param>
        /// <param name="templateType">模板类型。</param>
        /// <param name="templateId">模板Id。</param>
        /// <param name="filePath">生成页面的绝对路径。</param>
        /// <param name="headCodes">生成的Html页面中包含在head标签内的代码。</param>
        /// <param name="bodyCodes">生成的Html页面中包含在body标签内的代码。</param>
        /// <param name="footCodes">生成的Html页面中包含在页面最底部的代码。</param>
        /// <param name="contentBuilder">生成的Html页面代码。</param>
        public ParseEventArgs(int siteId, int channelId, int contentId, Content contentInfo, TemplateType templateType, int templateId, string filePath, SortedDictionary<string, string> headCodes, SortedDictionary<string, string> bodyCodes, SortedDictionary<string, string> footCodes, StringBuilder contentBuilder)
        {
            SiteId = siteId;
            ChannelId = channelId;
            ContentId = contentId;
            ContentInfo = contentInfo;
            TemplateType = templateType;
            TemplateId = templateId;
            FilePath = filePath;
            HeadCodes = headCodes;
            BodyCodes = bodyCodes;
            FootCodes = footCodes;
            ContentBuilder = contentBuilder;
        }

        /// <summary>
        /// 站点Id。
        /// </summary>
        public int SiteId { get; }

        /// <summary>
        /// 栏目Id。
        /// </summary>
        public int ChannelId { get; }

        /// <summary>
        /// 内容Id。
        /// </summary>
        public int ContentId { get; }

        /// <summary>
        /// 内容实体。
        /// </summary>
        Content ContentInfo { get; }

        /// <summary>
        /// 模板类型。
        /// </summary>
        public TemplateType TemplateType { get; }

        /// <summary>
        /// 模板Id。
        /// </summary>
        public int TemplateId { get; }

        /// <summary>
        /// 生成页面的绝对路径。
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// 生成的Html页面中包含在head标签内的代码。
        /// </summary>
        public SortedDictionary<string, string> HeadCodes { get; }

        /// <summary>
        /// 生成的Html页面中包含在body标签内的代码。
        /// </summary>
        public SortedDictionary<string, string> BodyCodes { get; }

        /// <summary>
        /// 生成的Html页面中包含在页面最底部的代码。
        /// </summary>
        public SortedDictionary<string, string> FootCodes { get; }

        /// <summary>
        /// 生成的Html页面代码。
        /// </summary>
        public StringBuilder ContentBuilder { get; }
    }
}