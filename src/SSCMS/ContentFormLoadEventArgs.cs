using System;
using System.Collections.Generic;

namespace SSCMS
{
    /// <summary>
    /// 为内容编辑（新增）页面的载入事件提供数据。
    /// </summary>
    public class ContentFormLoadEventArgs : EventArgs
    {
        /// <summary>
        /// 初始化 <see cref="T:SSCMS.ContentFormLoadEventArgs" /> 类的新实例。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelId">栏目Id。</param>
        /// <param name="contentId">内容Id。</param>
        /// <param name="form">表单数据。</param>
        /// <param name="attributeName">内容属性名称。</param>
        /// <param name="attributeHtml">内容属性Html标签。</param>
        public ContentFormLoadEventArgs(int siteId, int channelId, int contentId, IDictionary<string, object> form,
            string attributeName, string attributeHtml)
        {
            SiteId = siteId;
            ChannelId = channelId;
            ContentId = contentId;
            Form = form;
            AttributeName = attributeName;
            AttributeHtml = attributeHtml;
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
        /// 如果内容Id为0，则表示当前载入的页面为内容添加页面，否则当前载入的页面为内容编辑页面。
        /// </summary>
        public int ContentId { get; }

        /// <summary>
        /// 表单数据。
        /// </summary>
        public IDictionary<string, object> Form { get; }

        /// <summary>
        /// 内容属性名称，代表内容编辑（新增）页面中的内容字段名称。
        /// </summary>
        public string AttributeName { get; }

        /// <summary>
        /// 内容属性Html标签，内容属性在编辑（新增）页面中的Html标签。
        /// </summary>
        public string AttributeHtml { get; }
    }
}