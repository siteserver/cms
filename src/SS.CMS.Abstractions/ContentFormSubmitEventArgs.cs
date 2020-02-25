using System;
using System.Collections.Generic;

namespace SS.CMS.Abstractions
{
    /// <summary>
    /// 为内容编辑（新增）页面的提交事件提供数据。
    /// </summary>
    public class ContentFormSubmitEventArgs : EventArgs
    {
        /// <summary>
        /// 初始化 <see cref="T:SS.CMS.Abstractions.ContentFormSubmitEventArgs" /> 类的新实例。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelId">栏目Id。</param>
        /// <param name="contentId">内容Id。</param>
        /// <param name="form">表单数据。</param>
        /// <param name="contentInfo">内容对象。</param>
        public ContentFormSubmitEventArgs(int siteId, int channelId, int contentId, IDictionary<string, object> form,
            Content contentInfo)
        {
            SiteId = siteId;
            ChannelId = channelId;
            ContentId = contentId;
            Form = form;
            ContentInfo = contentInfo;
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
        /// 即将保存至数据库的内容对象，可以从表单数据 <see cref="T:SS.CMS.Abstractions.IAttributes" /> 中获取属性值并设置到内容对象中。
        /// </summary>
        public Content ContentInfo { get; }
    }
}